using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.Repositories
{
    public interface IReceiptRepository
    {
        Task<IEnumerable<Receipt>> GetAllAsync();
        Task<Receipt> GetByIdAsync(long id);
        Task<Receipt> GetDetailedByIdAsync(long id);
        Task<Receipt> CreateAsync(Receipt receipt);
    }
}
