using Application.Common;
using Domain.Jobs;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Presentation.ViewModels.Dashboard;

namespace Presentation.Controllers;

public sealed class EmployerDashboardController : Controller
{
    private readonly IJobUseCase _jobUseCase;
    private readonly IJobApplicationUseCase _jobApplicationUseCase;
    private readonly INotificationUseCase _notificationUseCase;

    public EmployerDashboardController(
        IJobUseCase jobUseCase,
        IJobApplicationUseCase jobApplicationUseCase,
        INotificationUseCase notificationUseCase)
    {
        _jobUseCase = jobUseCase;
        _jobApplicationUseCase = jobApplicationUseCase;
        _notificationUseCase = notificationUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var jobs = await _jobUseCase.GetMyJobsAsync();
            var unreadNotifications = await _notificationUseCase.CountMyUnreadAsync();

            var totalApplications = 0;
            foreach (var job in jobs)
            {
                var applications = await _jobApplicationUseCase.GetByJobIdAsync(job.Id);
                totalApplications += applications.Count;
            }

            var viewModel = new EmployerDashboardViewModel
            {
                TotalJobs = jobs.Count,
                OpenJobs = jobs.Count(j => j.Status == JobStatus.Published),
                TotalApplications = totalApplications,
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
