using System.Security.Claims;
using EventParkingReservationSystem.API.DTOs.Auth;
using EventParkingReservationSystem.API.DTOs.Customers;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICustomerService _customerService;

        public CustomersController(
            IAuthService authService,
            ICustomerService customerService)
        {
            _authService = authService;
            _customerService = customerService;
        }

        // =====================================================
        // Register Customer
        // POST: /api/customers/register
        // Public endpoint
        // =====================================================
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDto registerDto)
        {
            try
            {
                var message =
                    await _authService.RegisterAsync(registerDto);

                return StatusCode(
                    StatusCodes.Status201Created,
                    new
                    {
                        message
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // Get Customer Profile
        // GET: /api/customers/{id}
        //
        // Customer -> own profile only
        // Admin    -> any customer
        // =====================================================
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            try
            {
                if (!CanAccessCustomer(id))
                {
                    return Forbid();
                }

                var customer =
                    await _customerService.GetByIdAsync(id);

                return Ok(customer);
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
        // Update Customer Profile
        // PUT: /api/customers/{id}
        //
        // Customer -> own profile only
        // Admin    -> allowed if required
        // =====================================================
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(
            int id,
            [FromBody] UpdateCustomerDto dto)
        {
            try
            {
                if (!CanAccessCustomer(id))
                {
                    return Forbid();
                }

                var customer =
                    await _customerService.UpdateAsync(id, dto);

                return Ok(customer);
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
        // Search Customers
        // GET: /api/customers?search=jathu
        // Admin only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> SearchCustomers(
            [FromQuery] string? search)
        {
            var customers =
                await _customerService.SearchAsync(search);

            return Ok(customers);
        }


        // =====================================================
        // Reactivate Customer
        // POST: /api/customers/{id}/reactivate
        // Admin only
        // =====================================================
        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/reactivate")]
        public async Task<IActionResult> ReactivateCustomer(
            int id)
        {
            try
            {
                var message =
                    await _customerService.ReactivateAsync(id);

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
        // Customer Access Validation
        // =====================================================
        private bool CanAccessCustomer(int customerId)
        {
            // Admin can access any customer
            if (User.IsInRole("Admin"))
            {
                return true;
            }

            var customerIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(
                    customerIdClaim,
                    out var loggedInCustomerId))
            {
                return false;
            }

            // Normal customer can access own profile only
            return loggedInCustomerId == customerId;
        }
    }
}