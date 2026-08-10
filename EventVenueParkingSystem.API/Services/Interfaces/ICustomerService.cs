using EventParkingReservationSystem.API.DTOs.Customers;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponseDto> GetByIdAsync(int customerId);

        Task<CustomerResponseDto> UpdateAsync(
            int customerId,
            UpdateCustomerDto dto);

        Task<List<CustomerResponseDto>> SearchAsync(
            string? search);

        Task<string> ReactivateAsync(int customerId);
    }
}