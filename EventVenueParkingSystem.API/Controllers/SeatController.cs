using EventParkingReservationSystem.API.DTOs.Seats;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/seats")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(
            ISeatService seatService)
        {
            _seatService = seatService;
        }

        // =====================================================
        // Get Seat By Id
        // GET: /api/seats/{id}
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var seat =
                    await _seatService.GetByIdAsync(id);

                return Ok(seat);
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
        // Get Seats By Event
        // GET: /api/seats/event/{eventId}
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetByEvent(
            int eventId)
        {
            try
            {
                var seats =
                    await _seatService.GetByEventIdAsync(
                        eventId);

                return Ok(seats);
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
        // Create Seat
        // POST: /api/seats
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateSeatDto dto)
        {
            try
            {
                var seat =
                    await _seatService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = seat.SeatId },
                    seat);
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
        // Update Seat
        // PUT: /api/seats/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateSeatDto dto)
        {
            try
            {
                var seat =
                    await _seatService.UpdateAsync(
                        id,
                        dto);

                return Ok(seat);
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
        // Delete Seat
        // DELETE: /api/seats/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var message =
                    await _seatService.DeleteAsync(id);

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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =====================================================
        // Validate Final Seat Layout
        // POST: /api/seats/event/{eventId}/validate-layout
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPost("event/{eventId:int}/validate-layout")]
        public async Task<IActionResult> ValidateLayout(
            int eventId)
        {
            try
            {
                var message =
                    await _seatService
                        .ValidateSeatLayoutAsync(eventId);

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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}