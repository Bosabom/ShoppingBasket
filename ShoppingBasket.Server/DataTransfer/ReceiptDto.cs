namespace ShoppingBasket.Server.DataTransfer
{
    public class ReceiptDto
    {
        public long ReceiptId { get; set; }
        //FIXME: Change to proper receipt number generation
        public string ReceiptNumber { get; set; } = "0000001";
        public long[] ItemOrderedIds { get; set; } = Array.Empty<long>();
        public decimal SubTotalCost { get; set; }
        public decimal? TotalDiscount { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    }
}
