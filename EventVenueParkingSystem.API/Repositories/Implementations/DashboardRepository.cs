using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.DTOs.Dashboard;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Customer Dashboard
        // =====================================================
        public async Task<CustomerDashboardDto>
            GetCustomerDashboardAsync(
                int customerId)
        {
            var currentTime =
                DateTime.UtcNow;

            var bookings =
                _context.Bookings
                    .Where(x =>
                        x.CustomerId == customerId);

            var totalBookings =
                await bookings.CountAsync();

            var pendingBookings =
                await bookings.CountAsync(
                    x => x.Status == "Pending");

            var confirmedBookings =
                await bookings.CountAsync(
                    x => x.Status == "Confirmed");

            var cancelledBookings =
                await bookings.CountAsync(
                    x => x.Status == "Cancelled");

            var expiredBookings =
                await bookings.CountAsync(
                    x => x.Status == "Expired");

            var upcomingBookings =
                await bookings.CountAsync(
                    x =>
                        x.Event != null &&
                        x.Event.StartDateTime > currentTime &&
                        (
                            x.Status == "Pending" ||
                            x.Status == "Confirmed"
                        ));

            var totalSpent =
                await _context.Payments
                    .Where(x =>
                        x.Booking != null &&
                        x.Booking.CustomerId == customerId &&
                        x.Status == "Successful")
                    .SumAsync(
                        x => (decimal?)x.Amount)
                ?? 0;

            return new CustomerDashboardDto
            {
                CustomerId =
                    customerId,

                TotalBookings =
                    totalBookings,

                PendingBookings =
                    pendingBookings,

                ConfirmedBookings =
                    confirmedBookings,

                CancelledBookings =
                    cancelledBookings,

                ExpiredBookings =
                    expiredBookings,

                UpcomingBookings =
                    upcomingBookings,

                TotalSpent =
                    totalSpent
            };
        }

        // =====================================================
        // Admin Dashboard
        // =====================================================
        public async Task<AdminDashboardDto>
            GetAdminDashboardAsync()
        {
            var currentTime =
                DateTime.UtcNow;

            // Only normal customer accounts.
            // Seeded Admin account is not included.
            var customers =
                _context.Customers
                    .Where(x =>
                        x.Role == "Customer");

            var totalCustomers =
                await customers.CountAsync();

            var activeCustomers =
                await customers.CountAsync(
                    x => x.Status == "Active");

            var deactivatedCustomers =
                await customers.CountAsync(
                    x => x.Status == "Deactivated");

            var totalEvents =
                await _context.Events.CountAsync();

            var upcomingEvents =
                await _context.Events.CountAsync(
                    x =>
                        x.StartDateTime > currentTime &&
                        x.Status == "Active");

            var totalBookings =
                await _context.Bookings.CountAsync();

            var pendingBookings =
                await _context.Bookings.CountAsync(
                    x => x.Status == "Pending");

            var confirmedBookings =
                await _context.Bookings.CountAsync(
                    x => x.Status == "Confirmed");

            var cancelledBookings =
                await _context.Bookings.CountAsync(
                    x => x.Status == "Cancelled");

            var expiredBookings =
                await _context.Bookings.CountAsync(
                    x => x.Status == "Expired");

            var totalRevenue =
                await _context.Payments
                    .Where(x =>
                        x.Status == "Successful")
                    .SumAsync(
                        x => (decimal?)x.Amount)
                ?? 0;

            return new AdminDashboardDto
            {
                TotalCustomers =
                    totalCustomers,

                ActiveCustomers =
                    activeCustomers,

                DeactivatedCustomers =
                    deactivatedCustomers,

                TotalEvents =
                    totalEvents,

                UpcomingEvents =
                    upcomingEvents,

                TotalBookings =
                    totalBookings,

                PendingBookings =
                    pendingBookings,

                ConfirmedBookings =
                    confirmedBookings,

                CancelledBookings =
                    cancelledBookings,

                ExpiredBookings =
                    expiredBookings,

                TotalRevenue =
                    totalRevenue
            };
        }
    }
}