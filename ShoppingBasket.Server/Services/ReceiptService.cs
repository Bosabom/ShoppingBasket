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
            var itemsOrderedToCreate = await SetItemsOrdered(selectedItems, itemTypesQuantities);

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

        private async Task<List<ItemOrdered>> SetItemsOrdered(List<Item> selectedItems, KeyValuePair<ItemType, int>[] TypeQuantityKeyValuePairs)
        {
            var itemsOrdered = new List<ItemOrdered>();

            foreach (var keyValuePair in TypeQuantityKeyValuePairs)
            {
                var itemType = keyValuePair.Key;
                var qty = keyValuePair.Value;

                if (qty <= 0) continue;

                var item = selectedItems.SingleOrDefault(i => i.ItemType == itemType);

                // create ItemOrdered only if item exists and calculate costs
                if (item != null)
                {
                    decimal calculatedSubTotalCost = PriceCalculationHelper.CalculateSubTotalCost(item.Price, qty);
                    var itemOrderedToCreate = new ItemOrdered
                    {
                        ItemId = item.ItemId,
                        Quantity = qty,
                        SubTotalCost = calculatedSubTotalCost,
                        IsDiscounted = false,
                        TotalCost = calculatedSubTotalCost
                    };

                    // fetch discount for the item if any
                    var discountForItem = await _discountRepository.GetByItemIdAsync(item.ItemId);

                    // apply discount if any
                    if (discountForItem != null)
                    {
                        //TODO: implement more discount types if needed
                        //TODO: Multi-buy: Buy 2 tins of soup and get a loaf of bread for half price. 
                        itemOrderedToCreate.IsDiscounted = true;
                        itemOrderedToCreate.DiscountId = discountForItem.DiscountId;
                        if (discountForItem.Percentage.HasValue)
                        {
                            decimal calculatedDiscountedCost = PriceCalculationHelper.CalculateDiscountedCost(item.Price, qty, discountForItem.Percentage.Value);
                            itemOrderedToCreate.DiscountedCost = calculatedDiscountedCost;
                            itemOrderedToCreate.TotalCost = calculatedDiscountedCost;
                        }
                    }
                    itemsOrdered.Add(itemOrderedToCreate);
                }
            }

            return itemsOrdered;
        }
    }
}
