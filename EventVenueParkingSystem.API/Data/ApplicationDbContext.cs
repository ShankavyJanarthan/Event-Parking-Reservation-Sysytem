using EventParkingReservationSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =====================================================
        // Tables
        // =====================================================
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Venue> Venues { get; set; }

        public DbSet<EventCategory> EventCategories { get; set; }

        public DbSet<Event> Events { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================================
            // Customer
            // =====================================================
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(x => x.CustomerId);

                entity.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(x => x.Phone)
                    .HasMaxLength(30);

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.Property(x => x.Role)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(x => x.Email)
                    .IsUnique();
            });


            // =====================================================
            // Venue
            // =====================================================
            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(x => x.VenueId);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Address)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.Capacity)
                    .IsRequired();

                entity.Property(x => x.IsAvailable)
                    .IsRequired();
            });


            // =====================================================
            // Event Category
            // =====================================================
            modelBuilder.Entity<EventCategory>(entity =>
            {
                entity.HasKey(x => x.EventCategoryId);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.Property(x => x.IsActive)
                    .IsRequired();

                // Category names should not duplicate
                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });


            // =====================================================
            // Event
            // =====================================================
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(x => x.EventId);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.StartDateTime)
                    .IsRequired();

                entity.Property(x => x.EndDateTime)
                    .IsRequired();

                entity.Property(x => x.Capacity)
                    .IsRequired();

                entity.Property(x => x.TicketPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(30);


                // =================================================
                // Event -> Venue
                // =================================================
                entity.HasOne(x => x.Venue)
                    .WithMany()
                    .HasForeignKey(x => x.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);


                // =================================================
                // Event -> EventCategory
                // =================================================
                entity.HasOne(x => x.EventCategory)
                    .WithMany()
                    .HasForeignKey(x => x.EventCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}