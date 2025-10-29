using AutoMapper;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.MappingProfiles
{
    public class ReceiptMappingProfile : Profile
    {
        public ReceiptMappingProfile()
        {
            //add more mappings if needed
            CreateMap<Receipt, ReceiptDto>()
                .ForMember(dest => dest.ItemsOrdered,
                           opt => opt.MapFrom(src => src.ItemsOrdered))
                .ForMember(dest => dest.ReceiptNumber,
                           opt => opt.MapFrom(src => src.ReceiptNumber.ToString("D8")));

            CreateMap<Receipt, ReceiptShortDto>()
                .ForMember(dest => dest.ReceiptNumber,
                           opt => opt.MapFrom(src => src.ReceiptNumber.ToString("D8")));

            CreateMap<ItemOrdered, ItemOrderedDto>()
                .ForMember(dest => dest.ItemDescription,
                           opt => opt.MapFrom(src => src.Item != null ? src.Item.Description : string.Empty));
        }
    }
}
