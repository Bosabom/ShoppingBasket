using AutoMapper;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.MappingProfiles;
using ShoppingBasket.Server.Models;
using ShoppingBasket.Server.Repositories;

namespace ShoppingBasket.Server.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private ILoggerFactory _loggerFactory;
        private readonly Mapper _mapper;
        private readonly MapperConfiguration _mapperConfig;

        public DiscountService(IDiscountRepository discountRepository, ILoggerFactory loggerFactory)
        {
            _discountRepository = discountRepository;
            _loggerFactory = loggerFactory;
            _mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DiscountMappingProfile());
            }, _loggerFactory);
            _mapper = new Mapper(_mapperConfig);
        }

        //TODO: add more methods for creating, updating, deleting discounts
        public async Task<IEnumerable<DiscountDto>> GetAllDiscountsAsync()
        {
           var discounts = await _discountRepository.GetAllAsync();
           return _mapper.Map<IEnumerable<Discount>, IEnumerable<DiscountDto>>(discounts);
        }

        public async Task<DiscountDto> GetDiscountByIdAsync(long id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            return _mapper.Map<Discount, DiscountDto>(discount);
        }

        public async Task<DiscountDto> GetDiscountByItemIdAsync(long itemId)
        {
            var discount = await _discountRepository.GetByItemIdAsync(itemId);
            return _mapper.Map<Discount, DiscountDto>(discount);
        }
    }
}
