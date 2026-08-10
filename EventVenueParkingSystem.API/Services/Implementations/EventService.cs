using EventParkingReservationSystem.API.DTOs.Events;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IEventCategoryRepository _eventCategoryRepository;

        public EventService(
            IEventRepository eventRepository,
            IVenueRepository venueRepository,
            IEventCategoryRepository eventCategoryRepository)
        {
            _eventRepository = eventRepository;
            _venueRepository = venueRepository;
            _eventCategoryRepository = eventCategoryRepository;
        }

        // =====================================================
        // Get All / Search / Filter Events
        // =====================================================
        public async Task<List<EventResponseDto>> GetAllAsync(
            string? search,
            int? venueId,
            int? eventCategoryId)
        {
            var events =
                await _eventRepository.GetAllAsync(
                    search,
                    venueId,
                    eventCategoryId);

            return events
                .Select(MapToResponse)
                .ToList();
        }

        // =====================================================
        // Get Event By Id
        // =====================================================
        public async Task<EventResponseDto> GetByIdAsync(
            int eventId)
        {
            var eventItem =
                await _eventRepository.GetByIdAsync(eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event was not found.");
            }

            return MapToResponse(eventItem);
        }

        // =====================================================
        // Create Event
        // =====================================================
        public async Task<EventResponseDto> CreateAsync(
            CreateEventDto dto)
        {
            ValidateDateTime(
                dto.StartDateTime,
                dto.EndDateTime);

            var venue =
                await _venueRepository.GetByIdAsync(dto.VenueId);

            if (venue == null)
            {
                throw new InvalidOperationException(
                    "Selected venue was not found.");
            }

            if (!venue.IsAvailable)
            {
                throw new InvalidOperationException(
                    "Selected venue is currently unavailable.");
            }

            if (dto.Capacity > venue.Capacity)
            {
                throw new InvalidOperationException(
                    $"Event capacity cannot exceed venue capacity of {venue.Capacity}.");
            }

            var category =
                await _eventCategoryRepository
                    .GetByIdAsync(dto.EventCategoryId);

            if (category == null)
            {
                throw new InvalidOperationException(
                    "Selected event category was not found.");
            }

            if (!category.IsActive)
            {
                throw new InvalidOperationException(
                    "Selected event category is inactive.");
            }

            var hasOverlap =
                await _eventRepository.HasOverlappingEventAsync(
                    dto.VenueId,
                    dto.StartDateTime,
                    dto.EndDateTime);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "The selected venue already has another event during this time.");
            }

            var eventItem = new Event
            {
                Name = dto.Name.Trim(),

                Description =
                    dto.Description.Trim(),

                VenueId =
                    dto.VenueId,

                EventCategoryId =
                    dto.EventCategoryId,

                StartDateTime =
                    dto.StartDateTime,

                EndDateTime =
                    dto.EndDateTime,

                Capacity =
                    dto.Capacity,

                TicketPrice =
                    dto.TicketPrice,

                Status =
                    dto.Status.Trim(),

                CreatedAt =
                    DateTime.UtcNow
            };

            await _eventRepository.AddAsync(eventItem);

            await _eventRepository.SaveChangesAsync();

            // Reload to include Venue and Category
            var createdEvent =
                await _eventRepository.GetByIdAsync(
                    eventItem.EventId);

            if (createdEvent == null)
            {
                throw new InvalidOperationException(
                    "Event was created but could not be retrieved.");
            }

            return MapToResponse(createdEvent);
        }

        // =====================================================
        // Update Event
        // =====================================================
        public async Task<EventResponseDto> UpdateAsync(
            int eventId,
            UpdateEventDto dto)
        {
            var eventItem =
                await _eventRepository.GetByIdAsync(eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event was not found.");
            }

            ValidateDateTime(
                dto.StartDateTime,
                dto.EndDateTime);

            var venue =
                await _venueRepository.GetByIdAsync(dto.VenueId);

            if (venue == null)
            {
                throw new InvalidOperationException(
                    "Selected venue was not found.");
            }

            if (!venue.IsAvailable)
            {
                throw new InvalidOperationException(
                    "Selected venue is currently unavailable.");
            }

            if (dto.Capacity > venue.Capacity)
            {
                throw new InvalidOperationException(
                    $"Event capacity cannot exceed venue capacity of {venue.Capacity}.");
            }

            var category =
                await _eventCategoryRepository
                    .GetByIdAsync(dto.EventCategoryId);

            if (category == null)
            {
                throw new InvalidOperationException(
                    "Selected event category was not found.");
            }

            if (!category.IsActive)
            {
                throw new InvalidOperationException(
                    "Selected event category is inactive.");
            }

            var hasOverlap =
                await _eventRepository.HasOverlappingEventAsync(
                    dto.VenueId,
                    dto.StartDateTime,
                    dto.EndDateTime,
                    eventId);

            if (hasOverlap)
            {
                throw new InvalidOperationException(
                    "The selected venue already has another event during this time.");
            }

            // IMPORTANT:
            // BRD says event capacity cannot be reduced below
            // already booked seats.
            //
            // Booking/Seat module is not available yet.
            // We will integrate that validation later.

            eventItem.Name =
                dto.Name.Trim();

            eventItem.Description =
                dto.Description.Trim();

            eventItem.VenueId =
                dto.VenueId;

            eventItem.EventCategoryId =
                dto.EventCategoryId;

            eventItem.StartDateTime =
                dto.StartDateTime;

            eventItem.EndDateTime =
                dto.EndDateTime;

            eventItem.Capacity =
                dto.Capacity;

            eventItem.TicketPrice =
                dto.TicketPrice;

            eventItem.Status =
                dto.Status.Trim();

            eventItem.UpdatedAt =
                DateTime.UtcNow;

            await _eventRepository.UpdateAsync(eventItem);

            await _eventRepository.SaveChangesAsync();

            var updatedEvent =
                await _eventRepository.GetByIdAsync(eventId);

            if (updatedEvent == null)
            {
                throw new InvalidOperationException(
                    "Event was updated but could not be retrieved.");
            }

            return MapToResponse(updatedEvent);
        }

        // =====================================================
        // Delete Event
        // =====================================================
        public async Task<string> DeleteAsync(int eventId)
        {
            var eventItem =
                await _eventRepository.GetByIdAsync(eventId);

            if (eventItem == null)
            {
                throw new KeyNotFoundException(
                    "Event was not found.");
            }

            await _eventRepository.DeleteAsync(eventItem);

            await _eventRepository.SaveChangesAsync();

            return "Event deleted successfully.";
        }

        // =====================================================
        // Date Time Validation
        // =====================================================
        private static void ValidateDateTime(
            DateTime startDateTime,
            DateTime endDateTime)
        {
            if (startDateTime >= endDateTime)
            {
                throw new InvalidOperationException(
                    "Event start date and time must be before the end date and time.");
            }
        }

        // =====================================================
        // Entity -> Response DTO
        // =====================================================
        private static EventResponseDto MapToResponse(
            Event eventItem)
        {
            return new EventResponseDto
            {
                EventId =
                    eventItem.EventId,

                Name =
                    eventItem.Name,

                Description =
                    eventItem.Description,

                VenueId =
                    eventItem.VenueId,

                VenueName =
                    eventItem.Venue?.Name ?? string.Empty,

                EventCategoryId =
                    eventItem.EventCategoryId,

                EventCategoryName =
                    eventItem.EventCategory?.Name ?? string.Empty,

                StartDateTime =
                    eventItem.StartDateTime,

                EndDateTime =
                    eventItem.EndDateTime,

                Capacity =
                    eventItem.Capacity,

                TicketPrice =
                    eventItem.TicketPrice,

                Status =
                    eventItem.Status,

                CreatedAt =
                    eventItem.CreatedAt,

                UpdatedAt =
                    eventItem.UpdatedAt
            };
        }
    }
}