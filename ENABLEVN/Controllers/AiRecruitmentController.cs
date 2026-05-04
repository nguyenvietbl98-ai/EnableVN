using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Ai;
using Presentation.Services;

namespace Presentation.Controllers;

/// <summary>
/// API Trợ lý AI (mục 1) và bóc tách JD (mục 2): Gemini server-side, model <c>Gemini:Model</c> (mặc định gemini-flash-latest).
/// Không dùng cho chat ứng tuyển real-time (mục 3 — xem SignalR + <c>ChatModeration</c>).
/// </summary>
[Route("api/ai")]
[ApiController]
public sealed class AiRecruitmentController : ControllerBase
{
    private readonly AiRecruitmentService _ai;
    private readonly GeminiClient _gemini;

    public AiRecruitmentController(AiRecruitmentService ai, GeminiClient gemini)
    {
        _ai = ai;
        _gemini = gemini;
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new { configured = _gemini.IsConfigured });
    }

    [HttpPost("candidate-chat")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CandidateChat([FromBody] AiChatRequest request, CancellationToken ct)
    {
        if (!IsRole(UserRole.Candidate))
            return StatusCode(StatusCodes.Status403Forbidden, new { error = "Chỉ ứng viên dùng chat này." });

        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { error = "Nội dung trống." });

        if (request.Message.Length > 4000)
            return BadRequest(new { error = "Tin nhắn quá dài (tối đa 4000 ký tự)." });

        if (!_gemini.IsConfigured)
            return BadRequest(new { error = "Chưa cấu hình Gemini (ApiKey/Model). Xem appsettings hoặc user secrets." });

        try
        {
            var result = await _ai.CandidateSuggestJobsAsync(request.Message.Trim(), ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("employer-chat")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmployerChat([FromBody] AiChatRequest request, CancellationToken ct)
    {
        if (!IsRole(UserRole.Employer))
            return StatusCode(StatusCodes.Status403Forbidden, new { error = "Chỉ nhà tuyển dụng dùng chat này." });

        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { error = "Nội dung trống." });

        if (request.Message.Length > 4000)
            return BadRequest(new { error = "Tin nhắn quá dài (tối đa 4000 ký tự)." });

        if (!_gemini.IsConfigured)
            return BadRequest(new { error = "Chưa cấu hình Gemini (ApiKey/Model). Xem appsettings hoặc user secrets." });

        try
        {
            var result = await _ai.EmployerSuggestCandidatesAsync(request.Message.Trim(), ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("parse-jd")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ParseJd([FromBody] ParseJdRequest request, CancellationToken ct)
    {
        if (!IsRole(UserRole.Employer))
            return StatusCode(StatusCodes.Status403Forbidden, new { error = "Chỉ nhà tuyển dụng dùng tính năng này." });

        if (string.IsNullOrWhiteSpace(request.RawText) || request.RawText.Trim().Length < 40)
            return BadRequest(new { error = "Vui lòng dán JD đủ dài (ít nhất khoảng 40 ký tự)." });

        if (request.RawText.Length > 32000)
            return BadRequest(new { error = "JD quá dài (tối đa 32000 ký tự)." });

        if (!_gemini.IsConfigured)
            return BadRequest(new { error = "Chưa cấu hình Gemini (ApiKey/Model). Xem appsettings hoặc user secrets." });

        try
        {
            var result = await _ai.ParseJobDescriptionAsync(request.RawText.Trim(), ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private bool IsRole(UserRole role)
    {
        var r = HttpContext.Session.GetString("UserRole");
        return string.Equals(r, role.ToString(), StringComparison.Ordinal);
    }
}
