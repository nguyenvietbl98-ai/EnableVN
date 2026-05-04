using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Reviews;
using Ports.Outbound.Services;
using Application.Common;

namespace Presentation.Controllers
{
    public sealed class CompanyReviewsController : Controller
    {
        private readonly ICompanyReviewUseCase _companyReviewUseCase;
        private readonly ICurrentUserService _currentUserService;

        public CompanyReviewsController(ICompanyReviewUseCase companyReviewUseCase, ICurrentUserService currentUserService)
        {
            _companyReviewUseCase = companyReviewUseCase;
            _currentUserService = currentUserService;
        }

        // Ứng viên gửi đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyReviewCommand command)
        {
            try
            {
                if (!_currentUserService.UserId.HasValue)
                {
                    TempData["Error"] = "Vui lòng đăng nhập để gửi đánh giá.";
                    return RedirectToAction("Login", "Auth");
                }

                command.CandidateId = _currentUserService.UserId.Value;
                await _companyReviewUseCase.CreateAsync(command);

                TempData["Success"] = "Đánh giá doanh nghiệp thành công.";
                return RedirectToAction("Details", "EmployerProfile", new { id = command.EmployerId });
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "EmployerProfile", new { id = command.EmployerId });
            }
        }
    }
}