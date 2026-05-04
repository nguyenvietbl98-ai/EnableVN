using Application.Common;
using Domain.Applications;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Applications;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controller quản lý hồ sơ ứng tuyển.
    /// 
    /// Candidate:
    /// - Xem hồ sơ đã nộp
    /// - Rút hồ sơ
    /// 
    /// Employer:
    /// - Xem danh sách hồ sơ theo job
    /// - Đổi trạng thái hồ sơ
    /// </summary>
    public sealed class JobApplicationsController : Controller
    {
        private readonly IJobApplicationUseCase _jobApplicationUseCase;

        public JobApplicationsController(IJobApplicationUseCase jobApplicationUseCase)
        {
            _jobApplicationUseCase = jobApplicationUseCase;
        }

        /// <summary>
        /// Candidate xem danh sách hồ sơ mình đã nộp.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyApplications()
        {
            try
            {
                var applications = await _jobApplicationUseCase.GetMyApplicationsAsync();

                return View(applications);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Employer xem danh sách hồ sơ nộp vào một job.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ByJob(Guid jobId)
        {
            try
            {
                var applications = await _jobApplicationUseCase.GetByJobIdAsync(jobId);

                ViewBag.JobId = jobId;

                return View(applications);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "EmployerJobs");
            }
        }

        /// <summary>
        /// Employer đổi trạng thái hồ sơ.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(
            Guid applicationId,
            Guid jobId,
            ApplicationStatus newStatus,
            string? note
        )
        {
            try
            {
                await _jobApplicationUseCase.ChangeStatusAsync(
                    new ChangeApplicationStatusCommand
                    {
                        ApplicationId = applicationId,
                        NewStatus = newStatus,
                        Note = note
                    }
                );

                TempData["Success"] = "Đã cập nhật trạng thái hồ sơ.";
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(ByJob), new { jobId });
        }

        /// <summary>
        /// Candidate rút hồ sơ đã nộp.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(Guid applicationId)
        {
            try
            {
                await _jobApplicationUseCase.WithdrawAsync(applicationId);

                TempData["Success"] = "Đã rút hồ sơ ứng tuyển.";
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(MyApplications));
        }
    }
}
