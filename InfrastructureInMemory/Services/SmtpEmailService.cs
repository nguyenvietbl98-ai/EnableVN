using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ports.Outbound.Services;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace InfrastructureInMemory.Services
{
    public sealed class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(
            IConfiguration configuration,
            ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendAsync(
            string to,
            string subject,
            string body,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email is required.", nameof(to));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject is required.", nameof(subject));

            var normalizedTo = ValidateAndNormalizeEmail(to, paramName: nameof(to));

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
                _logger.LogError(
                    "Thiếu cấu hình SMTP. Required keys: Email:Smtp:Username, Email:Smtp:Password, Email:From:Email.");
                throw new InvalidOperationException("Thiếu cấu hình SMTP");
            }

            var normalizedFrom = ValidateAndNormalizeEmail(fromEmail, paramName: "Email:From:Email");

            using var message = new MailMessage
            {
                From = new MailAddress(normalizedFrom, fromName, Encoding.UTF8),
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                Body = body ?? string.Empty,
                BodyEncoding = Encoding.UTF8,
                HeadersEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
            message.To.Add(normalizedTo);

            using var client = new SmtpClient(host, port)
            {
                // Gmail SMTP: smtp.gmail.com:587 STARTTLS
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password),
                Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds
            };

            var sw = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation(
                    "Sending email. To={Recipient} Subject={Subject} Host={Host} Port={Port}",
                    normalizedTo,
                    subject,
                    host,
                    port);

                // SmtpClient không hỗ trợ CancellationToken trực tiếp.
                // Best-effort cancellation: nếu request bị hủy, vẫn để SMTP call hoàn tất.
                await client.SendMailAsync(message);

                _logger.LogInformation(
                    "Email sent successfully. To={Recipient} Subject={Subject} ElapsedMs={ElapsedMs}",
                    normalizedTo,
                    subject,
                    sw.ElapsedMilliseconds);
            }
            catch (SmtpException ex)
            {
                if (IsAuthenticationFailure(ex))
                {
                    _logger.LogError(
                        ex,
                        "Authentication failed. To={Recipient} Subject={Subject}",
                        normalizedTo,
                        subject);
                }
                else if (IsTimeout(ex))
                {
                    _logger.LogError(
                        ex,
                        "SMTP timeout. To={Recipient} Subject={Subject} Host={Host} Port={Port}",
                        normalizedTo,
                        subject,
                        host,
                        port);
                }
                else
                {
                    _logger.LogError(
                        ex,
                        "SMTP send failed. To={Recipient} Subject={Subject} StatusCode={StatusCode}",
                        normalizedTo,
                        subject,
                        ex.StatusCode);
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Email send failed. To={Recipient} Subject={Subject}",
                    normalizedTo,
                    subject);
                throw;
            }
        }

        private static string ValidateAndNormalizeEmail(string email, string paramName)
        {
            try
            {
                var addr = new MailAddress(email.Trim());
                return addr.Address;
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Invalid recipient email address.", paramName, ex);
            }
        }

        private static bool IsAuthenticationFailure(SmtpException ex)
        {
            var msg = ex.Message ?? string.Empty;
            return msg.Contains("authentication", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("5.7.8", StringComparison.OrdinalIgnoreCase); // Gmail common auth error code
        }

        private static bool IsTimeout(SmtpException ex)
        {
            var msg = ex.Message ?? string.Empty;
            return msg.Contains("timed out", StringComparison.OrdinalIgnoreCase)
                || msg.Contains("timeout", StringComparison.OrdinalIgnoreCase);
        }
    }
}

