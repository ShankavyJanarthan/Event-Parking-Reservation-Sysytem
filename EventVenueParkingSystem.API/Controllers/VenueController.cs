using EventParkingReservationSystem.API.DTOs.Venues;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/venues")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(
            IVenueService venueService)
        {
            _venueService = venueService;
        }

        // =====================================================
        // Get All Venues
        // GET: /api/venues
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var venues =
                await _venueService.GetAllAsync();

            return Ok(venues);
        }


        // =====================================================
        // Get Venue By Id
        // GET: /api/venues/{id}
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var venue =
                    await _venueService.GetByIdAsync(id);

                return Ok(venue);
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
        // Create Venue
        // POST: /api/venues
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateVenueDto dto)
        {
            var venue =
                await _venueService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = venue.VenueId },
                venue);
        }


        // =====================================================
        // Update Venue
        // PUT: /api/venues/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateVenueDto dto)
        {
            try
            {
                var venue =
                    await _venueService.UpdateAsync(
                        id,
                        dto);

                return Ok(venue);
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
        // Delete Venue
        // DELETE: /api/venues/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var message =
                    await _venueService.DeleteAsync(id);

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