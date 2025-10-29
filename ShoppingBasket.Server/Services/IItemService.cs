using ShoppingBasket.Server.DataTransfer;

namespace ShoppingBasket.Server.Services
{
    public interface IItemService
    {
        Task<IEnumerable<ItemDto>> GetAllItemsAsync();
        Task<ItemDto> GetItemByIdAsync(long id);
    }
}
