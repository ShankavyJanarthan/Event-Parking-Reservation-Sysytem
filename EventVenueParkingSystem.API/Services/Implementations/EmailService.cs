using EventParkingReservationSystem.API.Configuration;
using EventParkingReservationSystem.API.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(
            IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody)
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _emailSettings.SenderName,
                    _emailSettings.SenderEmail));

            message.To.Add(
                MailboxAddress.Parse(toEmail));

            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlBody
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _emailSettings.SmtpHost,
                _emailSettings.SmtpPort,
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _emailSettings.Username,
                _emailSettings.Password);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}