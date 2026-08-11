using EventParkingReservationSystem.API.DTOs.Auth;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(
            RegisterDto registerDto);

        Task<AuthResponseDto> LoginAsync(
            LoginDto loginDto);

        Task<string> VerifyEmailAsync(
            string token);

        Task<string> ResendVerificationAsync(
            ResendVerificationDto dto);

        Task<string> ForgotPasswordAsync(
            ForgotPasswordDto dto);

        Task<string> ResetPasswordAsync(
            ResetPasswordDto dto);
    }
}