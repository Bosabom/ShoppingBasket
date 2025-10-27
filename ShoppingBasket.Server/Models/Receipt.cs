using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingBasket.Server.Models
{
    [Table("receipt")]
    public class Receipt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("receipt_id")]
        public long ReceiptId { get; set; }

        [Column("receipt_number")]
        public string ReceiptNumber { get; set; }

        [Column("sub_total_cost")]
        public decimal SubTotalCost { get; set; }

        [Column("total_discount")]
        public decimal? TotalDiscount { get; set; }

        [Column("total_cost")]
        public decimal TotalCost { get; set; }

        [Column("created_datetime")]
        public DateTime CreatedDateTime { get; set; }

        // navigation to ordered items
        public virtual ICollection<ItemOrdered> ItemsOrdered { get; set; } = new List<ItemOrdered>();
    }
}
