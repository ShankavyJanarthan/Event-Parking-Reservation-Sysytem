namespace EventParkingReservationSystem.API.Configuration
{
    public class EmailSettings
    {
        public int VerificationExpiryHours { get; set; } = 24;

        public int PasswordResetExpiryMinutes { get; set; } = 60;

        public string SmtpHost { get; set; } = string.Empty;

        public int SmtpPort { get; set; } = 587;

        public string SenderName { get; set; } = string.Empty;

        public string SenderEmail { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ApplicationBaseUrl { get; set; } = string.Empty;
    }
}