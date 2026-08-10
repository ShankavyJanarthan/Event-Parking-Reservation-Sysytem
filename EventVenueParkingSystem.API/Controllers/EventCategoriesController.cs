using EventParkingReservationSystem.API.DTOs.EventCategories;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/event-categories")]
    public class EventCategoriesController : ControllerBase
    {
        private readonly IEventCategoryService _eventCategoryService;

        public EventCategoriesController(
            IEventCategoryService eventCategoryService)
        {
            _eventCategoryService = eventCategoryService;
        }

        // =====================================================
        // Get All Categories
        // GET: /api/event-categories
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories =
                await _eventCategoryService.GetAllAsync();

            return Ok(categories);
        }

        // =====================================================
        // Get Category By Id
        // GET: /api/event-categories/{id}
        // Public
        // =====================================================
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category =
                    await _eventCategoryService.GetByIdAsync(id);

                return Ok(category);
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
        // Create Category
        // POST: /api/event-categories
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateEventCategoryDto dto)
        {
            try
            {
                var category =
                    await _eventCategoryService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = category.EventCategoryId },
                    category);
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
        // Update Category
        // PUT: /api/event-categories/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateEventCategoryDto dto)
        {
            try
            {
                var category =
                    await _eventCategoryService.UpdateAsync(
                        id,
                        dto);

                return Ok(category);
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
        // Delete Category
        // DELETE: /api/event-categories/{id}
        // Admin Only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var message =
                    await _eventCategoryService.DeleteAsync(id);

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