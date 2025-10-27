using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.Repositories
{
    public interface IItemRepository
    {
        //TODO: add methods for itemOrdered if needed
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(long id);
    }
}
