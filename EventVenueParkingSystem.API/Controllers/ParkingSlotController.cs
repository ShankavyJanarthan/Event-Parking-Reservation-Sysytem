using EventParkingReservationSystem.API.DTOs.ParkingSlots;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/parking-slots")]
    public class ParkingSlotsController : ControllerBase
    {
        private readonly IParkingSlotService _parkingSlotService;

        public ParkingSlotsController(
            IParkingSlotService parkingSlotService)
        {
            _parkingSlotService = parkingSlotService;
        }

        // =====================================================
        // Get Parking Slot By Id
        // GET: /api/parking-slots/{id}
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var parkingSlot =
                    await _parkingSlotService.GetByIdAsync(id);

                return Ok(parkingSlot);
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
        // Get Parking Slots By Event
        // GET: /api/parking-slots/event/{eventId}
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet("event/{eventId:int}")]
        public async Task<IActionResult> GetByEvent(
            int eventId)
        {
            try
            {
                var parkingSlots =
                    await _parkingSlotService
                        .GetByEventIdAsync(eventId);

                return Ok(parkingSlots);
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
        // Create Parking Slot
        // POST: /api/parking-slots
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateParkingSlotDto dto)
        {
            try
            {
                var parkingSlot =
                    await _parkingSlotService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = parkingSlot.ParkingSlotId },
                    parkingSlot);
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
        // Update Parking Slot
        // PUT: /api/parking-slots/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateParkingSlotDto dto)
        {
            try
            {
                var parkingSlot =
                    await _parkingSlotService.UpdateAsync(
                        id,
                        dto);

                return Ok(parkingSlot);
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
        // Delete Parking Slot
        // DELETE: /api/parking-slots/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var message =
                    await _parkingSlotService.DeleteAsync(id);

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