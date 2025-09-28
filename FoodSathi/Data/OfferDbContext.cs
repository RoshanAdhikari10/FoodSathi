using Microsoft.EntityFrameworkCore;

public class OfferDbContext : DbContext
{
    public OfferDbContext(DbContextOptions<OfferDbContext> options)
        : base(options)
    {
    }

    public DbSet<Offer> Offers { get; set; }
}
