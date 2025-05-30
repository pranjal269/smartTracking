using Microsoft.EntityFrameworkCore;
using SmartTracking.Api.Models;

namespace SmartTracking.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        // Core functionality
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<HandoverLog> HandoverLogs { get; set; }
        public DbSet<HandlerAlert> HandlerAlerts { get; set; }
        public DbSet<HandlerLocation> HandlerLocations { get; set; }
        public DbSet<UserEntry> UserEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Configure relationships
            builder.Entity<Parcel>()
                .HasOne<UserEntry>()
                .WithMany()
                .HasForeignKey(p => p.SenderId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<HandlerLocation>()
                .HasOne<UserEntry>()
                .WithMany()
                .HasForeignKey(hl => hl.HandlerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}