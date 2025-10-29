using ShoppingBasket.Server.Models;
using ShoppingBasket.Server.Repositories;

namespace ShoppingBasket.Server.Tests.Fakes
{
    //TODO: write tests for this fake repository (e.x. DiscountServiceTests)
    internal class FakeDiscountRepository : IDiscountRepository
    {
        private readonly List<Discount> _discounts;
        public FakeDiscountRepository(IEnumerable<Discount> discounts) => _discounts = discounts.Select(Clone).ToList();
        public Task<IEnumerable<Discount>> GetAllAsync() => Task.FromResult(_discounts.AsEnumerable());
        public Task<Discount> GetByIdAsync(long id) => Task.FromResult(_discounts.SingleOrDefault(d => d.DiscountId == id));
        public Task<Discount> GetByItemIdAsync(long id) => Task.FromResult(_discounts.SingleOrDefault(d => d.ItemId == id));

        private static Discount Clone(Discount d) => new Discount
        {
            DiscountId = d.DiscountId,
            ItemId = d.ItemId,
            Name = d.Name,
            DiscountType = d.DiscountType,
            Percentage = d.Percentage,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            IsActive = d.IsActive
        };

        internal class FakeDiscountRepository_ReturnsNull : IDiscountRepository
        {
            public Task<IEnumerable<Discount>> GetAllAsync() => Task.FromResult<IEnumerable<Discount>>(null);
            public Task<Discount> GetByIdAsync(long id) => Task.FromResult<Discount>(null);
            public Task<Discount> GetByItemIdAsync(long id) => Task.FromResult<Discount>(null);
        }
    }
}
