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
            CreateMap<Receipt, ReceiptDto>().ReverseMap();
            CreateMap<Receipt, ReceiptShortDto>();
            //TODO: implement mapping for ReceiptCreateDto to Receipt
            //CreateMap<ReceiptCreateDto, Receipt>();
        }
    }
}
