using EventParkingReservationSystem.API.DTOs.Events;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(
            IEventService eventService)
        {
            _eventService = eventService;
        }

        // =====================================================
        // Get All / Search / Filter Events
        // GET: /api/events
        // GET: /api/events?search=concert
        // GET: /api/events?venueId=1
        // GET: /api/events?eventCategoryId=2
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? venueId,
            [FromQuery] int? eventCategoryId)
        {
            var events =
                await _eventService.GetAllAsync(
                    search,
                    venueId,
                    eventCategoryId);

            return Ok(events);
        }

        // =====================================================
        // Get Event By Id
        // GET: /api/events/{id}
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var eventItem =
                    await _eventService.GetByIdAsync(id);

                return Ok(eventItem);
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
        // Create Event
        // POST: /api/events
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateEventDto dto)
        {
            try
            {
                var eventItem =
                    await _eventService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = eventItem.EventId },
                    eventItem);
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
        // Update Event
        // PUT: /api/events/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateEventDto dto)
        {
            try
            {
                var eventItem =
                    await _eventService.UpdateAsync(
                        id,
                        dto);

                return Ok(eventItem);
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
        // Delete Event
        // DELETE: /api/events/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var message =
                    await _eventService.DeleteAsync(id);

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