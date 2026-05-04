using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Reviews;
using Ports.Outbound.Services;
using Application.Common;

namespace ENABLEVN.Controllers
{
    [Authorize]
    public class CompanyReviewsController : Controller
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
        [Authorize(Roles = "Candidate")]
      
        public async Task<IActionResult> Create(CreateCompanyReviewCommand command)
        {
            try
            {
                // Truyền chính xác ID người dùng hiện tại vào Command
                command.CandidateId = _currentUserService.UserId.Value;

                // Đã sửa tên hàm thành CreateAsync để khớp với ICompanyReviewUseCase
                await _companyReviewUseCase.CreateAsync(command);

                TempData["SuccessMessage"] = "Đánh giá doanh nghiệp thành công.";
                return RedirectToAction("Details", "EmployerProfile", new { id = command.EmployerId });
            }
            catch (UseCaseException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "EmployerProfile", new { id = command.EmployerId });
            }
        }
    
}
}