using EventParkingReservationSystem.API.DTOs.Dashboard;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<CustomerDashboardDto> GetCustomerDashboardAsync(
            int customerId);

        Task<AdminDashboardDto> GetAdminDashboardAsync();
    }
}