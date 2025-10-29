using ShoppingBasket.Server.Enums;

namespace ShoppingBasket.Server.DataTransfer
{
    public class DiscountDto
    {
        public long DiscountId { get; set; }

        public long ItemId { get; set; }

        public string Name { get; set; }

        public DiscountType DiscountType { get; set; }

        public decimal? Percentage { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
