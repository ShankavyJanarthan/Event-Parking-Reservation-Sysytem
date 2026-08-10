using EventParkingReservationSystem.API.DTOs.Venues;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface IVenueService
    {
        Task<List<VenueResponseDto>> GetAllAsync();

        Task<VenueResponseDto> GetByIdAsync(int venueId);

        Task<VenueResponseDto> CreateAsync(
            CreateVenueDto dto);

        Task<VenueResponseDto> UpdateAsync(
            int venueId,
            UpdateVenueDto dto);

        Task<string> DeleteAsync(int venueId);
    }
}