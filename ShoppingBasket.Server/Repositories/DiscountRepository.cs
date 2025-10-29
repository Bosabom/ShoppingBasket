using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Server.Data;
using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private ShoppingBasketDbContext _db;

        //TODO: add discount creation, update, delete methods if needed
        public DiscountRepository(ShoppingBasketDbContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<Discount>> GetAllAsync()
        {
            return await _db.Discounts.ToListAsync();
        }

        public async Task<Discount> GetByIdAsync(long id)
        {
            return await _db.Discounts.FindAsync(id);// may return null
        }

        public async Task<Discount> GetByItemIdAsync(long itemId)
        {
            return await _db.Discounts.FirstOrDefaultAsync(d => d.ItemId == itemId); // may return null
        }
    }
}
