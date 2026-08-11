using EventParkingReservationSystem.API.DTOs.Dashboard;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository
            _dashboardRepository;

        private readonly ICustomerRepository
            _customerRepository;

        public DashboardService(
            IDashboardRepository dashboardRepository,
            ICustomerRepository customerRepository)
        {
            _dashboardRepository =
                dashboardRepository;

            _customerRepository =
                customerRepository;
        }

        // =====================================================
        // Customer Dashboard
        // =====================================================
        public async Task<CustomerDashboardDto>
            GetCustomerDashboardAsync(
                int customerId)
        {
            var customer =
                await _customerRepository
                    .GetByIdAsync(customerId);

            if (customer == null)
            {
                throw new KeyNotFoundException(
                    "Customer was not found.");
            }

            return await _dashboardRepository
                .GetCustomerDashboardAsync(
                    customerId);
        }

        // =====================================================
        // Admin Dashboard
        // =====================================================
        public async Task<AdminDashboardDto>
            GetAdminDashboardAsync()
        {
            return await _dashboardRepository
                .GetAdminDashboardAsync();
        }
    }
}