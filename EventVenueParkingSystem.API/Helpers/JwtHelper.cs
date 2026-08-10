using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventParkingReservationSystem.API.Configuration;
using EventParkingReservationSystem.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EventParkingReservationSystem.API.Helpers
{
    public class JwtHelper
    {
        private readonly JwtSettings _jwtSettings;

        public JwtHelper(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        public string GenerateToken(Customer customer)
        {
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    customer.CustomerId.ToString()),

                new Claim(
                    ClaimTypes.Email,
                    customer.Email),

                new Claim(
                    ClaimTypes.Name,
                    $"{customer.FirstName} {customer.LastName}"),

                new Claim(
                    ClaimTypes.Role,
                    customer.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _jwtSettings.ExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}