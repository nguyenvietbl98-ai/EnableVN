using Application.Common;
using Domain.Common;
using Domain.Jobs;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Jobs;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controller cho Employer quản lý tin tuyển dụng của mình.
    /// </summary>
    public sealed class EmployerJobsController : Controller
    {
        private readonly IJobUseCase _jobUseCase;

        public EmployerJobsController(IJobUseCase jobUseCase)
        {
            _jobUseCase = jobUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var jobs = await _jobUseCase.GetMyJobsAsync();

                return View(jobs);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateJobCommand
            {
                WorkMode = WorkMode.Remote
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJobCommand command)
        {
            try
            {
                var jobId = await _jobUseCase.CreateDraftAsync(command);

                TempData["Success"] = "Đã tạo tin tuyển dụng dạng nháp.";

                return RedirectToAction("Index");
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                return View(command);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(Guid id)
        {
            try
            {
                await _jobUseCase.PublishAsync(id);

                TempData["Success"] = "Đã đăng tin tuyển dụng.";
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(Guid id)
        {
            try
            {
                await _jobUseCase.CloseAsync(id);

                TempData["Success"] = "Đã đóng tin tuyển dụng.";
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
