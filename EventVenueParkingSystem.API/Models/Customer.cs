namespace EventParkingReservationSystem.API.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        // Customer / Admin
        public string Role { get; set; } = "Customer";

        // Active / Deactivated
        public string Status { get; set; } = "Active";

        public bool EmailVerified { get; set; } = false;

        // Store only HASHED verification token
        public string? EmailVerificationToken { get; set; }

        public DateTime? EmailVerificationTokenExpiresAt { get; set; }

        // Store only HASHED password reset token
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}