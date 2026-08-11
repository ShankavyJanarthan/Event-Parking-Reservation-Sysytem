using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(
            int bookingId);

        Task<Booking?> GetByBookingNumberAsync(
            string bookingNumber);

        Task<List<Booking>> GetByCustomerIdAsync(
            int customerId);

        Task<bool> BookingNumberExistsAsync(
            string bookingNumber);

        Task<bool> HasActiveBookingForSeatAsync(
            int seatId);

        Task<bool> HasActiveBookingForParkingSlotAsync(
            int parkingSlotId);

        Task<bool> HasActiveFutureBookingsForCustomerAsync(
            int customerId);

        Task<int> GetActiveBookedSeatCountForEventAsync(
            int eventId);

        Task<bool> HasAnyBookingForEventAsync(
            int eventId);

        Task<List<Booking>> GetExpiredPendingBookingsAsync(
            DateTime currentTime);

        Task AddAsync(
            Booking booking);

        Task UpdateAsync(
            Booking booking);

        Task SaveChangesAsync();
    }
}