using System.Security.Cryptography;
using EventParkingReservationSystem.API.DTOs.Bookings;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IParkingSlotRepository _parkingSlotRepository;
        private readonly INotificationService _notificationService;

        private const int BookingHoldMinutes = 15;

        public BookingService(
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            IEventRepository eventRepository,
            ISeatRepository seatRepository,
            IParkingSlotRepository parkingSlotRepository,
            INotificationService notificationService)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _eventRepository = eventRepository;
            _seatRepository = seatRepository;
            _parkingSlotRepository = parkingSlotRepository;
            _notificationService = notificationService;
        }

        // =====================================================
        // Create Booking
        // =====================================================
        public async Task<BookingResponseDto> CreateAsync(
            int customerId,
            CreateBookingDto dto)
        {
            // =================================================
            // Validate Customer
            // =================================================
            var customer =
                await _customerRepository.GetByIdAsync(
                    customerId);

            if (customer == null)
            {
                throw new KeyNotFoundException(
                    "Customer was not found.");
            }

            if (!string.Equals(
                    customer.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Deactivated customers cannot create bookings.");
            }

            if (!customer.EmailVerified)
            {
                throw new InvalidOperationException(
                    "Email verification is required before creating a booking.");
            }

            // =================================================
            // Validate Event
            // =================================================
            var eventItem =
                await _eventRepository.GetByIdAsync(
                    dto.EventId);

            if (eventItem == null)
            {
                throw new InvalidOperationException(
                    "Selected event was not found.");
            }

            if (!string.Equals(
                    eventItem.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The selected event is not available for booking.");
            }

            if (eventItem.StartDateTime <= DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    "Bookings cannot be created for an event that has already started.");
            }

            // =================================================
            // Validate Final Seat Layout
            // =================================================
            var totalSeatCount =
                await _seatRepository
                    .GetSeatCountByEventAsync(
                        dto.EventId);

            if (totalSeatCount != eventItem.Capacity)
            {
                throw new InvalidOperationException(
                    $"The seat layout is not ready for booking. The event capacity is {eventItem.Capacity}, but the current seat layout contains {totalSeatCount} seat(s).");
            }

            // =================================================
            // Validate Seat
            // =================================================
            var seat =
                await _seatRepository.GetByIdAsync(
                    dto.SeatId);

            if (seat == null)
            {
                throw new InvalidOperationException(
                    "Selected seat was not found.");
            }

            if (seat.EventId != dto.EventId)
            {
                throw new InvalidOperationException(
                    "The selected seat does not belong to the selected event.");
            }

            if (!seat.IsActive)
            {
                throw new InvalidOperationException(
                    "The selected seat is not currently available.");
            }

            // =================================================
            // Prevent Seat Double Booking
            // =================================================
            var seatAlreadyBooked =
                await _bookingRepository
                    .HasActiveBookingForSeatAsync(
                        dto.SeatId);

            if (seatAlreadyBooked)
            {
                throw new InvalidOperationException(
                    "The selected seat is already reserved or booked.");
            }

            // =================================================
            // Optional Parking
            // =================================================
            ParkingSlot? parkingSlot = null;

            decimal parkingFee = 0;

            if (dto.ParkingSlotId.HasValue)
            {
                parkingSlot =
                    await _parkingSlotRepository
                        .GetByIdAsync(
                            dto.ParkingSlotId.Value);

                if (parkingSlot == null)
                {
                    throw new InvalidOperationException(
                        "Selected parking slot was not found.");
                }

                if (parkingSlot.EventId != dto.EventId)
                {
                    throw new InvalidOperationException(
                        "The selected parking slot does not belong to the selected event.");
                }

                if (!parkingSlot.IsActive)
                {
                    throw new InvalidOperationException(
                        "The selected parking slot is not currently available.");
                }

                var parkingAlreadyBooked =
                    await _bookingRepository
                        .HasActiveBookingForParkingSlotAsync(
                            parkingSlot.ParkingSlotId);

                if (parkingAlreadyBooked)
                {
                    throw new InvalidOperationException(
                        "The selected parking slot is already reserved or booked.");
                }

                parkingFee =
                    parkingSlot.Fee;
            }

            // =================================================
            // Price Snapshot
            // =================================================
            var seatPrice =
                eventItem.TicketPrice;

            var totalAmount =
                seatPrice + parkingFee;

            // =================================================
            // Generate Booking Number
            // =================================================
            var bookingNumber =
                await GenerateBookingNumberAsync();

            // =================================================
            // Create Pending Booking
            // =================================================
            var currentTime =
                DateTime.UtcNow;

            var booking = new Booking
            {
                BookingNumber =
                    bookingNumber,

                CustomerId =
                    customerId,

                EventId =
                    dto.EventId,

                SeatId =
                    dto.SeatId,

                ParkingSlotId =
                    dto.ParkingSlotId,

                SeatPrice =
                    seatPrice,

                ParkingFee =
                    parkingFee,

                TotalAmount =
                    totalAmount,

                Status =
                    "Pending",

                HoldExpiresAt =
                    currentTime.AddMinutes(
                        BookingHoldMinutes),

                CreatedAt =
                    currentTime
            };

            try
            {
                await _bookingRepository
                    .AddAsync(booking);

                await _bookingRepository
                    .SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException(
                    "The selected seat or parking slot was just reserved by another customer. Please select another option.");
            }

            var createdBooking =
                await _bookingRepository.GetByIdAsync(
                    booking.BookingId);

            if (createdBooking == null)
            {
                throw new InvalidOperationException(
                    "Booking was created but could not be retrieved.");
            }

            return MapToResponse(
                createdBooking);
        }

        // =====================================================
        // Get Booking By Id
        // =====================================================
        public async Task<BookingResponseDto> GetByIdAsync(
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
                    "You are not authorized to view this booking.");
            }

            return MapToResponse(
                booking);
        }

        // =====================================================
        // Customer Booking History
        // =====================================================
        public async Task<List<BookingResponseDto>>
            GetCustomerBookingsAsync(
                int customerId)
        {
            var bookings =
                await _bookingRepository
                    .GetByCustomerIdAsync(
                        customerId);

            return bookings
                .Select(MapToResponse)
                .ToList();
        }

        // =====================================================
        // Cancel Booking
        // =====================================================
        public async Task<string> CancelAsync(
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

            // Customer can cancel own booking only.
            // Admin can cancel any booking.
            if (!isAdmin &&
                booking.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to cancel this booking.");
            }

            if (booking.Status == "Cancelled")
            {
                throw new InvalidOperationException(
                    "This booking has already been cancelled.");
            }

            if (booking.Status == "Expired")
            {
                throw new InvalidOperationException(
                    "An expired booking cannot be cancelled.");
            }

            booking.Status =
                "Cancelled";

            booking.CancelledAt =
                DateTime.UtcNow;

            booking.UpdatedAt =
                DateTime.UtcNow;

            await _bookingRepository
                .UpdateAsync(booking);

            await _bookingRepository
                .SaveChangesAsync();

            // =================================================
            // Internal Cancellation Notification
            // =================================================
            await _notificationService.CreateAsync(
                booking.CustomerId,
                booking.BookingId,
                "BookingCancelled",
                "Booking Cancelled",
                $"Your booking {booking.BookingNumber} has been cancelled.");

            return "Booking cancelled successfully.";
        }

        // =====================================================
        // Generate Unique Booking Number
        // Format: BKG-YYYY-XXXXXX
        // =====================================================
        private async Task<string>
            GenerateBookingNumberAsync()
        {
            for (var attempt = 0;
                 attempt < 10;
                 attempt++)
            {
                var randomNumber =
                    RandomNumberGenerator.GetInt32(
                        0,
                        1_000_000);

                var bookingNumber =
                    $"BKG-{DateTime.UtcNow.Year}-{randomNumber:D6}";

                var exists =
                    await _bookingRepository
                        .BookingNumberExistsAsync(
                            bookingNumber);

                if (!exists)
                {
                    return bookingNumber;
                }
            }

            throw new InvalidOperationException(
                "Unable to generate a unique booking number. Please try again.");
        }

        // =====================================================
        // Entity -> Response DTO
        // =====================================================
        private static BookingResponseDto MapToResponse(
            Booking booking)
        {
            return new BookingResponseDto
            {
                BookingId =
                    booking.BookingId,

                BookingNumber =
                    booking.BookingNumber,

                CustomerId =
                    booking.CustomerId,

                CustomerName =
                    booking.Customer == null
                        ? string.Empty
                        : $"{booking.Customer.FirstName} {booking.Customer.LastName}",

                EventId =
                    booking.EventId,

                EventName =
                    booking.Event?.Name
                    ?? string.Empty,

                SeatId =
                    booking.SeatId,

                SeatLabel =
                    booking.Seat?.SeatLabel
                    ?? string.Empty,

                ParkingSlotId =
                    booking.ParkingSlotId,

                ParkingSlotNumber =
                    booking.ParkingSlot?.SlotNumber,

                SeatPrice =
                    booking.SeatPrice,

                ParkingFee =
                    booking.ParkingFee,

                TotalAmount =
                    booking.TotalAmount,

                Status =
                    booking.Status,

                HoldExpiresAt =
                    booking.HoldExpiresAt,

                CreatedAt =
                    booking.CreatedAt,

                UpdatedAt =
                    booking.UpdatedAt,

                ConfirmedAt =
                    booking.ConfirmedAt,

                CancelledAt =
                    booking.CancelledAt
            };
        }
    }
}