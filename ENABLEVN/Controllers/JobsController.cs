using Application.Common;
using Domain.Common;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Applications;
using Ports.Models.Jobs;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controller public cho danh sách và chi tiết việc làm.
    /// Candidate hoặc guest đều có thể xem.
    /// </summary>
    public sealed class JobsController : Controller
    {
        private readonly IJobUseCase _jobUseCase;
        private readonly IJobApplicationUseCase _jobApplicationUseCase;

        public JobsController(
            IJobUseCase jobUseCase,
            IJobApplicationUseCase jobApplicationUseCase
        )
        {
            _jobUseCase = jobUseCase;
            _jobApplicationUseCase = jobApplicationUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] SearchJobQuery query)
        {
            var jobs = await _jobUseCase.SearchPublishedJobsAsync(query);

            ViewBag.Query = query;

            return View(jobs);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var job = await _jobUseCase.GetByIdAsync(id);

            if (job is null)
                return NotFound();

            Guid? myApplicationIdForChat = null;
            var role = HttpContext.Session.GetString("UserRole");
            if (role == nameof(UserRole.Candidate))
            {
                try
                {
                    myApplicationIdForChat =
                        await _jobApplicationUseCase.TryGetCurrentCandidateApplicationIdForJobAsync(id);
                }
                catch (UseCaseException)
                {
                    myApplicationIdForChat = null;
                }
            }

            ViewBag.MyApplicationIdForChat = myApplicationIdForChat;

            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(Guid jobId)
        {
            try
            {
                await _jobApplicationUseCase.SubmitAsync(
                    new SubmitJobApplicationCommand
                    {
                        JobId = jobId
                    }
                );

                TempData["Success"] = "Bạn đã nộp hồ sơ thành công.";

                return RedirectToAction("Details", new { id = jobId });
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction("Details", new { id = jobId });
            }
        }
    }
}
