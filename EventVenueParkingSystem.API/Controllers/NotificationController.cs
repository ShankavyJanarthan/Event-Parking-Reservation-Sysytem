using System.Security.Claims;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // =====================================================
        // Get Logged-In Customer Notifications
        // GET: /api/notifications/my
        // Customer Only
        // =====================================================
        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotifications()
        {
            if (!TryGetCustomerId(out var customerId))
            {
                return Unauthorized(new
                {
                    message = "Invalid authentication token."
                });
            }

            var notifications =
                await _notificationService
                    .GetCustomerNotificationsAsync(
                        customerId);

            return Ok(notifications);
        }

        // =====================================================
        // Mark Notification As Read
        // POST: /api/notifications/{id}/read
        // Customer Only
        // =====================================================
        [Authorize(Roles = "Customer")]
        [HttpPost("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead(
            int id)
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
                var message =
                    await _notificationService
                        .MarkAsReadAsync(
                            id,
                            customerId);

                return Ok(new
                {
                    message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message = ex.Message
                    });
            }
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