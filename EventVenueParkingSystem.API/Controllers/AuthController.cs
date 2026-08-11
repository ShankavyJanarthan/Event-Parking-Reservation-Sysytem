using EventParkingReservationSystem.API.DTOs.Auth;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventParkingReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // =====================================================
        // Login
        // POST: /api/auth/login
        // =====================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto loginDto)
        {
            try
            {
                var result =
                    await _authService.LoginAsync(loginDto);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // Verify Email
        // GET: /api/auth/verify-email?token=xxxxx
        // =====================================================
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(
            [FromQuery] string token)
        {
            try
            {
                var message =
                    await _authService.VerifyEmailAsync(token);

                return Ok(new
                {
                    message
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
        // Resend Verification
        // POST: /api/auth/resend-verification
        // =====================================================
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification(
            [FromBody] ResendVerificationDto dto)
        {
            try
            {
                var message =
                    await _authService
                        .ResendVerificationAsync(dto);

                return Ok(new
                {
                    message
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
        // Forgot Password
        // POST: /api/auth/forgot-password
        // =====================================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordDto dto)
        {
            var message =
                await _authService.ForgotPasswordAsync(dto);

            return Ok(new
            {
                message
            });
        }


        // =====================================================
        // Reset Password
        // POST: /api/auth/reset-password
        // =====================================================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto dto)
        {
            try
            {
                var message =
                    await _authService.ResetPasswordAsync(dto);

                return Ok(new
                {
                    message
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