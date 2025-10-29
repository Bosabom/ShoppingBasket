using ShoppingBasket.Server.Enums;
using System.Collections.Generic;
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
        public string Description { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        // Navigation: the discount associated with this item
        public Discount? Discount { get; set; }

        // Navigation: all ordered lines that reference this item
        public virtual ICollection<ItemOrdered> ItemsOrdered { get; set; } = new HashSet<ItemOrdered>();
    }
}
