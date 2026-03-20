using Microsoft.EntityFrameworkCore;
using RentAPlace.Models;

namespace RentAPlace.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Property> Properties { get; set; }

        public DbSet<PropertyImage> PropertyImages { get; set; }

        public DbSet<Feature> Features { get; set; }

        public DbSet<PropertyFeature> PropertyFeatures { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Property>()
                .Property(p => p.PricePerNight)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PropertyFeature>()
                .HasKey(pf => new { pf.PropertyId, pf.FeatureId });

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Renter)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.RenterId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}