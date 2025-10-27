namespace ShoppingBasket.Server.DataTransfer
{
    public class ReceiptShortDto
    {
        public string ReceiptNumber { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
