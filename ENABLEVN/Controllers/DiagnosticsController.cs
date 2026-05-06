using Application.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ports.Outbound.Services;
using System.Net.Mail;

namespace Presentation.Controllers
{
    /// <summary>
    /// Endpoint test nhanh cho SMTP/Gmail.
    /// Chỉ dùng khi Development để xác nhận email gửi được thật hay không.
    /// </summary>
    [Route("Diagnostics")]
    public sealed class DiagnosticsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;
        private readonly ILogger<DiagnosticsController> _logger;

        public DiagnosticsController(
            IWebHostEnvironment environment,
            IEmailService emailService,
            ILogger<DiagnosticsController> logger)
        {
            _environment = environment;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("SendTestEmail")]
        [ValidateAntiForgeryToken]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> SendTestEmail(
            [FromForm] string to,
            CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            if (string.IsNullOrWhiteSpace(to))
                return BadRequest(new { success = false, error = "Form field 'to' is required." });

            string normalized;
            try
            {
                normalized = new MailAddress(to.Trim()).Address;
            }
            catch (FormatException)
            {
                return BadRequest(new { success = false, error = "Invalid recipient email address." });
            }

            const string subject = "EnableVN - Test email";
            var html = EmailTemplates.RenderNotificationHtml(
                "EnableVN - Test email",
                "Nếu bạn nhận được email này nghĩa là SMTP Gmail đã hoạt động.");

            try
            {
                _logger.LogInformation("Diagnostics test email requested. To={Recipient}", normalized);

                await _emailService.SendAsync(
                    normalized,
                    subject,
                    html,
                    cancellationToken
                );

                _logger.LogInformation("Diagnostics test email sent. To={Recipient}", normalized);
                return Ok(new { success = true, to = normalized, subject });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Diagnostics test email failed. To={Recipient}", normalized);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}

