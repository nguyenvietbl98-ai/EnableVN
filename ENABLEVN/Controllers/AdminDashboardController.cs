using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Presentation.ViewModels.Dashboard;

namespace Presentation.Controllers;

public sealed class AdminDashboardController : Controller
{
    private readonly IViolationReportUseCase _violationReportUseCase;
    private readonly INotificationUseCase _notificationUseCase;

    public AdminDashboardController(
        IViolationReportUseCase violationReportUseCase,
        INotificationUseCase notificationUseCase)
    {
        _violationReportUseCase = violationReportUseCase;
        _notificationUseCase = notificationUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var pendingReports = await _violationReportUseCase.GetPendingReportsAsync();
            var unreadNotifications = await _notificationUseCase.CountMyUnreadAsync();

            var viewModel = new AdminDashboardViewModel
            {
                PendingReports = pendingReports.Count,
                UnreadNotifications = unreadNotifications
            };

            return View(viewModel);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }
}
