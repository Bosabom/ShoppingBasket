using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingBasket.Server.Models
{
    [Table("item_ordered")]
    public class ItemOrdered
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("item_ordered_id")]
        public long ItemOrderedId { get; set; }

        // FK to Item
        [Column("item_id")]
        public long ItemId { get; set; }

        // FK to Receipt
        [Column("receipt_id")]
        public long ReceiptId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        // unit price stored on the ordered line
        [Column("price")]
        public decimal Price { get; set; }

        [Column("is_discounted")]
        public bool IsDiscounted { get; set; }

        [Column("discounted_price")]
        public decimal? DiscountedPrice { get; set; }

        [ForeignKey(nameof(ItemId))]
        public virtual Item? Item { get; set; }

        [ForeignKey(nameof(ReceiptId))]
        public virtual Receipt? Receipt { get; set; }
    }
}
