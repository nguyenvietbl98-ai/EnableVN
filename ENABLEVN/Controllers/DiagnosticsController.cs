using Microsoft.AspNetCore.Mvc;
using Ports.Outbound.Services;

namespace Presentation.Controllers
{
    /// <summary>
    /// Endpoint test nhanh cho SMTP/Gmail.
    /// Chỉ dùng khi Development để xác nhận email gửi được thật hay không.
    /// </summary>
    public sealed class DiagnosticsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;

        public DiagnosticsController(
            IWebHostEnvironment environment,
            IEmailService emailService)
        {
            _environment = environment;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> SendTestEmail(
            string to,
            CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            if (string.IsNullOrWhiteSpace(to))
                return BadRequest("Query param 'to' is required.");

            try
            {
                await _emailService.SendAsync(
                    to,
                    "EnableVN - Test email",
                    "Nếu bạn nhận được email này nghĩa là SMTP Gmail đã hoạt động.",
                    cancellationToken
                );

                return Content($"OK. Sent test email to: {to}");
            }
            catch (Exception ex)
            {
                return Content($"FAILED: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}

