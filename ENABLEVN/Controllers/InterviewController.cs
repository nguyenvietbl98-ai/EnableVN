using Application.Common;
using Domain.Common;
using Domain.Interviews;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Interviews;

namespace Presentation.Controllers;

/// <summary>
/// Lịch phỏng vấn — Employer tạo, Candidate xác nhận/từ chối.
/// </summary>
public sealed class InterviewController : Controller
{
    private readonly IInterviewScheduleUseCase _interviewUseCase;

    public InterviewController(IInterviewScheduleUseCase interviewUseCase)
    {
        _interviewUseCase = interviewUseCase;
    }

    // ───────── Candidate ─────────

    [HttpGet]
    public async Task<IActionResult> MyInterviews()
    {
        try
        {
            var list = await _interviewUseCase.GetMyInterviewsAsCandidateAsync();
            return View(list);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "CandidateDashboard");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(Guid id)
    {
        try
        {
            await _interviewUseCase.AcceptInterviewAsync(id);
            TempData["Success"] = "Đã xác nhận tham gia lịch phỏng vấn.";
        }
        catch (Exception ex) when (ex is UseCaseException or DomainException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(MyInterviews));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decline(Guid id, string? reason)
    {
        try
        {
            await _interviewUseCase.DeclineInterviewAsync(id, reason);
            TempData["Success"] = "Đã từ chối lịch phỏng vấn.";
        }
        catch (Exception ex) when (ex is UseCaseException or DomainException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(MyInterviews));
    }

    // ───────── Employer ─────────

    [HttpGet]
    public async Task<IActionResult> EmployerInterviews()
    {
        try
        {
            var list = await _interviewUseCase.GetMyInterviewsAsEmployerAsync();
            return View(list);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "EmployerDashboard");
        }
    }

    [HttpGet]
    public IActionResult Create(Guid applicationId)
    {
        if (!_environment_IsDevelopment_Guard())
        {
            // Bất kỳ môi trường nào — route đã được define, access control ở UseCase
        }

        ViewBag.ApplicationId = applicationId;
        return View(new CreateInterviewScheduleRequest
        {
            JobApplicationId = applicationId,
            ScheduledAt = DateTime.Now.AddDays(3),
            DurationMinutes = 60,
            InterviewType = InterviewType.Online
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInterviewScheduleRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ApplicationId = request.JobApplicationId;
            return View(request);
        }

        try
        {
            await _interviewUseCase.CreateInterviewScheduleAsync(request);
            TempData["Success"] = "Đã tạo lịch phỏng vấn. Ứng viên sẽ nhận thông báo.";
            return RedirectToAction(nameof(EmployerInterviews));
        }
        catch (Exception ex) when (ex is UseCaseException or DomainException)
        {
            TempData["Error"] = ex.Message;
            ViewBag.ApplicationId = request.JobApplicationId;
            return View(request);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string? reason)
    {
        try
        {
            await _interviewUseCase.CancelInterviewAsync(id, reason);
            TempData["Success"] = "Đã hủy lịch phỏng vấn.";
        }
        catch (Exception ex) when (ex is UseCaseException or DomainException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(EmployerInterviews));
    }

    // dummy guard — kiểm tra trực tiếp không cần HttpContext ở level này
    private static bool _environment_IsDevelopment_Guard() => true;
}
