namespace Presentation.Controllers;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller trang chủ.
/// Không chứa nghiệp vụ, chỉ trả view.
/// </summary>
public sealed class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}