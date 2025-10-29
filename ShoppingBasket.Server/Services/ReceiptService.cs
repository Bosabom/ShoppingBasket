using AutoMapper;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Enums;
using ShoppingBasket.Server.MappingProfiles;
using ShoppingBasket.Server.Models;
using ShoppingBasket.Server.Repositories;
using ShoppingBasket.Server.Utils;

namespace ShoppingBasket.Server.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        //TODO: inject services instead of repositories if needed not only direct data access
        private readonly IItemRepository _itemRepository;
        private readonly IDiscountRepository _discountRepository;
        private ILoggerFactory _loggerFactory;
        private readonly Mapper _mapper;
        private readonly MapperConfiguration _mapperConfig;

        public ReceiptService(IReceiptRepository receiptRepository, 
            IItemRepository itemRepository, 
            IDiscountRepository discountRepository, 
            ILoggerFactory loggerFactory)
        {
            _receiptRepository = receiptRepository;
            _itemRepository = itemRepository;
            _discountRepository = discountRepository;
            _loggerFactory = loggerFactory;
            _mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ReceiptMappingProfile());
            }, _loggerFactory);
            _mapper = new Mapper(_mapperConfig);
        }

        public async Task<IEnumerable<ReceiptDto>> GetAllReceiptsAsync()
        {
            var receipts = await _receiptRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Receipt>, IEnumerable<ReceiptDto>>(receipts);
        }

        public async Task<ReceiptDto> GetReceiptByIdAsync(long id)
        {
            var receipt = await _receiptRepository.GetByIdAsync(id);
            return _mapper.Map<Receipt, ReceiptDto>(receipt);
        }

        public async Task<ReceiptDto> GetDetailedReceiptByIdAsync(long id)
        {
            var receipt = await _receiptRepository.GetDetailedByIdAsync(id);
            return _mapper.Map<Receipt, ReceiptDto>(receipt);
        }

        public async Task<IEnumerable<ReceiptShortDto>> GetReceiptsHistoryAsync()
        {
            var receipts = await _receiptRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Receipt>, IEnumerable<ReceiptShortDto>>(receipts);
        }

        //TODO: refactor this method in future
        public async Task<ReceiptDto> CreateReceiptAsync(ReceiptCreateDto receiptCreateDto)
        {
            var requested = new Dictionary<ItemType, int>()
            {
                [ItemType.Soup] = receiptCreateDto.SoupQuantity,
                [ItemType.Bread] = receiptCreateDto.BreadQuantity,
                [ItemType.Milk] = receiptCreateDto.MilkQuantity,
                [ItemType.Apple] = receiptCreateDto.ApplesQuantity
            };

            var itemTypesQuantities = requested.Where(kv => kv.Value > 0).ToArray();

            if (itemTypesQuantities.Length == 0)
            {
                throw new BadRequestException("At least one item quantity must be greater than zero.");
            }

            // load items from DB
            var items = await _itemRepository.GetAllAsync();

            // filter items to only those requested by itemType
            var selectedItems = items.Where(i => itemTypesQuantities.Any(kv => kv.Key == i.ItemType)).ToList();

            //verify all requested items exist
            if (selectedItems is null || selectedItems.Count == 0)
            {
                throw new BadRequestException("Requested items not found in catalog.");
            }

            //Create items ordered
            var itemsOrderedToCreate = await BuildItemsOrdered(selectedItems, itemTypesQuantities);

            //Calculate receipt total cost
            decimal receiptTotalCost = itemsOrderedToCreate.Sum(i => i.TotalCost);

            var receiptToCreate = new Receipt
            {
                CreatedDateTime = DateTime.UtcNow,
                TotalCost = receiptTotalCost,
                ItemsOrdered = itemsOrderedToCreate
            };

            var createdReceipt = await _receiptRepository.CreateAsync(receiptToCreate);
            return _mapper.Map<Receipt, ReceiptDto>(createdReceipt);
        }

        private async Task<List<ItemOrdered>> BuildItemsOrdered(List<Item> selectedItems, KeyValuePair<ItemType, int>[] TypeQuantityKeyValuePairs)
        {
            var itemsOrdered = new List<ItemOrdered>();

            // compute quantities map for cross-item discounts
            var quantities = TypeQuantityKeyValuePairs.ToDictionary(k => k.Key, v => v.Value);

            quantities.TryGetValue(ItemType.Soup, out var soupQty);
            quantities.TryGetValue(ItemType.Bread, out var breadQty);

            // Determine how many bread loaves are eligible for the "Buy 2 soups => 1 bread at half price"
            var eligibleDiscountedBreadUnits = Math.Min(breadQty, soupQty / 2);

            foreach (var keyValuePair in TypeQuantityKeyValuePairs)
            {
                var itemType = keyValuePair.Key;
                var qty = keyValuePair.Value;

                if (qty <= 0) continue;

                var item = selectedItems.SingleOrDefault(i => i.ItemType == itemType);

                // create ItemOrdered only if item exists and calculate costs
                if (item != null)
                {
                    // fetch discount for the item if any
                    var discountForItem = await _discountRepository.GetByItemIdAsync(item.ItemId);

                    // TODO: refactor multi-buy discount handling for more complex scenarios
                    int multiBuyDiscountedUnits = 0;
                    // For multi-buy discounts that affect this item (bread), pass the number of discounted units.
                    if (itemType == ItemType.Bread && discountForItem != null && discountForItem.DiscountType == DiscountType.MultiBuy)
                    {
                        multiBuyDiscountedUnits = eligibleDiscountedBreadUnits;
                    }

                    // create ItemOrdered and apply discount if any
                    var itemOrderedToCreate = SetItemOrdered(item.ItemId, item.Price, qty, discountForItem, multiBuyDiscountedUnits);
                    itemsOrdered.Add(itemOrderedToCreate);
                }
            }
            return itemsOrdered;
        }

        private ItemOrdered SetItemOrdered(long itemId, decimal pricePerItem, int quantity, Discount? discount = null, int multiBuyDiscountedUnits = 0)
        {
            decimal calculatedSubTotalCost = PriceCalculationHelper.CalculateSubTotalCost(pricePerItem, quantity);
            var itemOrdered = new ItemOrdered
            {
                ItemId = itemId,
                Quantity = quantity,
                SubTotalCost = calculatedSubTotalCost,
                IsDiscounted = false,
                TotalCost = calculatedSubTotalCost
            };

            // apply discount if any
            if (discount != null)
            {
                itemOrdered.IsDiscounted = true;
                itemOrdered.DiscountId = discount.DiscountId;

                switch (discount.DiscountType)
                {
                    case DiscountType.Percentage:
                        if (discount.Percentage.HasValue)
                        {
                            decimal calculatedDiscountedCost = PriceCalculationHelper.CalculateDiscountedCost(pricePerItem, quantity, discount.Percentage.Value);
                            itemOrdered.DiscountedCost = calculatedDiscountedCost;
                            itemOrdered.TotalCost = calculatedDiscountedCost;
                        }
                        break;

                    //TODO: extend multi-buy logic for more complex scenarios
                    case DiscountType.MultiBuy:
                        // e.g. buy 2 soups => 1 bread at 50% off: discount.Percentage should represent 50
                        if (multiBuyDiscountedUnits > 0 && discount.Percentage.HasValue)
                        {
                            // cost for discounted units
                            decimal discountedPartTotal = PriceCalculationHelper.CalculateDiscountedCost(pricePerItem, multiBuyDiscountedUnits, discount.Percentage.Value);
                            // cost for remaining units (full price)
                            int remainingUnits = Math.Max(0, quantity - multiBuyDiscountedUnits);
                            decimal remainingPartTotal = PriceCalculationHelper.CalculateSubTotalCost(pricePerItem, remainingUnits);
                            // total cost = discounted part + remaining part
                            decimal total = Math.Round(discountedPartTotal + remainingPartTotal, 2);

                            itemOrdered.DiscountedCost = discountedPartTotal;
                            itemOrdered.TotalCost = total;
                        }
                        else if (discount.Percentage.HasValue && multiBuyDiscountedUnits >= quantity)
                        {
                            // All units discounted (fallback): apply percentage to all
                            decimal calculatedDiscountedCost = PriceCalculationHelper.CalculateDiscountedCost(pricePerItem, quantity, discount.Percentage.Value);
                            itemOrdered.DiscountedCost = calculatedDiscountedCost;
                            itemOrdered.TotalCost = calculatedDiscountedCost;
                        }
                        break;

                    default:
                        break;
                }
            }
            return itemOrdered;
        }
    }
}
