using Mapster;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Repositories;

namespace ShoppingBasket.Server.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private ILoggerFactory _loggerFactory;

        public ItemService(IItemRepository itemRepository, ILoggerFactory loggerFactory)
        {
            _itemRepository = itemRepository;
            _loggerFactory = loggerFactory;
        }

        //TODO: add more methods for creating, updating, deleting items
        public async Task<IEnumerable<ItemDto>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllAsync();
            if (items is null)
            {
                throw new BadRequestException("No items were found.");
            }
            return items.Adapt<IEnumerable<ItemDto>>();
        }

        public async Task<ItemDto> GetItemByIdAsync(long id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item is null)
            {
                throw new BadRequestException("No item was found.");
            }
            return item.Adapt<ItemDto>();
        }
    }
}
