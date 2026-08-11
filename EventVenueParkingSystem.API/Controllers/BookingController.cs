using System.Security.Claims;
using EventParkingReservationSystem.API.DTOs.Bookings;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(
            IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // =====================================================
        // Create Booking
        // POST: /api/bookings
        // Customer Only
        // =====================================================
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateBookingDto dto)
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
                var booking =
                    await _bookingService.CreateAsync(
                        customerId,
                        dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = booking.BookingId },
                    booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
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
        // Get Booking By Id
        // GET: /api/bookings/{id}
        //
        // Customer -> Own booking only
        // Admin    -> Any booking
        // =====================================================
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
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
                var isAdmin =
                    User.IsInRole("Admin");

                var booking =
                    await _bookingService.GetByIdAsync(
                        id,
                        customerId,
                        isAdmin);

                return Ok(booking);
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
        // Get Logged-In Customer Booking History
        // GET: /api/bookings/my
        // Customer Only
        // =====================================================
        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            if (!TryGetCustomerId(out var customerId))
            {
                return Unauthorized(new
                {
                    message = "Invalid authentication token."
                });
            }

            var bookings =
                await _bookingService
                    .GetCustomerBookingsAsync(
                        customerId);

            return Ok(bookings);
        }

        // =====================================================
        // Cancel Booking
        // POST: /api/bookings/{id}/cancel
        //
        // Customer -> Own booking only
        // Admin    -> Any booking
        // =====================================================
        [Authorize(Roles = "Customer,Admin")]
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(
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
                var isAdmin =
                    User.IsInRole("Admin");

                var message =
                    await _bookingService.CancelAsync(
                        id,
                        customerId,
                        isAdmin);

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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
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