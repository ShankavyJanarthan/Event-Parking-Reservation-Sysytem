using EventParkingReservationSystem.API.DTOs.Bookings;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateAsync(
            int customerId,
            CreateBookingDto dto);

        Task<BookingResponseDto> GetByIdAsync(
            int bookingId,
            int customerId,
            bool isAdmin);

        Task<List<BookingResponseDto>> GetCustomerBookingsAsync(
            int customerId);

        Task<string> CancelAsync(
            int bookingId,
            int customerId,
            bool isAdmin);
    }
}