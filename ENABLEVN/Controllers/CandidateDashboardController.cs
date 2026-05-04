using Application.Common;
using Domain.Applications;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Presentation.ViewModels.Dashboard;

namespace Presentation.Controllers;

public sealed class CandidateDashboardController : Controller
{
    private readonly IJobApplicationUseCase _jobApplicationUseCase;
    private readonly INotificationUseCase _notificationUseCase;

    public CandidateDashboardController(
        IJobApplicationUseCase jobApplicationUseCase,
        INotificationUseCase notificationUseCase)
    {
        _jobApplicationUseCase = jobApplicationUseCase;
        _notificationUseCase = notificationUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var myApplications = await _jobApplicationUseCase.GetMyApplicationsAsync();
            var unreadNotifications = await _notificationUseCase.CountMyUnreadAsync();

            var viewModel = new CandidateDashboardViewModel
            {
                TotalApplications = myApplications.Count,
                InterviewApplications = myApplications.Count(a => a.Status == ApplicationStatus.Interview),
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
