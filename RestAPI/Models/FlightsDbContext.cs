using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class FlightsDbContext : DbContext
    {
        public FlightsDbContext(DbContextOptions<FlightsDbContext> options)
            : base(options)    {}


        public DbSet<User> Users { get; set; }
        public DbSet<FlightInfo> Flights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<FlightInfo>()
                .HasKey(f => f.Id);
        }
    }
}
