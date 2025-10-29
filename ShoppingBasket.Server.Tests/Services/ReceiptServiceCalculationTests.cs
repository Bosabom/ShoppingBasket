using Mapster;
using Microsoft.Extensions.Logging.Abstractions;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Enums;
using ShoppingBasket.Server.Models;
using ShoppingBasket.Server.Services;
using ShoppingBasket.Server.Tests.Fakes;
using ShoppingBasket.Server.Utils;

namespace ShoppingBasket.Server.Tests.Services
{
    public class ReceiptServiceCalculationTests
    {
        // Prices used in tests
        private const decimal SoupPrice = 0.65m;
        private const decimal BreadPrice = 0.80m;
        private const decimal ApplePrice = 1.00m;
        private readonly long _soupItemId = 1;
        private readonly long _breadItemId = 2;
        private readonly long _appleItemId = 3;

        private ReceiptService CreateService(IEnumerable<Item> items, IEnumerable<Discount> discounts, out FakeReceiptRepository receiptRepo)
        {
            var itemRepo = new FakeItemRepository(items);
            var discountRepo = new FakeDiscountRepository(discounts);
            receiptRepo = new FakeReceiptRepository();
            var loggerFactory = NullLoggerFactory.Instance;

            // Ensure Mapster default config is available for mapping in service
            Mapster.TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            return new ReceiptService(receiptRepo, itemRepo, discountRepo, loggerFactory);
        }

        [Fact]
        public async Task PercentageDiscountOnApple_AppliesCorrectly_TotalAndLineCorrect()
        {
            // Arrange
            var items = new[]
            {
                new Item { ItemId = _appleItemId, ItemType = ItemType.Apple, Description = "Apple", Price = ApplePrice }
            };
            var discounts = new[]
            {
                new Discount { DiscountId = 50, ItemId = _appleItemId, DiscountType = DiscountType.Percentage, Percentage = 10m, IsActive = true }
            };

            var service = CreateService(items, discounts, out var receiptRepo);

            var createDto = new ReceiptCreateDto
            {
                SoupQuantity = 0,
                BreadQuantity = 0,
                MilkQuantity = 0,
                ApplesQuantity = 1
            };

            // Expected: 1 apple at 10% off => 1.00 - 0.10 = 0.90
            var expectedAppleTotal = PriceCalculationHelper.CalculateDiscountedCost(ApplePrice, 1, 10m);
            var expectedReceiptTotal = expectedAppleTotal;

            // Act
            var created = await service.CreateReceiptAsync(createDto);

            // Assert
            Assert.NotNull(created);
            Assert.Equal(expectedReceiptTotal, created.TotalCost);

            var appleLine = created.ItemsOrdered.Single(io => io.ItemId == _appleItemId);
            Assert.True(appleLine.IsDiscounted);
            Assert.Equal(1, appleLine.Quantity);
            Assert.Equal(expectedAppleTotal, appleLine.TotalCost);
        }

        [Fact]
        public async Task Buy2SoupsAnd1Bread_AppliesHalfPriceOnBread_TotalAndLinesCorrect()
        {
            // Arrange
            var items = new[]
            {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Description = "Soup", Price = SoupPrice },
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Description = "Bread", Price = BreadPrice }
            };
            var discounts = new[]
            {
                // Multi-buy discount attached to bread: 50% (half price)
                new Discount { DiscountId = 10, ItemId = _breadItemId, DiscountType = DiscountType.MultiBuy, Percentage = 50m, IsActive = true }
            };

            var service = CreateService(items, discounts, out var receiptRepo);

            var createDto = new ReceiptCreateDto
            {
                SoupQuantity = 2,
                BreadQuantity = 1,
                MilkQuantity = 0,
                ApplesQuantity = 0
            };

            // Expected calculations:
            // 2 soups = 2 * 0.65 = 1.30
            // 1 bread at half price = 0.80 * 0.5 = 0.40
            var expectedSoupTotal = PriceCalculationHelper.CalculateSubTotalCost(SoupPrice, 2);
            var expectedBreadTotal = PriceCalculationHelper.CalculateDiscountedCost(BreadPrice, 1, 50m);
            var expectedReceiptTotal = Math.Round(expectedSoupTotal + expectedBreadTotal, 2);

            // Act
            var created = await service.CreateReceiptAsync(createDto);

            // Assert
            Assert.NotNull(created);
            Assert.Equal(expectedReceiptTotal, created.TotalCost);

            var breadLine = created.ItemsOrdered.SingleOrDefault(io => io.ItemId == _breadItemId);
            Assert.NotNull(breadLine);
            Assert.True(breadLine.IsDiscounted);
            Assert.Equal(1, breadLine.Quantity);
            Assert.Equal(expectedBreadTotal, breadLine.TotalCost);

            var soupLine = created.ItemsOrdered.SingleOrDefault(io => io.ItemId == _soupItemId);
            Assert.NotNull(soupLine);
            Assert.False(soupLine.IsDiscounted);
            Assert.Equal(2, soupLine.Quantity);
            Assert.Equal(expectedSoupTotal, soupLine.TotalCost);
        }

        [Fact]
        public async Task Buy3SoupsAnd2Bread_OnlyOneBreadDiscounted_TotalAndLinesCorrect()
        {
            // Arrange
            var items = new[]
            {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Description = "Soup", Price = SoupPrice },
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Description = "Bread", Price = BreadPrice }
            };
            var discounts = new[]
            {
                new Discount { DiscountId = 20, ItemId = _breadItemId, DiscountType = DiscountType.MultiBuy, Percentage = 50m, IsActive = true }
            };

            var service = CreateService(items, discounts, out var receiptRepo);

            var createDto = new ReceiptCreateDto
            {
                SoupQuantity = 3,
                BreadQuantity = 2,
                MilkQuantity = 0,
                ApplesQuantity = 0
            };

            // Eligible discounted bread units = floor(3 / 2) = 1 (only one bread at 50% off)
            var soupTotal = PriceCalculationHelper.CalculateSubTotalCost(SoupPrice, 3);
            var discountedBreadPart = PriceCalculationHelper.CalculateDiscountedCost(BreadPrice, 1, 50m);
            var remainingBreadPart = PriceCalculationHelper.CalculateSubTotalCost(BreadPrice, 1);
            var expectedBreadTotal = Math.Round(discountedBreadPart + remainingBreadPart, 2);
            var expectedReceiptTotal = Math.Round(soupTotal + expectedBreadTotal, 2);

            // Act
            var created = await service.CreateReceiptAsync(createDto);

            // Assert
            Assert.NotNull(created);
            Assert.Equal(expectedReceiptTotal, created.TotalCost);

            var breadLine = created.ItemsOrdered.SingleOrDefault(io => io.ItemId == _breadItemId);
            Assert.NotNull(breadLine);
            Assert.True(breadLine.IsDiscounted);
            Assert.Equal(2, breadLine.Quantity);
            Assert.Equal(expectedBreadTotal, breadLine.TotalCost);

            var soupLine = created.ItemsOrdered.SingleOrDefault(io => io.ItemId == _soupItemId);
            Assert.NotNull(soupLine);
            Assert.False(soupLine.IsDiscounted);
            Assert.Equal(3, soupLine.Quantity);
            Assert.Equal(soupTotal, soupLine.TotalCost);
        }

        [Fact]
        public async Task Buy4SoupsAnd2Bread_BothBreadDiscounted_TotalAndLinesCorrect()
        {
            // Arrange
            var items = new[]
            {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Description = "Soup", Price = SoupPrice },
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Description = "Bread", Price = BreadPrice }
            };
            var discounts = new[]
            {
                new Discount { DiscountId = 30, ItemId = _breadItemId, DiscountType = DiscountType.MultiBuy, Percentage = 50m, IsActive = true }
            };

            var service = CreateService(items, discounts, out var receiptRepo);

            var createDto = new ReceiptCreateDto
            {
                SoupQuantity = 4,
                BreadQuantity = 2,
                MilkQuantity = 0,
                ApplesQuantity = 0
            };

            // Eligible discounted bread units = floor(4 / 2) = 2 -> both breads discounted
            var soupTotal = PriceCalculationHelper.CalculateSubTotalCost(SoupPrice, 4);
            var expectedBreadTotal = PriceCalculationHelper.CalculateDiscountedCost(BreadPrice, 2, 50m);
            var expectedReceiptTotal = Math.Round(soupTotal + expectedBreadTotal, 2);

            // Act
            var created = await service.CreateReceiptAsync(createDto);

            // Assert
            Assert.NotNull(created);
            Assert.Equal(expectedReceiptTotal, created.TotalCost);

            var breadLine = created.ItemsOrdered.Single(io => io.ItemId == _breadItemId);
            Assert.NotNull(breadLine);
            Assert.True(breadLine.IsDiscounted);
            Assert.Equal(2, breadLine.Quantity);
            Assert.Equal(expectedBreadTotal, breadLine.TotalCost);
        }

        [Fact]
        public async Task Buy5SoupsAnd3Bread_TwoBreadDiscounted_OneFullPrice_TotalAndLinesCorrect()
        {
            // Arrange
            var items = new[]
            {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Description = "Soup", Price = SoupPrice },
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Description = "Bread", Price = BreadPrice }
            };
            var discounts = new[]
            {
                new Discount { DiscountId = 40, ItemId = _breadItemId, DiscountType = DiscountType.MultiBuy, Percentage = 50m, IsActive = true }
            };

            var service = CreateService(items, discounts, out var receiptRepo);

            var createDto = new ReceiptCreateDto
            {
                SoupQuantity = 5,
                BreadQuantity = 3,
                MilkQuantity = 0,
                ApplesQuantity = 0
            };

            // Eligible discounted bread units = floor(5 / 2) = 2 (only 2 breads discounted)
            var soupTotal = PriceCalculationHelper.CalculateSubTotalCost(SoupPrice, 5);
            var discountedBreadPart = PriceCalculationHelper.CalculateDiscountedCost(BreadPrice, 2, 50m);
            var remainingBreadPart = PriceCalculationHelper.CalculateSubTotalCost(BreadPrice, 1);
            var expectedBreadTotal = Math.Round(discountedBreadPart + remainingBreadPart, 2);
            var expectedReceiptTotal = Math.Round(soupTotal + expectedBreadTotal, 2);

            // Act
            var created = await service.CreateReceiptAsync(createDto);

            // Assert
            Assert.NotNull(created);
            Assert.Equal(expectedReceiptTotal, created.TotalCost);

            var breadLine = created.ItemsOrdered.Single(io => io.ItemId == _breadItemId);
            Assert.True(breadLine.IsDiscounted);
            Assert.Equal(3, breadLine.Quantity);
            Assert.Equal(expectedBreadTotal, breadLine.TotalCost);
        }

        [Fact]
        public async Task DiscountInactive_IsIgnored()
        {
            // Arrange: bread has multi-buy discount but it's inactive
            var items = new[]
            {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Price = SoupPrice },
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Price = BreadPrice }
            };
            var discounts = new[]
            {
                new Discount { DiscountId = 1, ItemId = _breadItemId, DiscountType = DiscountType.MultiBuy, Percentage = 50m, IsActive = false }
            };
            var service = CreateService(items, discounts, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 2, BreadQuantity = 1, MilkQuantity = 0, ApplesQuantity = 0 };

            // Act
            var created = await service.CreateReceiptAsync(dto);

            // Assert: bread should NOT be discounted because discount is inactive
            var breadLine = created.ItemsOrdered.Single(io => io.ItemId == _breadItemId);
            Assert.False(breadLine.IsDiscounted);
            var expectedTotal = PriceCalculationHelper.CalculateSubTotalCost(SoupPrice, 2) + PriceCalculationHelper.CalculateSubTotalCost(BreadPrice, 1);
            Assert.Equal(Math.Round(expectedTotal, 2), created.TotalCost);
        }

        [Fact]
        public async Task PercentageDiscount_NullPercentage_Ignored()
        {
            // Arrange: apple has Percentage discount record but Percentage is null => treat as no discount
            var items = new[]
            {
                new Item { ItemId = _appleItemId, ItemType = ItemType.Apple, Price = ApplePrice }
            };
            var discounts = new[]
            {
                new Discount { DiscountId = 2, ItemId = _appleItemId, DiscountType = DiscountType.Percentage, Percentage = null, IsActive = true }
            };
            var service = CreateService(items, discounts, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 0, BreadQuantity = 0, MilkQuantity = 0, ApplesQuantity = 2 };

            // Act
            var created = await service.CreateReceiptAsync(dto);

            var expected = PriceCalculationHelper.CalculateSubTotalCost(ApplePrice, 2);
            Assert.Equal(expected, created.TotalCost);
        }

        [Fact]
        public async Task PercentageDiscount_100Percent_ItemFree()
        {
            // Arrange: apple discounted 100%
            var items = new[]
            {
                new Item { ItemId = _appleItemId, ItemType = ItemType.Apple, Price = ApplePrice }
            };
            var discounts = new[]
            {
                new Discount { DiscountId = 3, ItemId = _appleItemId, DiscountType = DiscountType.Percentage, Percentage = 100m, IsActive = true }
            };
            var service = CreateService(items, discounts, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 0, BreadQuantity = 0, MilkQuantity = 0, ApplesQuantity = 3 };

            // Act
            var created = await service.CreateReceiptAsync(dto);

            // Assert: all apples free
            var appleLine = created.ItemsOrdered.Single(io => io.ItemId == _appleItemId);
            Assert.True(appleLine.IsDiscounted);
            Assert.Equal(3, appleLine.Quantity);
            Assert.Equal(0.00m, appleLine.TotalCost);
            Assert.Equal(0.00m, created.TotalCost);
        }

        [Fact]
        public async Task MultiBuy_NoSoups_NoBreadDiscount()
        {
            // Arrange: multi-buy discount exists for bread but no soups ordered -> no discount
            var items = new[]
            {
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Price = BreadPrice },
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Price = SoupPrice }
            };
            var discounts = new[]
            {
                new Discount { DiscountId = 4, ItemId = _breadItemId, DiscountType = DiscountType.MultiBuy, Percentage = 50m, IsActive = true }
            };
            var service = CreateService(items, discounts, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 0, BreadQuantity = 2, MilkQuantity = 0, ApplesQuantity = 0 };

            // Act
            var created = await service.CreateReceiptAsync(dto);

            // Assert: bread lines not discounted
            var breadLine = created.ItemsOrdered.Single(io => io.ItemId == _breadItemId);
            Assert.False(breadLine.IsDiscounted);
            var expected = PriceCalculationHelper.CalculateSubTotalCost(BreadPrice, 2);
            Assert.Equal(expected, created.TotalCost);
        }

        [Fact]
        public async Task MultiBuy_InsufficientSoups_NoDiscount()
        {
            // Arrange: only 1 soup ordered, requires 2 -> no discount applies
            var items = new[]
            {
                new Item { ItemId = _soupItemId, ItemType = ItemType.Soup, Price = SoupPrice },
                new Item { ItemId = _breadItemId, ItemType = ItemType.Bread, Price = BreadPrice }
            };
            var discounts = new[]
            {
                new Discount { DiscountId = 5, ItemId = _breadItemId, DiscountType = DiscountType.MultiBuy, Percentage = 50m, IsActive = true }
            };
            var service = CreateService(items, discounts, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 1, BreadQuantity = 1, MilkQuantity = 0, ApplesQuantity = 0 };

            // Act
            var created = await service.CreateReceiptAsync(dto);

            // Assert: no discount applied
            var breadLine = created.ItemsOrdered.Single(io => io.ItemId == _breadItemId);
            Assert.False(breadLine.IsDiscounted);
            var expected = PriceCalculationHelper.CalculateSubTotalCost(SoupPrice, 1) + PriceCalculationHelper.CalculateSubTotalCost(BreadPrice, 1);
            Assert.Equal(Math.Round(expected, 2), created.TotalCost);
        }

        [Fact]
        public async Task Rounding_EdgeCase_LineAndReceiptTotalsMatch()
        {
            // Arrange: use price that creates repeating decimals; verify per-line rounding and receipt sum equality
            var oddPrice = 0.3333m;
            var items = new[]
            {
                new Item { ItemId = _appleItemId, ItemType = ItemType.Apple, Price = oddPrice }
            };
            var discounts = Array.Empty<Discount>();
            var service = CreateService(items, discounts, out _);

            var dto = new ReceiptCreateDto { SoupQuantity = 0, BreadQuantity = 0, MilkQuantity = 0, ApplesQuantity = 3 };

            // Act
            var created = await service.CreateReceiptAsync(dto);

            // Assert: sub total = round(0.3333 * 3) -> PriceCalculationHelper ensures rounding per-line
            var expectedLineTotal = PriceCalculationHelper.CalculateSubTotalCost(oddPrice, 3);
            Assert.Equal(expectedLineTotal, created.ItemsOrdered.Single().TotalCost);
            Assert.Equal(expectedLineTotal, created.TotalCost);
        }
    }
}