using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Server.Data;
using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private ShoppingBasketDbContext _db;
        public ReceiptRepository(ShoppingBasketDbContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            return await _db.Receipts.AsNoTracking().ToListAsync();
        }

        // Retrieve a receipt by its ID, including related items ordered
        public async Task<Receipt> GetByIdAsync(long id)
        {
            return await _db.Receipts.FindAsync(id);// may be null
        }

        public async Task<Receipt> GetDetailedByIdAsync(long id)
        {
            var receipt = await _db.Receipts
                    .Include(r => r.ItemsOrdered)
                    .ThenInclude(io => io.Item)
                    .SingleOrDefaultAsync(r => r.ReceiptId == id);
            return receipt;// may be null
        }

        //TODO: implement CreateReceipt method
        public async Task<Receipt> CreateAsync(Receipt receipt)
        {
            throw new NotImplementedException();
        }
    }
}
