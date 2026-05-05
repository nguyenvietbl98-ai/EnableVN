using Application.Common;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Employers;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controller cho nhà tuyển dụng quản lý hồ sơ doanh nghiệp.
    /// 
    /// Controller chỉ nhận request/form và gọi IEmployerProfileUseCase.
    /// Không gọi Repository trực tiếp.
    /// Không chứa business logic nặng.
    /// </summary>
    public sealed class EmployerProfileController : Controller
    {
        private readonly IEmployerProfileUseCase _employerProfileUseCase;
        private readonly ICompanyReviewUseCase _companyReviewUseCase;

        public EmployerProfileController(
            IEmployerProfileUseCase employerProfileUseCase,
            ICompanyReviewUseCase companyReviewUseCase)
        {
            _employerProfileUseCase = employerProfileUseCase;
            _companyReviewUseCase = companyReviewUseCase;
        }

        /// <summary>
        /// Trang hồ sơ doanh nghiệp của Employer hiện tại.
        /// Nếu chưa có profile thì điều hướng sang trang Create.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var profile = await _employerProfileUseCase.GetMyProfileAsync();

                if (profile is null)
                    return RedirectToAction(nameof(Create));

                return View(profile);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Form tạo hồ sơ doanh nghiệp.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateEmployerProfileCommand());
        }

        /// <summary>
        /// Xử lý tạo hồ sơ doanh nghiệp.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployerProfileCommand command, IFormFile? logoFile)
        {
            try
            {
                var logoUrl = await SaveImageFileAsync(logoFile, "employer-logos");
                command = new CreateEmployerProfileCommand
                {
                    CompanyName = command.CompanyName,
                    LogoUrl = logoUrl ?? command.LogoUrl,
                    ContactEmail = command.ContactEmail,
                    PhoneNumber = command.PhoneNumber,
                    Address = command.Address,
                    CompanySize = command.CompanySize,
                    Industry = command.Industry,
                    TaxCode = command.TaxCode,
                    RecruiterContactName = command.RecruiterContactName,
                    RecruiterContactTitle = command.RecruiterContactTitle,
                    Description = command.Description,
                    Benefits = command.Benefits,
                    Culture = command.Culture,
                    WebsiteUrl = command.WebsiteUrl,
                    HasWheelchairAccess = command.HasWheelchairAccess,
                    HasAccessibleRestroom = command.HasAccessibleRestroom,
                    SupportsFlexibleWorkingTime = command.SupportsFlexibleWorkingTime,
                    SupportsRemoteWork = command.SupportsRemoteWork,
                    ProvidesAssistiveDevices = command.ProvidesAssistiveDevices
                };
                await _employerProfileUseCase.CreateAsync(command);

                TempData["Success"] = "Đã tạo hồ sơ doanh nghiệp.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                return View(command);
            }
        }

        /// <summary>
        /// Form cập nhật hồ sơ doanh nghiệp.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var profile = await _employerProfileUseCase.GetMyProfileAsync();

                if (profile is null)
                {
                    TempData["Error"] = "Bạn chưa có hồ sơ doanh nghiệp.";
                    return RedirectToAction(nameof(Create));
                }

                var command = new UpdateEmployerProfileCommand
                {
                    CompanyName = profile.CompanyName,
                    LogoUrl = profile.LogoUrl,
                    ContactEmail = profile.ContactEmail,
                    PhoneNumber = profile.PhoneNumber,
                    Address = profile.Address,
                    CompanySize = profile.CompanySize,
                    Industry = profile.Industry,
                    TaxCode = profile.TaxCode,
                    RecruiterContactName = profile.RecruiterContactName,
                    RecruiterContactTitle = profile.RecruiterContactTitle,
                    Description = profile.Description,
                    Benefits = profile.Benefits,
                    Culture = profile.Culture,
                    WebsiteUrl = profile.WebsiteUrl,
                    HasWheelchairAccess = profile.HasWheelchairAccess,
                    HasAccessibleRestroom = profile.HasAccessibleRestroom,
                    SupportsFlexibleWorkingTime = profile.SupportsFlexibleWorkingTime,
                    SupportsRemoteWork = profile.SupportsRemoteWork,
                    ProvidesAssistiveDevices = profile.ProvidesAssistiveDevices
                };

                return View(command);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Xử lý cập nhật hồ sơ doanh nghiệp.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEmployerProfileCommand command, IFormFile? logoFile)
        {
            try
            {
                var logoUrl = await SaveImageFileAsync(logoFile, "employer-logos");
                command = new UpdateEmployerProfileCommand
                {
                    CompanyName = command.CompanyName,
                    LogoUrl = logoUrl ?? command.LogoUrl,
                    ContactEmail = command.ContactEmail,
                    PhoneNumber = command.PhoneNumber,
                    Address = command.Address,
                    CompanySize = command.CompanySize,
                    Industry = command.Industry,
                    TaxCode = command.TaxCode,
                    RecruiterContactName = command.RecruiterContactName,
                    RecruiterContactTitle = command.RecruiterContactTitle,
                    Description = command.Description,
                    Benefits = command.Benefits,
                    Culture = command.Culture,
                    WebsiteUrl = command.WebsiteUrl,
                    HasWheelchairAccess = command.HasWheelchairAccess,
                    HasAccessibleRestroom = command.HasAccessibleRestroom,
                    SupportsFlexibleWorkingTime = command.SupportsFlexibleWorkingTime,
                    SupportsRemoteWork = command.SupportsRemoteWork,
                    ProvidesAssistiveDevices = command.ProvidesAssistiveDevices
                };
                await _employerProfileUseCase.UpdateMyProfileAsync(command);

                TempData["Success"] = "Đã cập nhật hồ sơ doanh nghiệp.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                return View(command);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var profile = await _employerProfileUseCase.GetByIdAsync(id);
                if (profile is null)
                    return NotFound();

                var reviews = await _companyReviewUseCase.GetByEmployerIdAsync(id);
                ViewBag.Reviews = reviews;

                return View(profile);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        private const long MaxImageSizeBytes = 2 * 1024 * 1024;
        private async Task<string?> SaveImageFileAsync(IFormFile? imageFile, string folder)
        {
            if (imageFile is null || imageFile.Length == 0) return null;
            if (imageFile.Length > MaxImageSizeBytes) throw new UseCaseException("Ảnh tối đa 2MB.");
            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext)) throw new UseCaseException("Ảnh chỉ hỗ trợ JPG, PNG, WEBP.");
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folder);
            Directory.CreateDirectory(uploadsRoot);
            var safeName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsRoot, safeName);
            await using var stream = System.IO.File.Create(fullPath);
            await imageFile.CopyToAsync(stream);
            return $"/uploads/{folder}/{safeName}";
        }
    }
}
