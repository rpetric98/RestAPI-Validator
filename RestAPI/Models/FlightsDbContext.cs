using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace RestAPI.Models
{

    public class FlightsDbContext : DbContext
    {
        public FlightsDbContext(DbContextOptions<FlightsDbContext> options) : base(options) { }

        public DbSet<FlightDetails> FlightDetails { get; set; } = null!;
        public DbSet<Leg> Legs { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;  // Add Users DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationship for FlightDetails -> Legs
            modelBuilder.Entity<FlightDetails>()
                .HasMany(fd => fd.Legs)
                .WithOne(l => l.FlightDetails)
                .HasForeignKey(l => l.FlightDetailsId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}

