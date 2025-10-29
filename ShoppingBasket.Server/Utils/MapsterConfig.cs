using Mapster;
using ShoppingBasket.Server.DataTransfer;
using ShoppingBasket.Server.Models;
using System.Reflection;

namespace ShoppingBasket.Server.Utils
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            //add more mappings if needed
            TypeAdapterConfig<Item, ItemDto>
                .NewConfig();

            TypeAdapterConfig<ItemOrdered, ItemOrderedDto>
                .NewConfig()
                .Map(dest => dest.ItemDescription, 
                     src => src.Item != null ? src.Item.Description : string.Empty);

            TypeAdapterConfig<Discount, DiscountDto>
                .NewConfig();

            TypeAdapterConfig<Receipt, ReceiptDto>.NewConfig()
                .Map(dest => dest.ReceiptNumber,
                     src => src.ReceiptNumber.ToString("D8"))
                .Map(dest => dest.ItemsOrdered,
                     src => src.ItemsOrdered);

            TypeAdapterConfig<Receipt, ReceiptShortDto>.NewConfig()
                .Map(dest => dest.ReceiptNumber,
                     src => src.ReceiptNumber.ToString("D8"));

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
