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
        // Member 1
        // =====================================================
        public DbSet<Customer> Customers { get; set; }

        // =====================================================
        // Member 2
        // =====================================================
        public DbSet<Venue> Venues { get; set; }

        public DbSet<EventCategory> EventCategories { get; set; }

        public DbSet<Event> Events { get; set; }

        // =====================================================
        // Member 3
        // =====================================================
        public DbSet<Seat> Seats { get; set; }

        public DbSet<ParkingSlot> ParkingSlots { get; set; }

        // =====================================================
        // Member 4
        // =====================================================
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =================================================
            // Customer
            // =================================================
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
                    .HasMaxLength(200);

                entity.Property(x => x.Phone)
                    .HasMaxLength(30);

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.Property(x => x.Role)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasIndex(x => x.Email)
                    .IsUnique();
            });


            // =================================================
            // Venue
            // =================================================
            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(x => x.VenueId);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Address)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.Capacity)
                    .IsRequired();
            });


            // =================================================
            // Event Category
            // =================================================
            modelBuilder.Entity<EventCategory>(entity =>
            {
                entity.HasKey(x => x.EventCategoryId);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });


            // =================================================
            // Event
            // =================================================
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(x => x.EventId);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.Capacity)
                    .IsRequired();

                entity.Property(x => x.TicketPrice)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(x => x.Venue)
                    .WithMany()
                    .HasForeignKey(x => x.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.EventCategory)
                    .WithMany()
                    .HasForeignKey(x => x.EventCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =================================================
            // Seat
            // =================================================
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(x => x.SeatId);

                entity.Property(x => x.RowLabel)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.SeatNumber)
                    .IsRequired();

                entity.Property(x => x.SeatLabel)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(x => x.Event)
                    .WithMany()
                    .HasForeignKey(x => x.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Same row + seat number cannot duplicate
                entity.HasIndex(x => new
                {
                    x.EventId,
                    x.RowLabel,
                    x.SeatNumber
                })
                .IsUnique();

                // Seat label must also be unique per event
                entity.HasIndex(x => new
                {
                    x.EventId,
                    x.SeatLabel
                })
                .IsUnique();
            });


            // =================================================
            // Parking Slot
            // =================================================
            modelBuilder.Entity<ParkingSlot>(entity =>
            {
                entity.HasKey(x => x.ParkingSlotId);

                entity.Property(x => x.SlotNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Fee)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(x => x.Event)
                    .WithMany()
                    .HasForeignKey(x => x.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new
                {
                    x.EventId,
                    x.SlotNumber
                })
                .IsUnique();
            });


            // =================================================
            // Booking
            // =================================================
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(x => x.BookingId);

                entity.Property(x => x.BookingNumber)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(x => x.SeatPrice)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.ParkingFee)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.TotalAmount)
                    .HasColumnType("decimal(18,2)");

                // Booking number must always be unique
                entity.HasIndex(x => x.BookingNumber)
                    .IsUnique();


                // ---------------------------------------------
                // Customer
                // ---------------------------------------------
                entity.HasOne(x => x.Customer)
                    .WithMany()
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);


                // ---------------------------------------------
                // Event
                // ---------------------------------------------
                entity.HasOne(x => x.Event)
                    .WithMany()
                    .HasForeignKey(x => x.EventId)
                    .OnDelete(DeleteBehavior.Restrict);


                // ---------------------------------------------
                // Seat
                // ---------------------------------------------
                entity.HasOne(x => x.Seat)
                    .WithMany()
                    .HasForeignKey(x => x.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);


                // ---------------------------------------------
                // Optional Parking
                // ---------------------------------------------
                entity.HasOne(x => x.ParkingSlot)
                    .WithMany()
                    .HasForeignKey(x => x.ParkingSlotId)
                    .OnDelete(DeleteBehavior.Restrict);


                // ---------------------------------------------
                // Prevent active double booking of a Seat
                //
                // Cancelled / Expired booking will not block
                // the seat from being booked again.
                // ---------------------------------------------
                entity.HasIndex(x => x.SeatId)
                    .IsUnique()
                    .HasFilter(
                        "[Status] IN ('Pending', 'Confirmed')");


                // ---------------------------------------------
                // Prevent active double booking of Parking
                //
                // Null parking is allowed.
                // Cancelled / Expired booking will not block
                // the parking slot from being reused.
                // ---------------------------------------------
                entity.HasIndex(x => x.ParkingSlotId)
                    .IsUnique()
                    .HasFilter(
                        "[ParkingSlotId] IS NOT NULL AND [Status] IN ('Pending', 'Confirmed')");
            });


            // =================================================
            // Payment
            // =================================================
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(x => x.PaymentId);

                entity.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(x => x.TransactionReference)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(x => x.Booking)
                    .WithMany()
                    .HasForeignKey(x => x.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Only one payment record for a booking
                entity.HasIndex(x => x.BookingId)
                    .IsUnique();

                // Payment reference must be unique
                entity.HasIndex(x => x.TransactionReference)
                    .IsUnique();
            });


            // =================================================
            // Notification
            // =================================================
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(x => x.NotificationId);

                entity.Property(x => x.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Message)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(x => x.Customer)
                    .WithMany()
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Booking)
                    .WithMany()
                    .HasForeignKey(x => x.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}