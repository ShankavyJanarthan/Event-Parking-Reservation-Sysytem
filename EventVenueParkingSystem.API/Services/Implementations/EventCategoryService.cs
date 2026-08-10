using EventParkingReservationSystem.API.DTOs.EventCategories;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class EventCategoryService : IEventCategoryService
    {
        private readonly IEventCategoryRepository _eventCategoryRepository;

        public EventCategoryService(
            IEventCategoryRepository eventCategoryRepository)
        {
            _eventCategoryRepository = eventCategoryRepository;
        }

        // =====================================================
        // Get All Categories
        // =====================================================
        public async Task<List<EventCategoryResponseDto>> GetAllAsync()
        {
            var categories =
                await _eventCategoryRepository.GetAllAsync();

            return categories
                .Select(MapToResponse)
                .ToList();
        }

        // =====================================================
        // Get Category By Id
        // =====================================================
        public async Task<EventCategoryResponseDto> GetByIdAsync(
            int eventCategoryId)
        {
            var category =
                await _eventCategoryRepository
                    .GetByIdAsync(eventCategoryId);

            if (category == null)
            {
                throw new KeyNotFoundException(
                    "Event category was not found.");
            }

            return MapToResponse(category);
        }

        // =====================================================
        // Create Category
        // =====================================================
        public async Task<EventCategoryResponseDto> CreateAsync(
            CreateEventCategoryDto dto)
        {
            var normalizedName =
                dto.Name.Trim();

            var existingCategory =
                await _eventCategoryRepository
                    .GetByNameAsync(normalizedName);

            if (existingCategory != null)
            {
                throw new InvalidOperationException(
                    "An event category with this name already exists.");
            }

            var category = new EventCategory
            {
                Name = normalizedName,

                Description =
                    dto.Description.Trim(),

                IsActive =
                    dto.IsActive,

                CreatedAt =
                    DateTime.UtcNow
            };

            await _eventCategoryRepository
                .AddAsync(category);

            await _eventCategoryRepository
                .SaveChangesAsync();

            return MapToResponse(category);
        }

        // =====================================================
        // Update Category
        // =====================================================
        public async Task<EventCategoryResponseDto> UpdateAsync(
            int eventCategoryId,
            UpdateEventCategoryDto dto)
        {
            var category =
                await _eventCategoryRepository
                    .GetByIdAsync(eventCategoryId);

            if (category == null)
            {
                throw new KeyNotFoundException(
                    "Event category was not found.");
            }

            var normalizedName =
                dto.Name.Trim();

            var existingCategory =
                await _eventCategoryRepository
                    .GetByNameAsync(normalizedName);

            // Another category cannot use same name
            if (existingCategory != null &&
                existingCategory.EventCategoryId != eventCategoryId)
            {
                throw new InvalidOperationException(
                    "An event category with this name already exists.");
            }

            category.Name =
                normalizedName;

            category.Description =
                dto.Description.Trim();

            category.IsActive =
                dto.IsActive;

            category.UpdatedAt =
                DateTime.UtcNow;

            await _eventCategoryRepository
                .UpdateAsync(category);

            await _eventCategoryRepository
                .SaveChangesAsync();

            return MapToResponse(category);
        }

        // =====================================================
        // Delete Category
        // =====================================================
        public async Task<string> DeleteAsync(
            int eventCategoryId)
        {
            var category =
                await _eventCategoryRepository
                    .GetByIdAsync(eventCategoryId);

            if (category == null)
            {
                throw new KeyNotFoundException(
                    "Event category was not found.");
            }

            var isAssignedToEvents =
                await _eventCategoryRepository
                    .IsAssignedToEventsAsync(eventCategoryId);

            if (isAssignedToEvents)
            {
                throw new InvalidOperationException(
                    "Event category cannot be deleted because it is assigned to one or more events.");
            }

            await _eventCategoryRepository
                .DeleteAsync(category);

            await _eventCategoryRepository
                .SaveChangesAsync();

            return "Event category deleted successfully.";
        }

        // =====================================================
        // Entity -> Response DTO
        // =====================================================
        private static EventCategoryResponseDto MapToResponse(
            EventCategory category)
        {
            return new EventCategoryResponseDto
            {
                EventCategoryId =
                    category.EventCategoryId,

                Name =
                    category.Name,

                Description =
                    category.Description,

                IsActive =
                    category.IsActive,

                CreatedAt =
                    category.CreatedAt,

                UpdatedAt =
                    category.UpdatedAt
            };
        }
    }
}