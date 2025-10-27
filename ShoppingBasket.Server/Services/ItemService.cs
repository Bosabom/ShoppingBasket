using AutoMapper;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.MappingProfiles;
using ShoppingBasket.Server.Models;
using ShoppingBasket.Server.Repositories;

namespace ShoppingBasket.Server.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private ILoggerFactory _loggerFactory;
        private readonly Mapper _mapper;
        private readonly MapperConfiguration _mapperConfig;

        public ItemService(IItemRepository itemRepository, ILoggerFactory loggerFactory)
        {
            _itemRepository = itemRepository;
            _loggerFactory = loggerFactory;
            _mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ItemMappingProfile());
            }, _loggerFactory);
            _mapper = new Mapper(_mapperConfig);
        }
        public async Task<IEnumerable<ItemDto>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(items);
        }

        public async Task<ItemDto> GetItemByIdAsync(long id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            return _mapper.Map<Item, ItemDto>(item);
        }
    }
}
