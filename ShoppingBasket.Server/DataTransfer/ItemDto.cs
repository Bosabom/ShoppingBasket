using ShoppingBasket.Server.Enums;

namespace ShoppingBasket.Server.DataTransfer
{
    public class ItemDto
    {
        public long ItemId { get; set; }

        public ItemType ItemType { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}
