using System.Security.Claims;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // =====================================================
        // Customer Dashboard
        // GET: /api/dashboard/customer
        // Customer Only
        // =====================================================
        [Authorize(Roles = "Customer")]
        [HttpGet("customer")]
        public async Task<IActionResult> GetCustomerDashboard()
        {
            if (!TryGetCustomerId(out var customerId))
            {
                return Unauthorized(new
                {
                    message = "Invalid authentication token."
                });
            }

            try
            {
                var dashboard =
                    await _dashboardService
                        .GetCustomerDashboardAsync(
                            customerId);

                return Ok(dashboard);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // =====================================================
        // Admin Dashboard
        // GET: /api/dashboard/admin
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var dashboard =
                await _dashboardService
                    .GetAdminDashboardAsync();

            return Ok(dashboard);
        }

        // =====================================================
        // Read CustomerId From JWT
        // =====================================================
        private bool TryGetCustomerId(
            out int customerId)
        {
            customerId = 0;

            var customerIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (customerIdClaim == null)
            {
                return false;
            }

            return int.TryParse(
                customerIdClaim.Value,
                out customerId);
        }
    }
}