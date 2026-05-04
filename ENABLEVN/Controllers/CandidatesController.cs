using Application.Common;
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
    }
}
