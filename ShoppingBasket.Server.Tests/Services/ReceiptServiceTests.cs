using Mapster;
using Microsoft.Extensions.Logging.Abstractions;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Enums;
using ShoppingBasket.Server.Models;
using ShoppingBasket.Server.Repositories;
using ShoppingBasket.Server.Services;
using ShoppingBasket.Server.Tests.Fakes;
using Xunit;

namespace ShoppingBasket.Server.Tests.Services
{
    public class ReceiptServiceTests
    {
        private const decimal SoupPrice = 0.65m;
        private const decimal BreadPrice = 0.80m;
        private readonly long _soupItemId = 1;
        private readonly long _breadItemId = 2;

        private ReceiptService CreateService(IItemRepository itemRepo, IDiscountRepository discountRepo, out FakeReceiptRepository receiptRepo)
        {
            receiptRepo = new FakeReceiptRepository();
            var loggerFactory = NullLoggerFactory.Instance;

            // ensure Mapster won't fail during mapping calls in service
            Mapster.TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            return new ReceiptService(receiptRepo, itemRepo, discountRepo, loggerFactory);
        }

        [Fact]
        public async Task CreateReceipt_AllZeroQuantities_ThrowsBadRequestException()
        {
            // Arrange
            var items = new[] {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Price = SoupPrice },
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Price = BreadPrice }
            };
           
            var itemRepo = new FakeItemRepository(items);
            var discountRepo = new FakeDiscountRepository(Array.Empty<Discount>());

            var service = CreateService(itemRepo, discountRepo, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 0, BreadQuantity = 0, MilkQuantity = 0, ApplesQuantity = 0 };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => service.CreateReceiptAsync(dto));
        }

        [Fact]
        public async Task CreateReceipt_NegativeQuantities_TreatedAsZero_ThrowsBadRequestException()
        {
            // Negative quantities are filtered out in the service; when all are <=0 the service should throw.
            var items = new[] {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Price = SoupPrice }
            };
            var itemRepo = new FakeItemRepository(items);
            var discountRepo = new FakeDiscountRepository(Array.Empty<Discount>());

            var service = CreateService(itemRepo, discountRepo, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = -1, BreadQuantity = 0, MilkQuantity = 0, ApplesQuantity = 0 };

            await Assert.ThrowsAsync<BadRequestException>(() => service.CreateReceiptAsync(dto));
        }

        [Fact]
        public async Task CreateReceipt_NoItemsInRepository_ThrowsBadRequestException()
        {
            // Arrange: item repo returns empty list -> service should detect "Requested items not found in catalog."
            var itemRepo = new FakeItemRepository(Array.Empty<Item>());
            var discountRepo = new FakeDiscountRepository(Array.Empty<Discount>());

            var service = CreateService(itemRepo, discountRepo, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 1, BreadQuantity = 0, MilkQuantity = 0, ApplesQuantity = 0 };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => service.CreateReceiptAsync(dto));
        }

        [Fact]
        public async Task CreateReceipt_ItemRepositoryReturnsNull_ThrowsException()
        {
            // Arrange: mimic a broken repository that returns null; service should detect "No items found in catalog.".
            var itemRepo = new FakeItemRepository_ReturnsNull();
            var discountRepo = new FakeDiscountRepository(Array.Empty<Discount>());

            var service = CreateService(itemRepo, discountRepo, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 1, BreadQuantity = 0, MilkQuantity = 0, ApplesQuantity = 0 };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => service.CreateReceiptAsync(dto));
        }

        [Fact]
        public async Task CreateReceipt_NoDiscounts_NoDiscountLinesPresent()
        {
            // Arrange: items exist but discounts empty -> no discounted lines expected
            var items = new[] {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Price = SoupPrice },
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Price = BreadPrice }
            };
            var itemRepo = new FakeItemRepository(items);
            var discountRepo = new FakeDiscountRepository(Array.Empty<Discount>());

            var service = CreateService(itemRepo, discountRepo, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 2, BreadQuantity = 1, MilkQuantity = 0, ApplesQuantity = 0 };

            // Act
            var result = await service.CreateReceiptAsync(dto);

            // Assert: both lines exist, but no line is marked discounted
            Assert.NotNull(result);
            Assert.Equal(2, result.ItemsOrdered.Count());
            Assert.All(result.ItemsOrdered, io => Assert.False(io.IsDiscounted));
        }

        [Fact]
        public async Task CreateReceipt_DiscountForDifferentItem_Ignored()
        {
            // Arrange: discount exists but attached to milk (not bread) -> bread should not get discounted
            var items = new[] {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Price = SoupPrice },
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Price = BreadPrice },
                new Item { ItemId = 99, ItemType = ItemType.Milk, Price = 1.00m }
            };
            var discounts = new[] {
                new Discount { DiscountId = 42, ItemId = 99, DiscountType = DiscountType.MultiBuy, Percentage = 50m, IsActive = true }
            };

            var itemRepo = new FakeItemRepository(items);
            var discountRepo = new FakeDiscountRepository(discounts);

            var service = CreateService(itemRepo, discountRepo, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 2, BreadQuantity = 1, MilkQuantity = 0, ApplesQuantity = 0 };

            // Act
            var created = await service.CreateReceiptAsync(dto);

            // Assert
            var breadLine = created.ItemsOrdered.Single(io => io.ItemId == _breadItemId);
            Assert.False(breadLine.IsDiscounted);
        }
    }
}