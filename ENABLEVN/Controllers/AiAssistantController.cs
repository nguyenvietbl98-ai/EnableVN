using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// Trang trợ lý AI (ứng viên / nhà tuyển dụng) — giao diện full page, gọi API /api/ai/*.
/// </summary>
public sealed class AiAssistantController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != nameof(UserRole.Candidate) && role != nameof(UserRole.Employer))
        {
            TempData["Error"] = "Chỉ ứng viên hoặc nhà tuyển dụng mới dùng Trợ lý AI.";
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Role = role;
        return View();
    }
}
