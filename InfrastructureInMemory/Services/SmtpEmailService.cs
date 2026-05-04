using Microsoft.Extensions.Configuration;
using Ports.Outbound.Services;
using System.Net;
using System.Net.Mail;

namespace InfrastructureInMemory.Services
{
    public sealed class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(
            string to,
            string subject,
            string body,
            CancellationToken cancellationToken = default)
        {
            var host = _configuration["Email:Smtp:Host"] ?? "smtp.gmail.com";
            var port = int.TryParse(_configuration["Email:Smtp:Port"], out var p) ? p : 587;
            var username = _configuration["Email:Smtp:Username"];
            var password = _configuration["Email:Smtp:Password"];

            var fromEmail = _configuration["Email:From:Email"] ?? username;
            var fromName = _configuration["Email:From:Name"] ?? "EnableVN";

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException("Thiếu cấu hình Email SMTP. Cần Email:Smtp:Username/Password và Email:From:Email.");
            }

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            message.To.Add(to);

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            // SmtpClient không hỗ trợ CancellationToken trực tiếp.
            await client.SendMailAsync(message);
        }
    }
}

