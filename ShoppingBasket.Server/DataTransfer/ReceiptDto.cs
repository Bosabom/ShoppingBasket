namespace ShoppingBasket.Server.DataTransfer
{
    public class ReceiptDto
    {
        public long ReceiptId { get; set; }

        public string ReceiptNumber { get; set; }

        public decimal TotalCost { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public IEnumerable<ItemOrderedDto> ItemsOrdered { get; set; } = new List<ItemOrderedDto>();
    }
}
