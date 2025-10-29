using ShoppingBasket.Server.Models;
using ShoppingBasket.Server.Repositories;

namespace ShoppingBasket.Server.Tests.Fakes
{
    //TODO: write tests for this fake repository (e.x. ItemServiceTests)
    internal class FakeItemRepository : IItemRepository
    {
        private readonly List<Item> _items;
        public FakeItemRepository(IEnumerable<Item> items) => _items = items.Select(i => Clone(i)).ToList();
        public Task<IEnumerable<Item>> GetAllAsync() => Task.FromResult(_items.AsEnumerable());

        public Task<Item> GetByIdAsync(long id) => Task.FromResult(_items.SingleOrDefault(i => i.ItemId == id));

        private static Item Clone(Item i) => new Item
        {
            ItemId = i.ItemId,
            ItemType = i.ItemType,
            Description = i.Description,
            Price = i.Price
        };
    }

    internal class FakeItemRepository_ReturnsNull : IItemRepository
    {
        public Task<IEnumerable<Item>> GetAllAsync() => Task.FromResult<IEnumerable<Item>>(null);
        public Task<Item> GetByIdAsync(long id) => Task.FromResult<Item>(null);
    }
}
