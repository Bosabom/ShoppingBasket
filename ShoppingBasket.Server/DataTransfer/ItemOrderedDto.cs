namespace ShoppingBasket.Server.DataTransfer
{
    public class ItemOrderedDto
    {
        public long ItemOrderedId { get; set; }

        public long ItemId { get; set; }

        public long ReceiptId { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotalCost { get; set; }

        public bool IsDiscounted { get; set; }

        public decimal? DiscountedCost { get; set; }

        public decimal TotalCost { get; set; }

        public string ItemDescription { get; set; } = string.Empty;
    }
}
