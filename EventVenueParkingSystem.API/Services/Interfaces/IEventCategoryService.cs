using EventParkingReservationSystem.API.DTOs.EventCategories;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface IEventCategoryService
    {
        Task<List<EventCategoryResponseDto>> GetAllAsync();

        Task<EventCategoryResponseDto> GetByIdAsync(
            int eventCategoryId);

        Task<EventCategoryResponseDto> CreateAsync(
            CreateEventCategoryDto dto);

        Task<EventCategoryResponseDto> UpdateAsync(
            int eventCategoryId,
            UpdateEventCategoryDto dto);

        Task<string> DeleteAsync(
            int eventCategoryId);
    }
}