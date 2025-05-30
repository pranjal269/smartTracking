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
            base.OnModelCreating(builder);

            // Configure UserEntry
            builder.Entity<UserEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.FullName).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Role).IsRequired();
            });

            // Configure Parcel
            builder.Entity<Parcel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(p => p.Sender)
                    .WithMany()
                    .HasForeignKey(p => p.SenderId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure HandlerLocation
            builder.Entity<HandlerLocation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(hl => hl.Handler)
                    .WithMany()
                    .HasForeignKey(hl => hl.HandlerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure HandoverLog
            builder.Entity<HandoverLog>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            // Configure HandlerAlert
            builder.Entity<HandlerAlert>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}