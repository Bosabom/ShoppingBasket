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

        //[Column("ítems")]
        //public List<Item> Items { get; set; } = new List<Item>();

        //[ForeignKey]
        [Column("item_ordered_ids")]
        public long[] ItemOrderedIds { get; set; } = Array.Empty<long>();

        [Column("sub_total_cost")]
        public decimal SubTotalCost { get; set; }

        [Column("total_discount")]
        public decimal? TotalDiscount { get; set; }

        [Column("total_cost")]
        public decimal TotalCost { get; set; }

        [Column("created_datetime")]
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    }
}
