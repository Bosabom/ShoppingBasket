using AutoMapper;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.MappingProfiles
{
    public class DiscountMappingProfile : Profile
    {
        public DiscountMappingProfile()
        {
            //add more mappings if needed
            CreateMap<Discount, DiscountDto>().ReverseMap();
        }
    }
}
