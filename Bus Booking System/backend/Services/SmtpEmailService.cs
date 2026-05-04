using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace backend.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;

        public SmtpEmailService(IConfiguration config)
        {
            _smtpHost = config["Email:Host"] ?? "smtp.ethereal.email";
            _smtpPort = int.TryParse(config["Email:Port"], out int p) ? p : 587;
            _smtpUser = config["Email:User"] ?? "test@ethereal.email";
            _smtpPass = config["Email:Pass"] ?? "testpass";
            _fromEmail = config["Email:From"] ?? "noreply@busbooking.com";
        }

        public async Task SendBookingConfirmationAsync(string toEmail, string userName, string bookingId, string routeDetails, string seatDetails)
        {
            var subject = $"Bus Booking Confirmation - {bookingId}";
            var body = $@"
                <h2>Hello {userName},</h2>
                <p>Your bus booking is confirmed!</p>
                <p><strong>Booking ID:</strong> {bookingId}</p>
                <p><strong>Route:</strong> {routeDetails}</p>
                <p><strong>Seats:</strong> {seatDetails}</p>
                <p>Thank you for traveling with us!</p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendBookingCancellationAsync(string toEmail, string userName, string bookingId, decimal refundAmount)
        {
            var subject = $"Bus Booking Cancelled - {bookingId}";
            var body = $@"
                <h2>Hello {userName},</h2>
                <p>Your bus booking ({bookingId}) has been cancelled.</p>
                <p><strong>Refund Amount:</strong> ${refundAmount:F2}</p>
                <p>We hope to see you again soon.</p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string to, string subject, string bodyHtml)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Bus Booking", _fromEmail));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = bodyHtml };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                
                // Only authenticate if user/pass are provided (some local SMTP servers don't need it)
                if (!string.IsNullOrEmpty(_smtpUser) && _smtpUser != "test@ethereal.email")
                {
                    await smtp.AuthenticateAsync(_smtpUser, _smtpPass);
                }
                
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                Console.WriteLine($"[SmtpEmailService] Sent email to {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SmtpEmailService] Failed to send email to {to}: {ex.Message}");
            }
        }
    }
}
