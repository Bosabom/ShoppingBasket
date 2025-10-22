namespace ShoppingBasket.Server.DataTransfer
{
    public class ItemOrderedDto
    {
        public long ItemOrderedId { get; set; }

        public long ItemId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public bool IsDiscounted { get; set; }

        public decimal? DiscountedPrice { get; set; }
    }
}
