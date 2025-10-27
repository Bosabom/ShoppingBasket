using AutoMapper;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.MappingProfiles
{
    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile()
        {
            //add more mappings if needed
            CreateMap<Item, ItemDto>().ReverseMap();
            CreateMap<ItemOrdered, ItemOrderedDto>().ReverseMap();
        }
    }
}
