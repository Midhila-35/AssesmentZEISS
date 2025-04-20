using AssesmentZEISS.Model;
using Microsoft.EntityFrameworkCore;

namespace AssesmentZEISS.Context
{
    public class AppDbContext :DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductIdTracker> ProductIdTrackers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
