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

        [Column("item_id")]
        //[ForeignKey]
        public long ItemId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("is_discounted")]
        public bool IsDiscounted { get; set; }

        [Column("discounted_price")]
        public decimal? DiscountedPrice { get; set; }
    }
}
