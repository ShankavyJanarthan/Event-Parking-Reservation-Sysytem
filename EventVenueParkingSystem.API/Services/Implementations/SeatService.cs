using EventParkingReservationSystem.API.DTOs.Seats;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IBookingRepository _bookingRepository;

        public SeatService(
            ISeatRepository seatRepository,
            IEventRepository eventRepository,
            IBookingRepository bookingRepository)
        {
            _seatRepository = seatRepository;
            _eventRepository = eventRepository;
            _bookingRepository = bookingRepository;
        }

        // =====================================================
        // Get Seats By Event
        // =====================================================
        public async Task<List<SeatResponseDto>> GetByEventIdAsync(
            int eventId)
        {
            var eventItem =
                await _eventRepository.GetByIdAsync(eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event was not found.");
            }

            var seats =
                await _seatRepository.GetByEventIdAsync(eventId);

            return seats
                .Select(MapToResponse)
                .ToList();
        }

        // =====================================================
        // Get Seat By Id
        // =====================================================
        public async Task<SeatResponseDto> GetByIdAsync(
            int seatId)
        {
            var seat =
                await _seatRepository.GetByIdAsync(seatId);

            if (seat == null)
            {
                throw new KeyNotFoundException(
                    "Seat was not found.");
            }

            return MapToResponse(seat);
        }

        // =====================================================
        // Create Seat
        // =====================================================
        public async Task<SeatResponseDto> CreateAsync(
            CreateSeatDto dto)
        {
            var eventItem =
                await _eventRepository.GetByIdAsync(
                    dto.EventId);

            if (eventItem == null)
            {
                throw new InvalidOperationException(
                    "Selected event was not found.");
            }

            var duplicateExists =
                await _seatRepository.SeatExistsAsync(
                    dto.EventId,
                    dto.RowLabel,
                    dto.SeatNumber,
                    dto.SeatLabel);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "A seat with the same row, seat number, or seat label already exists for this event.");
            }

            var currentSeatCount =
                await _seatRepository
                    .GetSeatCountByEventAsync(dto.EventId);

            if (currentSeatCount >= eventItem.Capacity)
            {
                throw new InvalidOperationException(
                    $"Seat layout cannot exceed the event capacity of {eventItem.Capacity}.");
            }

            var seat = new Seat
            {
                EventId = dto.EventId,

                RowLabel =
                    dto.RowLabel.Trim().ToUpperInvariant(),

                SeatNumber =
                    dto.SeatNumber,

                SeatLabel =
                    dto.SeatLabel.Trim().ToUpperInvariant(),

                IsActive =
                    dto.IsActive,

                CreatedAt =
                    DateTime.UtcNow
            };

            await _seatRepository.AddAsync(seat);

            await _seatRepository.SaveChangesAsync();

            var createdSeat =
                await _seatRepository.GetByIdAsync(
                    seat.SeatId);

            if (createdSeat == null)
            {
                throw new InvalidOperationException(
                    "Seat was created but could not be retrieved.");
            }

            return MapToResponse(createdSeat);
        }

        // =====================================================
        // Update Seat
        // Cannot update an actively reserved/booked seat
        // =====================================================
        public async Task<SeatResponseDto> UpdateAsync(
            int seatId,
            UpdateSeatDto dto)
        {
            var seat =
                await _seatRepository.GetByIdAsync(seatId);

            if (seat == null)
            {
                throw new KeyNotFoundException(
                    "Seat was not found.");
            }

            var hasActiveBooking =
                await _bookingRepository
                    .HasActiveBookingForSeatAsync(
                        seatId);

            if (hasActiveBooking)
            {
                throw new InvalidOperationException(
                    "This seat cannot be updated because it is currently reserved or booked.");
            }

            var duplicateExists =
                await _seatRepository.SeatExistsAsync(
                    seat.EventId,
                    dto.RowLabel,
                    dto.SeatNumber,
                    dto.SeatLabel,
                    seatId);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "A seat with the same row, seat number, or seat label already exists for this event.");
            }

            seat.RowLabel =
                dto.RowLabel.Trim().ToUpperInvariant();

            seat.SeatNumber =
                dto.SeatNumber;

            seat.SeatLabel =
                dto.SeatLabel.Trim().ToUpperInvariant();

            seat.IsActive =
                dto.IsActive;

            seat.UpdatedAt =
                DateTime.UtcNow;

            await _seatRepository.UpdateAsync(seat);

            await _seatRepository.SaveChangesAsync();

            var updatedSeat =
                await _seatRepository.GetByIdAsync(
                    seatId);

            if (updatedSeat == null)
            {
                throw new InvalidOperationException(
                    "Seat was updated but could not be retrieved.");
            }

            return MapToResponse(updatedSeat);
        }

        // =====================================================
        // Delete Seat
        // Cannot delete an actively reserved/booked seat
        // =====================================================
        public async Task<string> DeleteAsync(
            int seatId)
        {
            var seat =
                await _seatRepository.GetByIdAsync(seatId);

            if (seat == null)
            {
                throw new KeyNotFoundException(
                    "Seat was not found.");
            }

            var hasActiveBooking =
                await _bookingRepository
                    .HasActiveBookingForSeatAsync(
                        seatId);

            if (hasActiveBooking)
            {
                throw new InvalidOperationException(
                    "This seat cannot be deleted because it is currently reserved or booked.");
            }

            await _seatRepository.DeleteAsync(seat);

            await _seatRepository.SaveChangesAsync();

            return "Seat deleted successfully.";
        }

        // =====================================================
        // Validate Final Seat Layout
        // Seat count must exactly equal Event Capacity
        // =====================================================
        public async Task<string> ValidateSeatLayoutAsync(
            int eventId)
        {
            var eventItem =
                await _eventRepository.GetByIdAsync(eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event was not found.");
            }

            var seatCount =
                await _seatRepository
                    .GetSeatCountByEventAsync(eventId);

            if (seatCount < eventItem.Capacity)
            {
                var remainingSeats =
                    eventItem.Capacity - seatCount;

                throw new InvalidOperationException(
                    $"Seat layout is incomplete. {remainingSeats} more seat(s) must be added to match the event capacity of {eventItem.Capacity}.");
            }

            if (seatCount > eventItem.Capacity)
            {
                throw new InvalidOperationException(
                    $"Seat layout exceeds the event capacity of {eventItem.Capacity}.");
            }

            return
                $"Seat layout is valid. Total seats: {seatCount}.";
        }

        // =====================================================
        // Entity -> Response DTO
        // =====================================================
        private static SeatResponseDto MapToResponse(
            Seat seat)
        {
            return new SeatResponseDto
            {
                SeatId =
                    seat.SeatId,

                EventId =
                    seat.EventId,

                EventName =
                    seat.Event?.Name ?? string.Empty,

                RowLabel =
                    seat.RowLabel,

                SeatNumber =
                    seat.SeatNumber,

                SeatLabel =
                    seat.SeatLabel,

                IsActive =
                    seat.IsActive,

                CreatedAt =
                    seat.CreatedAt,

                UpdatedAt =
                    seat.UpdatedAt
            };
        }
    }
}