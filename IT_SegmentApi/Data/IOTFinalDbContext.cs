using IT_SegmentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IT_SegmentApi.Data
{
    public class IOTFinalDbContext: DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public IOTFinalDbContext(DbContextOptions<IOTFinalDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Marabou MjölkChoklad", Description = "Chocolate Bar", Price = 29.99m, Stock = 320 },
                new Product { ProductId = 2, Name = "Marabou Schweizernöt", Description = "Chocolate Bar", Price = 29.99m, Stock = 200 },
                new Product { ProductId = 3, Name = "Marabou Apelsinkrokant", Description = "Chocolate Bar", Price = 27.99m, Stock = 160 },
                new Product { ProductId = 4, Name = "Marabou Mintkrokant", Description = "Chocolate Bar", Price = 31.99m, Stock = 88 },
                new Product { ProductId = 5, Name = "Marabou Frukt & Mandel", Description = "Chocolate Bar", Price = 34.99m, Stock = 206 },
                new Product { ProductId = 6, Name = "Marabou Helnöt", Description = "Chocolate Bar", Price = 29.99m, Stock = 185 },
                new Product { ProductId = 7, Name = "Marabou Salta Mandlar", Description = "Chocolate Bar", Price = 36.99m, Stock = 200 },
                new Product { ProductId = 8, Name = "Marabou Jordgubb", Description = "Chocolate Bar", Price = 27.99m, Stock = 107 },
                new Product { ProductId = 9, Name = "Marabou Gräddnougat", Description = "Chocolate Bar", Price = 32.99m, Stock = 60 },
                new Product { ProductId = 10, Name = "Marabou Fudge & Havssalt", Description = "Chocolate Bar", Price = 36.99m, Stock = 145 }
            );
        }
    }
}
