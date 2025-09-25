using Microsoft.EntityFrameworkCore;

namespace FoodSathi.Models
{
    public class OfferDbContext : DbContext
    {
        public OfferDbContext(DbContextOptions<OfferDbContext> options) : base(options) { }

        public DbSet<Offer> Offers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>()
                .Property(o => o.DiscountPercentage)
                .HasPrecision(5, 2); // allows up to 999.99
        }

    }
}
