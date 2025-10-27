using Microsoft.EntityFrameworkCore;
using ShoppingBasket.Server.Enums;
using ShoppingBasket.Server.Models;

namespace ShoppingBasket.Server.Data
{
    public class ShoppingBasketDbContext : DbContext
    {
        public ShoppingBasketDbContext(DbContextOptions<ShoppingBasketDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<ItemOrdered> ItemsOrdered { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

        //TODO: Implement Discount entity and DbSet

        //public DbSet<Discount> Discounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemOrdered>()
                .HasOne(io => io.Item)
                .WithMany(i => i.ItemsOrdered)
                .HasForeignKey(io => io.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemOrdered>()
                .HasOne(io => io.Receipt)
                .WithMany(r => r.ItemsOrdered)
                .HasForeignKey(io => io.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data for Items
            modelBuilder.Entity<Item>().HasData(
                new Item { ItemId = 1, ItemType = ItemType.Soup, Description = "Tomato Soup (400g)", Price = 0.65m },
                new Item { ItemId = 2, ItemType = ItemType.Bread, Description = "Wholemeal Bread (800g)", Price = 0.80m },
                new Item { ItemId = 3, ItemType = ItemType.Milk, Description = "Semi-skimmed Milk (1L)", Price = 1.30m },
                new Item { ItemId = 4, ItemType = ItemType.Apple, Description = "Apples bag", Price = 1 }
            );
        }
    }
}