using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Get Booking By Id
        // =====================================================
        public async Task<Booking?> GetByIdAsync(
            int bookingId)
        {
            return await _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Event)
                .Include(x => x.Seat)
                .Include(x => x.ParkingSlot)
                .FirstOrDefaultAsync(
                    x => x.BookingId == bookingId);
        }

        // =====================================================
        // Get Booking By Booking Number
        // =====================================================
        public async Task<Booking?> GetByBookingNumberAsync(
            string bookingNumber)
        {
            return await _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Event)
                .Include(x => x.Seat)
                .Include(x => x.ParkingSlot)
                .FirstOrDefaultAsync(
                    x => x.BookingNumber == bookingNumber);
        }

        // =====================================================
        // Get Customer Booking History
        // =====================================================
        public async Task<List<Booking>> GetByCustomerIdAsync(
            int customerId)
        {
            return await _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Event)
                .Include(x => x.Seat)
                .Include(x => x.ParkingSlot)
                .Where(x =>
                    x.CustomerId == customerId)
                .OrderByDescending(x =>
                    x.CreatedAt)
                .ToListAsync();
        }

        // =====================================================
        // Check Booking Number Exists
        // =====================================================
        public async Task<bool> BookingNumberExistsAsync(
            string bookingNumber)
        {
            return await _context.Bookings
                .AnyAsync(x =>
                    x.BookingNumber == bookingNumber);
        }

        // =====================================================
        // Check Seat Active Booking
        // =====================================================
        public async Task<bool> HasActiveBookingForSeatAsync(
            int seatId)
        {
            var currentTime =
                DateTime.UtcNow;

            return await _context.Bookings
                .AnyAsync(x =>
                    x.SeatId == seatId &&
                    (
                        x.Status == "Confirmed" ||
                        (
                            x.Status == "Pending" &&
                            x.HoldExpiresAt > currentTime
                        )
                    ));
        }

        // =====================================================
        // Check Parking Slot Active Booking
        // =====================================================
        public async Task<bool> HasActiveBookingForParkingSlotAsync(
            int parkingSlotId)
        {
            var currentTime =
                DateTime.UtcNow;

            return await _context.Bookings
                .AnyAsync(x =>
                    x.ParkingSlotId == parkingSlotId &&
                    (
                        x.Status == "Confirmed" ||
                        (
                            x.Status == "Pending" &&
                            x.HoldExpiresAt > currentTime
                        )
                    ));
        }

        // =====================================================
        // Customer Active Future Booking Validation
        // =====================================================
        public async Task<bool> HasActiveFutureBookingsForCustomerAsync(
            int customerId)
        {
            var currentTime =
                DateTime.UtcNow;

            return await _context.Bookings
                .AnyAsync(x =>
                    x.CustomerId == customerId &&
                    x.Event != null &&
                    x.Event.StartDateTime > currentTime &&
                    (
                        x.Status == "Confirmed" ||
                        (
                            x.Status == "Pending" &&
                            x.HoldExpiresAt > currentTime
                        )
                    ));
        }

        // =====================================================
        // Get Active Reserved / Booked Seat Count For Event
        // =====================================================
        public async Task<int> GetActiveBookedSeatCountForEventAsync(
            int eventId)
        {
            var currentTime =
                DateTime.UtcNow;

            return await _context.Bookings
                .CountAsync(x =>
                    x.EventId == eventId &&
                    (
                        x.Status == "Confirmed" ||
                        (
                            x.Status == "Pending" &&
                            x.HoldExpiresAt > currentTime
                        )
                    ));
        }

        // =====================================================
        // Check Whether Event Has Any Booking History
        // Pending / Confirmed / Cancelled / Expired
        // =====================================================
        public async Task<bool> HasAnyBookingForEventAsync(
            int eventId)
        {
            return await _context.Bookings
                .AnyAsync(x =>
                    x.EventId == eventId);
        }

        // =====================================================
        // Get Expired Pending Bookings
        // =====================================================
        public async Task<List<Booking>> GetExpiredPendingBookingsAsync(
            DateTime currentTime)
        {
            return await _context.Bookings
                .Include(x => x.Customer)
                .Include(x => x.Event)
                .Include(x => x.Seat)
                .Include(x => x.ParkingSlot)
                .Where(x =>
                    x.Status == "Pending" &&
                    x.HoldExpiresAt <= currentTime)
                .ToListAsync();
        }

        // =====================================================
        // Add Booking
        // =====================================================
        public async Task AddAsync(
            Booking booking)
        {
            await _context.Bookings
                .AddAsync(booking);
        }

        // =====================================================
        // Update Booking
        // =====================================================
        public Task UpdateAsync(
            Booking booking)
        {
            _context.Bookings.Update(
                booking);

            return Task.CompletedTask;
        }

        // =====================================================
        // Save Changes
        // =====================================================
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}