using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;

namespace Presentation.Controllers;

/// <summary>
/// Job Recommendation cho Candidate — "Việc làm phù hợp với bạn".
/// </summary>
public sealed class JobRecommendationController : Controller
{
    private readonly IJobRecommendationUseCase _recommendationUseCase;

    public JobRecommendationController(IJobRecommendationUseCase recommendationUseCase)
    {
        _recommendationUseCase = recommendationUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int limit = 10)
    {
        try
        {
            var recommendations = await _recommendationUseCase.GetRecommendedJobsAsync(limit);
            return View(recommendations);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "CandidateDashboard");
        }
    }
}
