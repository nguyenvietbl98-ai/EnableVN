using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Outbound.Repositories;

namespace Presentation.Controllers;

/// <summary>
/// Chat ứng viên ↔ NTD theo từng hồ sơ ứng tuyển (SignalR + lịch sử JSON).
/// </summary>
public sealed class ApplicationChatController : Controller
{
    private readonly IJobApplicationUseCase _jobApplicationUseCase;
    private readonly IApplicationChatRepository _chatRepository;

    public ApplicationChatController(
        IJobApplicationUseCase jobApplicationUseCase,
        IApplicationChatRepository chatRepository)
    {
        _jobApplicationUseCase = jobApplicationUseCase;
        _chatRepository = chatRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Thread(Guid applicationId)
    {
        try
        {
            var info = await _jobApplicationUseCase.GetChatThreadForCurrentUserAsync(applicationId);
            return View(info);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> History(Guid applicationId)
    {
        try
        {
            await _jobApplicationUseCase.EnsureCurrentUserCanChatOnApplicationAsync(applicationId);

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdStr, out var currentUserId))
                return Unauthorized();

            var rows = await _chatRepository.ListByApplicationIdAsync(applicationId);

            return Json(
                new
                {
                    applicationId,
                    currentUserId,
                    messages = rows.Select(m => new
                    {
                        m.Id,
                        m.Body,
                        m.SenderUserId,
                        isMine = m.SenderUserId == currentUserId,
                        m.ModerationOutcome,
                        m.ModerationReasonVi,
                        sentAtUtc = m.SentAtUtc
                    })
                });
        }
        catch (UseCaseException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
