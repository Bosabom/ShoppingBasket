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

        // Sequential receipt number generated from a database sequence
        [Column("receipt_number")]
        public long ReceiptNumber { get; set; }

        [Column("total_cost")]
        public decimal TotalCost { get; set; }

        [Column("created_datetime")]
        public DateTime CreatedDateTime { get; set; }

        // navigation to ordered items
        public virtual ICollection<ItemOrdered> ItemsOrdered { get; set; } = new List<ItemOrdered>();
    }
}
