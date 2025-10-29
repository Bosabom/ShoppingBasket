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
        [Column("sub_total_cost")]
        public decimal SubTotalCost { get; set; }

        [Column("is_discounted")]
        public bool IsDiscounted { get; set; }

        // optional link to the discount that affected this line (nullable)
        [Column("discount_id")]
        public long? DiscountId { get; set; }

        // per-unit discounted price after applying discounts (nullable)
        [Column("discounted_cost")]
        public decimal? DiscountedCost { get; set; }

        [Column("total_cost")]
        public decimal TotalCost { get; set; }

        // Navigation: the item associated with this orderedItem
        [ForeignKey(nameof(ItemId))]
        public virtual Item? Item { get; set; }

        // Navigation: the receipt associated with this orderedItem
        [ForeignKey(nameof(ReceiptId))]
        public virtual Receipt? Receipt { get; set; }

        // Navigation: the discount associated with this orderedItem
        [ForeignKey(nameof(DiscountId))]
        public virtual Discount? Discount { get; set; }
    }
}
