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

        // Define DbSets for each entity
        public DbSet<Item> Items { get; set; }

        public DbSet<ItemOrdered> ItemsOrdered { get; set; }

        public DbSet<Receipt> Receipts { get; set; }
        
        public DbSet<Discount> Discounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //TODO: change to one-to-many because one item can have multiple discounts over time??
            //add relation to link Discount and Item (one-to-one)
            modelBuilder.Entity<Discount>()
                .HasOne(d => d.Item)
                .WithOne(i => i.Discount)
                .HasForeignKey<Discount>(d => d.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            //add relations for ItemOrdered to Item (one-to-many)
            modelBuilder.Entity<ItemOrdered>()
                .HasOne(io => io.Item)
                .WithMany(i => i.ItemsOrdered)
                .HasForeignKey(io => io.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            //add relations for ItemOrdered to Receipt (one-to-many)
            //When a receipt is removed, delete items ordered as well
            modelBuilder.Entity<ItemOrdered>()
                .HasOne(io => io.Receipt)
                .WithMany(r => r.ItemsOrdered)
                .HasForeignKey(io => io.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add relation for ItemOrdered to Discount (one-to-many, optional)
            // When a discount is removed, keep history by setting FK to null
            modelBuilder.Entity<ItemOrdered>()
                .HasOne(io => io.Discount)
                .WithMany()
                .HasForeignKey(io => io.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure ReceiptNumber to use sequental generation
            modelBuilder.HasSequence<long>("receipt_number_seq");

            modelBuilder.Entity<Receipt>()
                .Property(r => r.ReceiptNumber)
                .HasDefaultValueSql("nextval('receipt_number_seq')");

            // Seed data for items
            modelBuilder.Entity<Item>().HasData(
                new Item { ItemId = 1, ItemType = ItemType.Soup, Description = "Tomato Soup (400g)", Price = 0.65m },
                new Item { ItemId = 2, ItemType = ItemType.Bread, Description = "Wholemeal Bread (800g)", Price = 0.80m },
                new Item { ItemId = 3, ItemType = ItemType.Milk, Description = "Semi-skimmed Milk (1L)", Price = 1.30m },
                new Item { ItemId = 4, ItemType = ItemType.Apple, Description = "Apples bag", Price = 1 }
            );

            // Seed data for discounts
            modelBuilder.Entity<Discount>().HasData(
                // Apples 10% off
                new Discount
                {
                    DiscountId = 1,
                    Name = "Apples 10% off",
                    DiscountType = DiscountType.Percentage,
                    ItemId = 4,
                    Percentage = 10m,
                    IsActive = true
                },
                // Buy 2 soups get 1 bread at 50% off
                new Discount
                {
                    DiscountId = 2,
                    Name = "Buy 2 soups get bread half price",
                    DiscountType = DiscountType.MultiBuy,
                    ItemId = 2,
                    Percentage = 50m,
                    IsActive = true
                }
            );
        }
    }
}