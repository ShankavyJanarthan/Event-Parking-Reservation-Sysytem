using EventParkingReservationSystem.API.Configuration;
using EventParkingReservationSystem.API.DTOs.Auth;
using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly PasswordHelper _passwordHelper;
        private readonly TokenHelper _tokenHelper;
        private readonly JwtHelper _jwtHelper;
        private readonly IEmailService _emailService;
        private readonly EmailSettings _emailSettings;

        public AuthService(
            ICustomerRepository customerRepository,
            PasswordHelper passwordHelper,
            TokenHelper tokenHelper,
            JwtHelper jwtHelper,
            IEmailService emailService,
            IOptions<EmailSettings> emailOptions)
        {
            _customerRepository = customerRepository;
            _passwordHelper = passwordHelper;
            _tokenHelper = tokenHelper;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _emailSettings = emailOptions.Value;
        }

        // =====================================================
        // Register
        // =====================================================
        public async Task<string> RegisterAsync(
            RegisterDto registerDto)
        {
            var normalizedEmail = registerDto.Email
                .Trim()
                .ToLowerInvariant();

            var emailExists =
                await _customerRepository.EmailExistsAsync(
                    normalizedEmail);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "A customer with this email address already exists.");
            }

            var passwordHash =
                _passwordHelper.HashPassword(
                    registerDto.Password);

            var rawVerificationToken =
                _tokenHelper.GenerateToken();

            var hashedVerificationToken =
                _tokenHelper.HashToken(
                    rawVerificationToken);

            var customer = new Customer
            {
                FirstName = registerDto.FirstName.Trim(),
                LastName = registerDto.LastName.Trim(),
                Email = normalizedEmail,
                Phone = registerDto.Phone.Trim(),

                PasswordHash = passwordHash,

                Role = "Customer",
                Status = "Active",

                EmailVerified = false,

                EmailVerificationToken =
                    hashedVerificationToken,

                EmailVerificationTokenExpiresAt =
                    DateTime.UtcNow.AddHours(
                        _emailSettings.VerificationExpiryHours),

                CreatedAt = DateTime.UtcNow
            };

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            // Build verification link
            var verificationLink =
                $"{_emailSettings.ApplicationBaseUrl}" +
                $"/api/auth/verify-email?token=" +
                $"{Uri.EscapeDataString(rawVerificationToken)}";

            var emailBody = $@"
                <h2>Verify Your Email</h2>

                <p>Hello {customer.FirstName},</p>

                <p>
                    Thank you for registering with the
                    Event & Parking Reservation System.
                </p>

                <p>
                    Please verify your email address by
                    clicking the link below:
                </p>

                <p>
                    <a href=""{verificationLink}"">
                        Verify Email
                    </a>
                </p>

                <p>
                    This verification link expires in
                    {_emailSettings.VerificationExpiryHours} hours.
                </p>
            ";

            await _emailService.SendEmailAsync(
                customer.Email,
                "Verify Your Email Address",
                emailBody);

            return "Registration successful. Please check your email to verify your account.";
        }

        // =====================================================
        // Login
        // =====================================================
        public async Task<AuthResponseDto> LoginAsync(
            LoginDto loginDto)
        {
            var normalizedEmail = loginDto.Email
                .Trim()
                .ToLowerInvariant();

            var customer =
                await _customerRepository.GetByEmailAsync(
                    normalizedEmail);

            if (customer == null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var passwordValid =
                _passwordHelper.VerifyPassword(
                    loginDto.Password,
                    customer.PasswordHash);

            if (!passwordValid)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            if (customer.Status.Equals(
                    "Deactivated",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(
                    "This account has been deactivated.");
            }

            if (!customer.EmailVerified)
            {
                throw new UnauthorizedAccessException(
                    "Please verify your email before logging in.");
            }

            var token =
                _jwtHelper.GenerateToken(customer);

            return new AuthResponseDto
            {
                CustomerId = customer.CustomerId,

                FullName =
                    $"{customer.FirstName} {customer.LastName}",

                Email = customer.Email,

                Role = customer.Role,

                Token = token
            };
        }

        // =====================================================
        // Verify Email
        // =====================================================
        public async Task<string> VerifyEmailAsync(
            string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException(
                    "Verification token is required.");
            }

            var hashedToken =
                _tokenHelper.HashToken(token);

            var customer =
                await _customerRepository
                    .GetByVerificationTokenAsync(
                        hashedToken);

            if (customer == null)
            {
                throw new InvalidOperationException(
                    "Invalid verification token.");
            }

            if (customer.EmailVerified)
            {
                throw new InvalidOperationException(
                    "Email address is already verified.");
            }

            if (customer.EmailVerificationTokenExpiresAt == null ||
                customer.EmailVerificationTokenExpiresAt <=
                    DateTime.UtcNow)
            {
                throw new InvalidOperationException(
                    "Verification token has expired.");
            }

            customer.EmailVerified = true;

            customer.EmailVerificationToken = null;
            customer.EmailVerificationTokenExpiresAt = null;

            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return "Email verified successfully. You can now log in.";
        }

        // =====================================================
        // Resend Verification
        // =====================================================
        public async Task<string> ResendVerificationAsync(
            ResendVerificationDto dto)
        {
            var normalizedEmail = dto.Email
                .Trim()
                .ToLowerInvariant();

            var customer =
                await _customerRepository.GetByEmailAsync(
                    normalizedEmail);

            if (customer == null)
            {
                throw new InvalidOperationException(
                    "Customer account was not found.");
            }

            if (customer.EmailVerified)
            {
                throw new InvalidOperationException(
                    "Email address is already verified.");
            }

            var rawVerificationToken =
                _tokenHelper.GenerateToken();

            customer.EmailVerificationToken =
                _tokenHelper.HashToken(
                    rawVerificationToken);

            customer.EmailVerificationTokenExpiresAt =
                DateTime.UtcNow.AddHours(
                    _emailSettings.VerificationExpiryHours);

            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();

            var verificationLink =
                $"{_emailSettings.ApplicationBaseUrl}" +
                $"/api/auth/verify-email?token=" +
                $"{Uri.EscapeDataString(rawVerificationToken)}";

            var emailBody = $@"
                <h2>Email Verification</h2>

                <p>Hello {customer.FirstName},</p>

                <p>
                    A new email verification link
                    has been requested.
                </p>

                <p>
                    <a href=""{verificationLink}"">
                        Verify Email
                    </a>
                </p>

                <p>
                    This link expires in
                    {_emailSettings.VerificationExpiryHours} hours.
                </p>
            ";

            await _emailService.SendEmailAsync(
                customer.Email,
                "New Email Verification Link",
                emailBody);

            return "A new verification link has been sent to your email.";
        }

        // =====================================================
        // Forgot Password
        // =====================================================
        public async Task<string> ForgotPasswordAsync(
            ForgotPasswordDto dto)
        {
            const string genericResponse =
                "If an account exists for this email, a password reset email has been sent.";

            var normalizedEmail = dto.Email
                .Trim()
                .ToLowerInvariant();

            var customer =
                await _customerRepository.GetByEmailAsync(
                    normalizedEmail);

            // Do not reveal whether account exists
            if (customer == null)
            {
                return genericResponse;
            }

            var rawResetToken =
                _tokenHelper.GenerateToken();

            customer.PasswordResetToken =
                _tokenHelper.HashToken(
                    rawResetToken);

            customer.PasswordResetTokenExpiresAt =
                DateTime.UtcNow.AddMinutes(
                    _emailSettings.PasswordResetExpiryMinutes);

            customer.UpdatedAt =
                DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();

            var emailBody = $@"
                <h2>Password Reset</h2>

                <p>Hello {customer.FirstName},</p>

                <p>
                    A password reset request was received
                    for your account.
                </p>

                <p>
                    Use the following reset token:
                </p>

                <p>
                    <strong>{rawResetToken}</strong>
                </p>

                <p>
                    This token expires in
                    {_emailSettings.PasswordResetExpiryMinutes}
                    minutes.
                </p>

                <p>
                    If you did not request this,
                    you can ignore this email.
                </p>
            ";

            await _emailService.SendEmailAsync(
                customer.Email,
                "Password Reset Request",
                emailBody);

            return genericResponse;
        }

        // =====================================================
        // Reset Password
        // =====================================================
        public async Task<string> ResetPasswordAsync(
            ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token))
            {
                throw new InvalidOperationException(
                    "Password reset token is required.");
            }

            var hashedToken =
                _tokenHelper.HashToken(dto.Token);

            var customer =
                await _customerRepository
                    .GetByPasswordResetTokenAsync(
                        hashedToken);

            if (customer == null)
            {
                throw new InvalidOperationException(
                    "Invalid password reset token.");
            }

            if (customer.PasswordResetTokenExpiresAt == null ||
                customer.PasswordResetTokenExpiresAt <=
                    DateTime.UtcNow)
            {
                customer.PasswordResetToken = null;

                customer.PasswordResetTokenExpiresAt = null;

                customer.UpdatedAt =
                    DateTime.UtcNow;

                await _customerRepository.UpdateAsync(customer);

                await _customerRepository.SaveChangesAsync();

                throw new InvalidOperationException(
                    "Password reset token has expired.");
            }

            customer.PasswordHash =
                _passwordHelper.HashPassword(
                    dto.NewPassword);

            // Single use token
            customer.PasswordResetToken = null;
            customer.PasswordResetTokenExpiresAt = null;

            customer.UpdatedAt =
                DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return "Password reset successfully. You can now log in with your new password.";
        }
    }
}