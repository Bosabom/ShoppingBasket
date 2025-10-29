using ShoppingBasket.Server.DataTransfer;

namespace ShoppingBasket.Server.Services
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountDto>> GetAllDiscountsAsync();
        Task<DiscountDto> GetDiscountByIdAsync(long id);
        Task<DiscountDto> GetDiscountByItemIdAsync(long id);
    }
}
