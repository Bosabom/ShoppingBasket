namespace ShoppingBasket.Server.DataTransfer
{
    public class ReceiptShortDto
    {
        public long ReceiptId { get; set; }

        public string ReceiptNumber { get; set; }

        public decimal TotalCost { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
