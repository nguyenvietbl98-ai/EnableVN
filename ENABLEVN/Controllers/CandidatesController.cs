using Application.Common;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Candidates;

namespace Presentation.Controllers
{
    // Controller này phục vụ Employer tìm ứng viên public.
    public sealed class CandidatesController : Controller
    {
        private readonly IEmployerCandidateSearchUseCase _searchUseCase;

        public CandidatesController(IEmployerCandidateSearchUseCase searchUseCase)
        {
            _searchUseCase = searchUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SearchPublicCandidatesQuery query)
        {
            if (!string.Equals(
                    HttpContext.Session.GetString("UserRole"),
                    nameof(UserRole.Employer),
                    StringComparison.Ordinal))
            {
                TempData["Error"] = "Chỉ nhà tuyển dụng mới truy cập được trang tìm ứng viên.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var candidates = await _searchUseCase.SearchAsync(query);

                ViewBag.Query = query;

                return View(candidates);
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
            if (!string.Equals(
                    HttpContext.Session.GetString("UserRole"),
                    nameof(UserRole.Employer),
                    StringComparison.Ordinal))
            {
                TempData["Error"] = "Chỉ nhà tuyển dụng mới xem được hồ sơ ứng viên.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var profile = await _searchUseCase.GetPublicProfileByIdAsync(id);
                if (profile is null)
                    return NotFound();

                return View(profile);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
