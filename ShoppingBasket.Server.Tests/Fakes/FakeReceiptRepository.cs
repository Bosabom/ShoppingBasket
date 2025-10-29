using ShoppingBasket.Server.Models;
using ShoppingBasket.Server.Repositories;

namespace ShoppingBasket.Server.Tests.Fakes
{
    internal class FakeReceiptRepository : IReceiptRepository
    {
        private long _nextId = 200;
        private readonly List<Receipt> _receipts;
        public Task<IEnumerable<Receipt>> GetAllAsync() => Task.FromResult(_receipts.AsEnumerable());
        public Task<Receipt> GetByIdAsync(long id) => Task.FromResult(_receipts.SingleOrDefault(r => r.ReceiptId == id));
        public Task<Receipt> GetDetailedByIdAsync(long id) => Task.FromResult(_receipts.SingleOrDefault(r => r.ReceiptId == id));

        public Task<Receipt> CreateAsync(Receipt receipt)
        {
            receipt.ReceiptId = _nextId++;
            receipt.ReceiptNumber = receipt.ReceiptId;
            return Task.FromResult(receipt);
        }
    }

    internal class FakeReceiptRepository_ReturnsNull : IReceiptRepository
    {
        public Task<IEnumerable<Receipt>> GetAllAsync() => Task.FromResult<IEnumerable<Receipt>>(null);
        public Task<Receipt> GetByIdAsync(long id) => Task.FromResult<Receipt>(null);
        public Task<Receipt> GetDetailedByIdAsync(long id) => Task.FromResult<Receipt>(null);
        public Task<Receipt> CreateAsync(Receipt receipt) => Task.FromResult<Receipt>(null);
    }
}
