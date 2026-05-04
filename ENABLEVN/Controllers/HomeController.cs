namespace Presentation.Controllers;

using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Jobs;
using Presentation.ViewModels.Home;

/// <summary>
/// Controller trang chủ.
/// Không chứa nghiệp vụ, chỉ trả view.
/// </summary>
public sealed class HomeController : Controller
{
    private readonly IJobUseCase _jobUseCase;

    public HomeController(IJobUseCase jobUseCase)
    {
        _jobUseCase = jobUseCase;
    }

    public async Task<IActionResult> Index()
    {
        var jobs = await _jobUseCase.SearchPublishedJobsAsync(new SearchJobQuery());
        var viewModel = new HomeIndexViewModel
        {
            LatestJobs = jobs.Take(6).ToList()
        };

        return View(viewModel);
    }

    public IActionResult Error()
    {
        return View();
    }
}