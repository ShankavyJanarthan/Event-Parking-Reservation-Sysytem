using Microsoft.AspNetCore.Identity;

namespace EventParkingReservationSystem.API.Helpers
{
    public class PasswordHelper
    {
        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordHelper()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(
                new object(),
                password);
        }

        public bool VerifyPassword(
            string password,
            string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                password);

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}