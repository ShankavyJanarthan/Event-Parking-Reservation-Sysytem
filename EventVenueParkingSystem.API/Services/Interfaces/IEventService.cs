using EventParkingReservationSystem.API.DTOs.Events;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventResponseDto>> GetAllAsync(
            string? search,
            int? venueId,
            int? eventCategoryId);

        Task<EventResponseDto> GetByIdAsync(int eventId);

        Task<EventResponseDto> CreateAsync(
            CreateEventDto dto);

        Task<EventResponseDto> UpdateAsync(
            int eventId,
            UpdateEventDto dto);

        Task<string> DeleteAsync(int eventId);
    }
}