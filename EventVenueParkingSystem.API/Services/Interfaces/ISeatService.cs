using EventParkingReservationSystem.API.DTOs.Seats;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface ISeatService
    {
        Task<List<SeatResponseDto>> GetByEventIdAsync(
            int eventId);

        Task<SeatResponseDto> GetByIdAsync(
            int seatId);

        Task<SeatResponseDto> CreateAsync(
            CreateSeatDto dto);

        Task<SeatResponseDto> UpdateAsync(
            int seatId,
            UpdateSeatDto dto);

        Task<string> DeleteAsync(
            int seatId);

        Task<string> ValidateSeatLayoutAsync(
            int eventId);
    }
}