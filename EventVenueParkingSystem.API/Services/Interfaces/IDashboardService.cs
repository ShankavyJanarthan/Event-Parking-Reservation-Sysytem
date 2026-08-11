using EventParkingReservationSystem.API.DTOs.Dashboard;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<CustomerDashboardDto>
            GetCustomerDashboardAsync(
                int customerId);

        Task<AdminDashboardDto>
            GetAdminDashboardAsync();
    }
}