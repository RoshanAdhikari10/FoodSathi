using FoodSathi.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSathi.Models
{
    public class MenuDbContext : DbContext
    {
        public MenuDbContext(DbContextOptions<MenuDbContext> options)
            : base(options) { }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Cart> Carts { get; set; }   // 👈 Added for Cart

        public DbSet<Order> Orders { get; set; }   // 👈 Added for Cart

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Specify precision for Price to avoid truncation warnings
            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasColumnType("decimal(18,2)");
        }

    }

}
