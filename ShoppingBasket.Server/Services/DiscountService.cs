using Mapster;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Repositories;

namespace ShoppingBasket.Server.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private ILoggerFactory _loggerFactory;

        public DiscountService(IDiscountRepository discountRepository, ILoggerFactory loggerFactory)
        {
            _discountRepository = discountRepository;
            _loggerFactory = loggerFactory;
        }

        //TODO: add more methods for creating, updating, deleting discounts
        public async Task<IEnumerable<DiscountDto>> GetAllDiscountsAsync()
        {
            var discounts = await _discountRepository.GetAllAsync();
            return discounts.Adapt<IEnumerable<DiscountDto>>();
        }

        public async Task<DiscountDto> GetDiscountByIdAsync(long id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            return discount.Adapt<DiscountDto>();
        }

        public async Task<DiscountDto> GetDiscountByItemIdAsync(long itemId)
        {
            var discount = await _discountRepository.GetByItemIdAsync(itemId);
            return discount.Adapt<DiscountDto>();
        }
    }
}
