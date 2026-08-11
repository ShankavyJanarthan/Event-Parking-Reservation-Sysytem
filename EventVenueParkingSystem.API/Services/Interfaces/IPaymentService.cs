using EventParkingReservationSystem.API.DTOs.Payments;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> ProcessPaymentAsync(
            int bookingId,
            int customerId,
            bool isAdmin,
            ProcessPaymentDto dto);

        Task<PaymentResponseDto> GetByBookingIdAsync(
            int bookingId,
            int customerId,
            bool isAdmin);
    }
}