using System.Security.Claims;
using EventParkingReservationSystem.API.DTOs.Payments;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // =====================================================
        // Process Payment
        // POST: /api/payments/booking/{bookingId}
        //
        // Customer -> Own booking only
        // Admin    -> Any booking
        // =====================================================
        [Authorize(Roles = "Customer,Admin")]
        [HttpPost("booking/{bookingId:int}")]
        public async Task<IActionResult> ProcessPayment(
            int bookingId,
            [FromBody] ProcessPaymentDto dto)
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
                var isAdmin =
                    User.IsInRole("Admin");

                var payment =
                    await _paymentService
                        .ProcessPaymentAsync(
                            bookingId,
                            customerId,
                            isAdmin,
                            dto);

                return Ok(payment);
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =====================================================
        // Get Payment By Booking
        // GET: /api/payments/booking/{bookingId}
        //
        // Customer -> Own booking only
        // Admin    -> Any booking
        // =====================================================
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("booking/{bookingId:int}")]
        public async Task<IActionResult> GetByBooking(
            int bookingId)
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
                var isAdmin =
                    User.IsInRole("Admin");

                var payment =
                    await _paymentService
                        .GetByBookingIdAsync(
                            bookingId,
                            customerId,
                            isAdmin);

                return Ok(payment);
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