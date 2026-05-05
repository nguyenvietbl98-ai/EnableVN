using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Presentation.ViewModels.Dashboard;

namespace Presentation.Controllers;

public sealed class AdminDashboardController : Controller
{
    private readonly IViolationReportUseCase _violationReportUseCase;
    private readonly INotificationUseCase _notificationUseCase;
    private readonly IEmployerProfileUseCase _employerProfileUseCase;

    public AdminDashboardController(
        IViolationReportUseCase violationReportUseCase,
        INotificationUseCase notificationUseCase,
        IEmployerProfileUseCase employerProfileUseCase)
    {
        _violationReportUseCase = violationReportUseCase;
        _notificationUseCase = notificationUseCase;
        _employerProfileUseCase = employerProfileUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var pendingReports = await _violationReportUseCase.GetPendingReportsAsync();
            var unreadNotifications = await _notificationUseCase.CountMyUnreadAsync();
            var pendingEmployerProfiles = await _employerProfileUseCase.GetPendingProfilesAsync();
            var employerProfilesForReview = await _employerProfileUseCase.GetProfilesForReviewAsync();

            var viewModel = new AdminDashboardViewModel
            {
                PendingReports = pendingReports.Count,
                UnreadNotifications = unreadNotifications,
                PendingEmployerProfiles = pendingEmployerProfiles,
                EmployerProfilesForReview = employerProfilesForReview
            };

            return View(viewModel);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveEmployerProfile(Guid id, string? note)
    {
        try
        {
            await _employerProfileUseCase.ApproveProfileAsync(id, note);
            TempData["Success"] = "Đã duyệt hồ sơ doanh nghiệp.";
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectEmployerProfile(Guid id, string? note)
    {
        try
        {
            await _employerProfileUseCase.RejectProfileAsync(id, note);
            TempData["Success"] = "Đã từ chối hồ sơ doanh nghiệp.";
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
