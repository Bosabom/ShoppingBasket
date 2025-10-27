using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Server.Data;
using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private ShoppingBasketDbContext _db;

        //TODO: add methods for itemOrdered if needed
        public ItemRepository(ShoppingBasketDbContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            return await _db.Items.AsNoTracking().ToListAsync();
        }

        public async Task<Item> GetByIdAsync(long id)
        {
            return await _db.Items.FindAsync(id);// may return null
        }
    }
}
