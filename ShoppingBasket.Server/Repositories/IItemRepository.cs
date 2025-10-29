using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.Repositories
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(long id);
    }
}
