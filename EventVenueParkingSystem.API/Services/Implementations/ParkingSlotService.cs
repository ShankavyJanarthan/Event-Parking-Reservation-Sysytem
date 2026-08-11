using EventParkingReservationSystem.API.DTOs.ParkingSlots;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class ParkingSlotService : IParkingSlotService
    {
        private readonly IParkingSlotRepository _parkingSlotRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IBookingRepository _bookingRepository;

        public ParkingSlotService(
            IParkingSlotRepository parkingSlotRepository,
            IEventRepository eventRepository,
            IBookingRepository bookingRepository)
        {
            _parkingSlotRepository = parkingSlotRepository;
            _eventRepository = eventRepository;
            _bookingRepository = bookingRepository;
        }

        // =====================================================
        // Get Parking Slots By Event
        // =====================================================
        public async Task<List<ParkingSlotResponseDto>>
            GetByEventIdAsync(
                int eventId)
        {
            var eventItem =
                await _eventRepository.GetByIdAsync(
                    eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event was not found.");
            }

            var parkingSlots =
                await _parkingSlotRepository
                    .GetByEventIdAsync(
                        eventId);

            return parkingSlots
                .Select(MapToResponse)
                .ToList();
        }

        // =====================================================
        // Get Parking Slot By Id
        // =====================================================
        public async Task<ParkingSlotResponseDto> GetByIdAsync(
            int parkingSlotId)
        {
            var parkingSlot =
                await _parkingSlotRepository
                    .GetByIdAsync(
                        parkingSlotId);

            if (parkingSlot == null)
            {
                throw new KeyNotFoundException(
                    "Parking slot was not found.");
            }

            return MapToResponse(
                parkingSlot);
        }

        // =====================================================
        // Create Parking Slot
        // =====================================================
        public async Task<ParkingSlotResponseDto> CreateAsync(
            CreateParkingSlotDto dto)
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
                await _parkingSlotRepository
                    .SlotExistsAsync(
                        dto.EventId,
                        dto.SlotNumber);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "A parking slot with the same slot number already exists for this event.");
            }

            var parkingSlot = new ParkingSlot
            {
                EventId =
                    dto.EventId,

                SlotNumber =
                    dto.SlotNumber
                        .Trim()
                        .ToUpperInvariant(),

                Fee =
                    dto.Fee,

                IsActive =
                    dto.IsActive,

                CreatedAt =
                    DateTime.UtcNow
            };

            await _parkingSlotRepository.AddAsync(
                parkingSlot);

            await _parkingSlotRepository
                .SaveChangesAsync();

            var createdParkingSlot =
                await _parkingSlotRepository
                    .GetByIdAsync(
                        parkingSlot.ParkingSlotId);

            if (createdParkingSlot == null)
            {
                throw new InvalidOperationException(
                    "Parking slot was created but could not be retrieved.");
            }

            return MapToResponse(
                createdParkingSlot);
        }

        // =====================================================
        // Update Parking Slot
        // Cannot update actively reserved/booked parking
        // =====================================================
        public async Task<ParkingSlotResponseDto> UpdateAsync(
            int parkingSlotId,
            UpdateParkingSlotDto dto)
        {
            var parkingSlot =
                await _parkingSlotRepository
                    .GetByIdAsync(
                        parkingSlotId);

            if (parkingSlot == null)
            {
                throw new KeyNotFoundException(
                    "Parking slot was not found.");
            }

            // =================================================
            // Booking Safeguard
            // =================================================
            var hasActiveBooking =
                await _bookingRepository
                    .HasActiveBookingForParkingSlotAsync(
                        parkingSlotId);

            if (hasActiveBooking)
            {
                throw new InvalidOperationException(
                    "This parking slot cannot be updated because it is currently reserved or booked.");
            }

            var duplicateExists =
                await _parkingSlotRepository
                    .SlotExistsAsync(
                        parkingSlot.EventId,
                        dto.SlotNumber,
                        parkingSlotId);

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    "A parking slot with the same slot number already exists for this event.");
            }

            parkingSlot.SlotNumber =
                dto.SlotNumber
                    .Trim()
                    .ToUpperInvariant();

            parkingSlot.Fee =
                dto.Fee;

            parkingSlot.IsActive =
                dto.IsActive;

            parkingSlot.UpdatedAt =
                DateTime.UtcNow;

            await _parkingSlotRepository.UpdateAsync(
                parkingSlot);

            await _parkingSlotRepository
                .SaveChangesAsync();

            var updatedParkingSlot =
                await _parkingSlotRepository
                    .GetByIdAsync(
                        parkingSlotId);

            if (updatedParkingSlot == null)
            {
                throw new InvalidOperationException(
                    "Parking slot was updated but could not be retrieved.");
            }

            return MapToResponse(
                updatedParkingSlot);
        }

        // =====================================================
        // Delete Parking Slot
        // Cannot delete actively reserved/booked parking
        // =====================================================
        public async Task<string> DeleteAsync(
            int parkingSlotId)
        {
            var parkingSlot =
                await _parkingSlotRepository
                    .GetByIdAsync(
                        parkingSlotId);

            if (parkingSlot == null)
            {
                throw new KeyNotFoundException(
                    "Parking slot was not found.");
            }

            // =================================================
            // Booking Safeguard
            // =================================================
            var hasActiveBooking =
                await _bookingRepository
                    .HasActiveBookingForParkingSlotAsync(
                        parkingSlotId);

            if (hasActiveBooking)
            {
                throw new InvalidOperationException(
                    "This parking slot cannot be deleted because it is currently reserved or booked.");
            }

            await _parkingSlotRepository.DeleteAsync(
                parkingSlot);

            await _parkingSlotRepository
                .SaveChangesAsync();

            return "Parking slot deleted successfully.";
        }

        // =====================================================
        // Entity -> Response DTO
        // =====================================================
        private static ParkingSlotResponseDto MapToResponse(
            ParkingSlot parkingSlot)
        {
            return new ParkingSlotResponseDto
            {
                ParkingSlotId =
                    parkingSlot.ParkingSlotId,

                EventId =
                    parkingSlot.EventId,

                EventName =
                    parkingSlot.Event?.Name
                    ?? string.Empty,

                SlotNumber =
                    parkingSlot.SlotNumber,

                Fee =
                    parkingSlot.Fee,

                IsActive =
                    parkingSlot.IsActive,

                CreatedAt =
                    parkingSlot.CreatedAt,

                UpdatedAt =
                    parkingSlot.UpdatedAt
            };
        }
    }
}