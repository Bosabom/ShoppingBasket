using ShoppingBasket.Server.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingBasket.Server.Models
{
    [Table("item")]
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("item_id")]
        public long ItemId { get; set; }

        [Column("item_type")]
        public ItemType ItemType { get; set; }

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("price")]
        public decimal Price { get; set; }
    }
}
