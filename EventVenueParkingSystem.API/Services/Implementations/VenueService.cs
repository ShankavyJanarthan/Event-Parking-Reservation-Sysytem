using EventParkingReservationSystem.API.DTOs.Venues;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;

        public VenueService(
            IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        // =====================================================
        // Get All Venues
        // =====================================================
        public async Task<List<VenueResponseDto>> GetAllAsync()
        {
            var venues =
                await _venueRepository.GetAllAsync();

            return venues
                .Select(MapToResponse)
                .ToList();
        }

        // =====================================================
        // Get Venue By Id
        // =====================================================
        public async Task<VenueResponseDto> GetByIdAsync(
            int venueId)
        {
            var venue =
                await _venueRepository.GetByIdAsync(venueId);

            if (venue == null)
            {
                throw new KeyNotFoundException(
                    "Venue was not found.");
            }

            return MapToResponse(venue);
        }

        // =====================================================
        // Create Venue
        // =====================================================
        public async Task<VenueResponseDto> CreateAsync(
            CreateVenueDto dto)
        {
            var venue = new Venue
            {
                Name = dto.Name.Trim(),

                Address = dto.Address.Trim(),

                Description =
                    dto.Description.Trim(),

                Capacity = dto.Capacity,

                IsAvailable = dto.IsAvailable,

                CreatedAt = DateTime.UtcNow
            };

            await _venueRepository.AddAsync(venue);

            await _venueRepository.SaveChangesAsync();

            return MapToResponse(venue);
        }

        // =====================================================
        // Update Venue
        // =====================================================
        public async Task<VenueResponseDto> UpdateAsync(
            int venueId,
            UpdateVenueDto dto)
        {
            var venue =
                await _venueRepository.GetByIdAsync(venueId);

            if (venue == null)
            {
                throw new KeyNotFoundException(
                    "Venue was not found.");
            }

            venue.Name = dto.Name.Trim();

            venue.Address = dto.Address.Trim();

            venue.Description =
                dto.Description.Trim();

            venue.Capacity = dto.Capacity;

            venue.IsAvailable =
                dto.IsAvailable;

            venue.UpdatedAt =
                DateTime.UtcNow;

            await _venueRepository.UpdateAsync(venue);

            await _venueRepository.SaveChangesAsync();

            return MapToResponse(venue);
        }

        // =====================================================
        // Delete Venue
        // =====================================================
        public async Task<string> DeleteAsync(
            int venueId)
        {
            var venue =
                await _venueRepository.GetByIdAsync(venueId);

            if (venue == null)
            {
                throw new KeyNotFoundException(
                    "Venue was not found.");
            }

            var hasUpcomingEvents =
                await _venueRepository
                    .HasUpcomingEventsAsync(venueId);

            if (hasUpcomingEvents)
            {
                throw new InvalidOperationException(
                    "Venue cannot be deleted because it has upcoming events.");
            }

            await _venueRepository.DeleteAsync(venue);

            await _venueRepository.SaveChangesAsync();

            return "Venue deleted successfully.";
        }

        // =====================================================
        // Map Entity -> Response DTO
        // =====================================================
        private static VenueResponseDto MapToResponse(
            Venue venue)
        {
            return new VenueResponseDto
            {
                VenueId = venue.VenueId,

                Name = venue.Name,

                Address = venue.Address,

                Description = venue.Description,

                Capacity = venue.Capacity,

                IsAvailable =
                    venue.IsAvailable,

                CreatedAt =
                    venue.CreatedAt,

                UpdatedAt =
                    venue.UpdatedAt
            };
        }
    }
}