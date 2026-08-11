using System.Security.Cryptography;
using EventParkingReservationSystem.API.DTOs.Payments;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly INotificationService _notificationService;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            INotificationService notificationService)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _notificationService = notificationService;
        }

        // =====================================================
        // Process Payment
        // =====================================================
        public async Task<PaymentResponseDto> ProcessPaymentAsync(
            int bookingId,
            int customerId,
            bool isAdmin,
            ProcessPaymentDto dto)
        {
            var booking =
                await _bookingRepository.GetByIdAsync(
                    bookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException(
                    "Booking was not found.");
            }

            // =================================================
            // Authorization
            // =================================================
            if (!isAdmin &&
                booking.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to process payment for this booking.");
            }

            // =================================================
            // Booking Status Validation
            // =================================================
            if (booking.Status == "Cancelled")
            {
                throw new InvalidOperationException(
                    "Payment cannot be processed for a cancelled booking.");
            }

            if (booking.Status == "Expired")
            {
                throw new InvalidOperationException(
                    "Payment cannot be processed for an expired booking.");
            }

            if (booking.Status == "Confirmed")
            {
                throw new InvalidOperationException(
                    "This booking has already been confirmed.");
            }

            if (booking.Status != "Pending")
            {
                throw new InvalidOperationException(
                    "Payment can only be processed for a pending booking.");
            }

            // =================================================
            // Hold Expiry Validation
            // =================================================
            if (booking.HoldExpiresAt <= DateTime.UtcNow)
            {
                booking.Status =
                    "Expired";

                booking.UpdatedAt =
                    DateTime.UtcNow;

                await _bookingRepository.UpdateAsync(
                    booking);

                await _bookingRepository.SaveChangesAsync();

                throw new InvalidOperationException(
                    "The booking hold has expired. Please create a new booking.");
            }

            // =================================================
            // Prevent Duplicate Payment
            // =================================================
            var existingPayment =
                await _paymentRepository
                    .GetByBookingIdAsync(
                        bookingId);

            if (existingPayment != null)
            {
                throw new InvalidOperationException(
                    "A payment already exists for this booking.");
            }

            var alreadyPaid =
                await _paymentRepository
                    .HasSuccessfulPaymentAsync(
                        bookingId);

            if (alreadyPaid)
            {
                throw new InvalidOperationException(
                    "This booking has already been paid.");
            }

            // =================================================
            // Payment Method Validation
            // =================================================
            if (string.IsNullOrWhiteSpace(
                    dto.PaymentMethod))
            {
                throw new InvalidOperationException(
                    "Payment method is required.");
            }

            // =================================================
            // Generate Transaction Reference
            // =================================================
            var transactionReference =
                await GenerateTransactionReferenceAsync();

            var currentTime =
                DateTime.UtcNow;

            // =================================================
            // Simulated Successful Payment
            // =================================================
            var payment = new Payment
            {
                BookingId =
                    booking.BookingId,

                Amount =
                    booking.TotalAmount,

                PaymentMethod =
                    dto.PaymentMethod.Trim(),

                Status =
                    "Successful",

                TransactionReference =
                    transactionReference,

                PaidAt =
                    currentTime,

                CreatedAt =
                    currentTime
            };

            try
            {
                await _paymentRepository.AddAsync(
                    payment);

                await _paymentRepository
                    .SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException(
                    "Payment could not be completed. A payment may already exist for this booking.");
            }

            // =================================================
            // Confirm Booking
            // =================================================
            booking.Status =
                "Confirmed";

            booking.ConfirmedAt =
                currentTime;

            booking.UpdatedAt =
                currentTime;

            await _bookingRepository.UpdateAsync(
                booking);

            await _bookingRepository
                .SaveChangesAsync();

            // =================================================
            // Payment Successful Notification
            // =================================================
            await _notificationService.CreateAsync(
                booking.CustomerId,
                booking.BookingId,
                "PaymentSuccessful",
                "Payment Successful",
                $"Your payment of {booking.TotalAmount:F2} for booking {booking.BookingNumber} was successful.");

            // =================================================
            // Booking Confirmed Notification
            // =================================================
            await _notificationService.CreateAsync(
                booking.CustomerId,
                booking.BookingId,
                "BookingConfirmed",
                "Booking Confirmed",
                $"Your booking {booking.BookingNumber} has been confirmed successfully.");

            // =================================================
            // Return Payment
            // =================================================
            var createdPayment =
                await _paymentRepository.GetByIdAsync(
                    payment.PaymentId);

            if (createdPayment == null)
            {
                throw new InvalidOperationException(
                    "Payment was processed but could not be retrieved.");
            }

            return MapToResponse(
                createdPayment);
        }

        // =====================================================
        // Get Payment By Booking
        // =====================================================
        public async Task<PaymentResponseDto> GetByBookingIdAsync(
            int bookingId,
            int customerId,
            bool isAdmin)
        {
            var booking =
                await _bookingRepository.GetByIdAsync(
                    bookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException(
                    "Booking was not found.");
            }

            if (!isAdmin &&
                booking.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to view this payment.");
            }

            var payment =
                await _paymentRepository
                    .GetByBookingIdAsync(
                        bookingId);

            if (payment == null)
            {
                throw new KeyNotFoundException(
                    "Payment was not found for this booking.");
            }

            return MapToResponse(
                payment);
        }

        // =====================================================
        // Generate Transaction Reference
        // PAY-YYYY-XXXXXX
        // =====================================================
        private async Task<string>
            GenerateTransactionReferenceAsync()
        {
            for (var attempt = 0;
                 attempt < 10;
                 attempt++)
            {
                var randomNumber =
                    RandomNumberGenerator.GetInt32(
                        0,
                        1_000_000);

                var reference =
                    $"PAY-{DateTime.UtcNow.Year}-{randomNumber:D6}";

                var exists =
                    await _paymentRepository
                        .TransactionReferenceExistsAsync(
                            reference);

                if (!exists)
                {
                    return reference;
                }
            }

            throw new InvalidOperationException(
                "Unable to generate a unique payment reference. Please try again.");
        }

        // =====================================================
        // Entity -> Response DTO
        // =====================================================
        private static PaymentResponseDto MapToResponse(
            Payment payment)
        {
            return new PaymentResponseDto
            {
                PaymentId =
                    payment.PaymentId,

                BookingId =
                    payment.BookingId,

                BookingNumber =
                    payment.Booking?.BookingNumber
                    ?? string.Empty,

                Amount =
                    payment.Amount,

                PaymentMethod =
                    payment.PaymentMethod,

                Status =
                    payment.Status,

                TransactionReference =
                    payment.TransactionReference,

                PaidAt =
                    payment.PaidAt,

                CreatedAt =
                    payment.CreatedAt,

                UpdatedAt =
                    payment.UpdatedAt
            };
        }
    }
}