using ShoppingBasket.Server.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingBasket.Server.Models
{
    [Table("discount")]
    public class Discount
    {
        //TODO: add more properties?
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("discount_id")]
        public long DiscountId { get; set; }

        [Column("item_id")]
        public long ItemId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("discount_type")]
        public DiscountType DiscountType { get; set; }

        [Column("percentage")]
        public decimal? Percentage { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation: the item associated with this discount
        [ForeignKey(nameof(ItemId))]
        public virtual Item? Item { get; set; }
    }
}
