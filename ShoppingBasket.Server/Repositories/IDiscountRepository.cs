using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.Repositories
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<Discount>> GetAllAsync();
        Task<Discount> GetByIdAsync(long id);
        Task<Discount> GetByItemIdAsync(long id);
    }
}
