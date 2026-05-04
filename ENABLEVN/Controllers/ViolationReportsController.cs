using Application.Common;
using Domain.Common;
using Domain.Reports;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Reports;

namespace Presentation.Controllers;

public sealed class ViolationReportsController : Controller
{
    private readonly IViolationReportUseCase _violationReportUseCase;

    public ViolationReportsController(IViolationReportUseCase violationReportUseCase)
    {
        _violationReportUseCase = violationReportUseCase;
    }

    private bool IsAdmin() =>
        string.Equals(
            HttpContext.Session.GetString("UserRole"),
            nameof(UserRole.Admin),
            StringComparison.Ordinal);

    private bool IsLoggedIn() =>
        !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserId"));

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!IsAdmin())
        {
            TempData["Error"] = "Bạn không có quyền truy cập trang quản trị báo cáo.";
            return RedirectToAction("Index", "Home");
        }

        try
        {
            var reports = await _violationReportUseCase.GetPendingReportsAsync();
            return View(reports);
        }
        catch (UseCaseException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public IActionResult Create(ReportTargetType targetType, Guid targetId)
    {
        if (!IsLoggedIn())
        {
            TempData["Error"] = "Vui lòng đăng nhập để gửi báo cáo.";
            return RedirectToAction("Login", "Auth");
        }

        return View(new CreateViolationReportCommand
        {
            TargetType = targetType,
            TargetId = targetId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateViolationReportCommand command)
    {
        if (!IsLoggedIn())
        {
            TempData["Error"] = "Vui lòng đăng nhập để gửi báo cáo.";
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            await _violationReportUseCase.CreateAsync(command);
            TempData["Success"] = "Báo cáo đã được gửi tới quản trị viên.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex) when (ex is UseCaseException or DomainException)
        {
            TempData["Error"] = ex.Message;
            return View(command);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resolve(HandleViolationReportCommand command)
    {
        if (!IsAdmin())
        {
            TempData["Error"] = "Bạn không có quyền xử lý báo cáo.";
            return RedirectToAction("Index", "Home");
        }

        try
        {
            await _violationReportUseCase.ResolveAsync(command);
            TempData["Success"] = "Đã xử lý báo cáo.";
        }
        catch (Exception ex) when (ex is UseCaseException or DomainException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(HandleViolationReportCommand command)
    {
        if (!IsAdmin())
        {
            TempData["Error"] = "Bạn không có quyền xử lý báo cáo.";
            return RedirectToAction("Index", "Home");
        }

        try
        {
            await _violationReportUseCase.RejectAsync(command);
            TempData["Success"] = "Đã từ chối báo cáo.";
        }
        catch (Exception ex) when (ex is UseCaseException or DomainException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
