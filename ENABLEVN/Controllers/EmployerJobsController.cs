using Application.Common;
using Domain.Common;
using Domain.Jobs;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Jobs;

namespace Presentation.Controllers;

/// <summary>
/// Controller cho Employer quản lý tin tuyển dụng của mình.
/// 
/// Controller này chỉ gọi IJobUseCase.
/// Không gọi Repository trực tiếp để giữ đúng Hexagonal Architecture.
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
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var job = await _jobUseCase.GetByIdAsync(id);

            if (job is null)
                return NotFound();

            return View(job);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Details), new { id = jobId });
        }
        catch (Exception ex) when (ex is UseCaseException or DomainException)
        {
            TempData["Error"] = ex.Message;
            return View(command);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var job = await _jobUseCase.GetByIdAsync(id);

            if (job is null)
                return NotFound();

            var command = new UpdateJobCommand
            {
                JobId = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirement = job.Requirement,
                WorkMode = job.WorkMode,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                SupportsWheelchairAccess = job.SupportsWheelchairAccess,
                SupportsRemoteWork = job.SupportsRemoteWork,
                SupportsFlexibleTime = job.SupportsFlexibleTime,
                ProvidesAssistiveDevices = job.ProvidesAssistiveDevices,
                AdditionalSupportDescription = job.AdditionalSupportDescription
            };

            return View(command);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateJobCommand command)
    {
        try
        {
            await _jobUseCase.UpdateAsync(command);

            TempData["Success"] = "Đã cập nhật tin tuyển dụng.";
            return RedirectToAction(nameof(Details), new { id = command.JobId });
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

        return RedirectToAction(nameof(Index));
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

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _jobUseCase.DeleteAsync(id);
            TempData["Success"] = "Đã xóa tin tuyển dụng.";
        }
        catch (Exception ex) when (ex is UseCaseException or DomainException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}