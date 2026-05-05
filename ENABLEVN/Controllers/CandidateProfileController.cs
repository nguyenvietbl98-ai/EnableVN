using Application.Common;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Candidates;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controller cho ứng viên quản lý hồ sơ cá nhân.
    /// 
    /// Bao gồm:
    /// - Tạo hồ sơ
    /// - Cập nhật thông tin cơ bản
    /// - Cập nhật thông tin khuyết tật
    /// - Ẩn/hiện thông tin khuyết tật
    /// - Public/private profile
    /// </summary>
    public sealed class CandidateProfileController : Controller
    {
        private readonly ICandidateProfileUseCase _candidateProfileUseCase;

        private readonly ICatalogUseCase _catalogUseCase;

        public CandidateProfileController(
            ICandidateProfileUseCase candidateProfileUseCase,
            ICatalogUseCase catalogUseCase
        )
        {
            _candidateProfileUseCase = candidateProfileUseCase;
            _catalogUseCase = catalogUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var profile = await _candidateProfileUseCase.GetMyProfileAsync();

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

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCandidateProfileCommand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCandidateProfileCommand command, IFormFile? cvFile, IFormFile? avatarFile)
        {
            try
            {
                var cvUrl = await SaveCvFileAsync(cvFile);
                var avatarUrl = await SaveImageFileAsync(avatarFile, "avatars");

                command = new CreateCandidateProfileCommand
                {
                    FullName = command.FullName,
                    AvatarUrl = avatarUrl,
                    DateOfBirth = command.DateOfBirth,
                    Gender = command.Gender,
                    PhoneNumber = command.PhoneNumber,
                    ContactEmail = command.ContactEmail,
                    Address = command.Address,
                    DesiredPosition = command.DesiredPosition,
                    DesiredSalary = command.DesiredSalary,
                    ExperienceSummary = command.ExperienceSummary,
                    Skills = command.Skills,
                    Education = command.Education,
                    Certifications = command.Certifications,
                    PortfolioUrl = command.PortfolioUrl,
                    Bio = command.Bio,
                    CvUrl = cvUrl,
                    JobSeekingStatus = command.JobSeekingStatus,
                    DesiredWorkMode = command.DesiredWorkMode,
                    AccessibilityNeeds = command.AccessibilityNeeds
                };

                await _candidateProfileUseCase.CreateAsync(command);
                TempData["Success"] = "Đã tạo hồ sơ ứng viên.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                return View(command);
            }
        }
        private const long MaxCvSizeBytes = 5 * 1024 * 1024;
        private const long MaxImageSizeBytes = 2 * 1024 * 1024;

        private async Task<string?> SaveCvFileAsync(IFormFile? cvFile)
        {
            if (cvFile is null || cvFile.Length == 0) return null;

            if (cvFile.Length > MaxCvSizeBytes)
                throw new UseCaseException("CV tối đa 5MB.");

            var ext = Path.GetExtension(cvFile.FileName).ToLowerInvariant();
            var allowed = new[] { ".pdf", ".doc", ".docx" };

            if (!allowed.Contains(ext))
                throw new UseCaseException("CV chỉ hỗ trợ PDF, DOC hoặc DOCX.");

            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cv");
            Directory.CreateDirectory(uploadsRoot);

            var safeName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsRoot, safeName);

            await using var stream = System.IO.File.Create(fullPath);
            await cvFile.CopyToAsync(stream);

            return $"/uploads/cv/{safeName}";
        }

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
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var profile = await _candidateProfileUseCase.GetMyProfileAsync();

                if (profile is null)
                {
                    TempData["Error"] = "Bạn chưa có hồ sơ ứng viên.";
                    return RedirectToAction(nameof(Create));
                }

                var command = new UpdateCandidateProfileCommand
                {
                    FullName = profile.FullName,
                    AvatarUrl = profile.AvatarUrl,
                    DateOfBirth = profile.DateOfBirth,
                    Gender = profile.Gender,
                    PhoneNumber = profile.PhoneNumber,
                    ContactEmail = profile.ContactEmail,
                    Address = profile.Address,
                    DesiredPosition = profile.DesiredPosition,
                    DesiredSalary = profile.DesiredSalary,
                    ExperienceSummary = profile.ExperienceSummary,
                    Skills = profile.Skills,
                    Education = profile.Education,
                    Certifications = profile.Certifications,
                    PortfolioUrl = profile.PortfolioUrl,
                    Bio = profile.Bio,
                    CvUrl = profile.CvUrl,
                    JobSeekingStatus = profile.JobSeekingStatus,
                    DesiredWorkMode = profile.DesiredWorkMode,
                    AccessibilityNeeds = profile.AccessibilityNeeds
                };

                return View(command);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCandidateProfileCommand command, IFormFile? cvFile, IFormFile? avatarFile)
        {
            try
            {
                var uploadedCvUrl = await SaveCvFileAsync(cvFile);
                var uploadedAvatarUrl = await SaveImageFileAsync(avatarFile, "avatars");

                command = new UpdateCandidateProfileCommand
                {
                    FullName = command.FullName,
                    AvatarUrl = uploadedAvatarUrl ?? command.AvatarUrl,
                    DateOfBirth = command.DateOfBirth,
                    Gender = command.Gender,
                    PhoneNumber = command.PhoneNumber,
                    ContactEmail = command.ContactEmail,
                    Address = command.Address,
                    DesiredPosition = command.DesiredPosition,
                    DesiredSalary = command.DesiredSalary,
                    ExperienceSummary = command.ExperienceSummary,
                    Skills = command.Skills,
                    Education = command.Education,
                    Certifications = command.Certifications,
                    PortfolioUrl = command.PortfolioUrl,
                    Bio = command.Bio,
                    CvUrl = uploadedCvUrl ?? command.CvUrl,
                    JobSeekingStatus = command.JobSeekingStatus,
                    DesiredWorkMode = command.DesiredWorkMode,
                    AccessibilityNeeds = command.AccessibilityNeeds
                };

                await _candidateProfileUseCase.UpdateMyProfileAsync(command);
                TempData["Success"] = "Đã cập nhật hồ sơ ứng viên.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                return View(command);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Disability()
        {
            try
            {
                var profile = await _candidateProfileUseCase.GetMyProfileAsync();

                if (profile is null)
                {
                    TempData["Error"] = "Bạn chưa có hồ sơ ứng viên.";
                    return RedirectToAction(nameof(Create));
                }

                await LoadDisabilityTypesAsync();

                var command = new UpdateDisabilityInfoCommand
                {
                    DisabilityTypeId = profile.DisabilityTypeId,
                    Description = profile.DisabilityDescription,
                    IsVisibleToEmployer = profile.IsDisabilityInfoVisibleToEmployer
                };

                return View(command);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disability(UpdateDisabilityInfoCommand command)
        {
            try
            {
                await _candidateProfileUseCase.UpdateMyDisabilityInfoAsync(command);

                TempData["Success"] = "Đã cập nhật thông tin khuyết tật.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;

                // Nếu return lại View khi lỗi, cần load lại dropdown,
                // nếu không ViewBag.DisabilityTypes sẽ null.
                await LoadDisabilityTypesAsync();

                return View(command);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HideDisabilityInfo()
        {
            try
            {
                await _candidateProfileUseCase.HideMyDisabilityInfoAsync();

                TempData["Success"] = "Đã ẩn thông tin khuyết tật khỏi nhà tuyển dụng.";
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShowDisabilityInfo()
        {
            try
            {
                await _candidateProfileUseCase.ShowMyDisabilityInfoAsync();

                TempData["Success"] = "Đã cho phép nhà tuyển dụng xem thông tin khuyết tật.";
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakePublic()
        {
            try
            {
                await _candidateProfileUseCase.MakeMyProfilePublicAsync();

                TempData["Success"] = "Hồ sơ của bạn đã được public.";
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakePrivate()
        {
            try
            {
                await _candidateProfileUseCase.MakeMyProfilePrivateAsync();

                TempData["Success"] = "Hồ sơ của bạn đã được chuyển về riêng tư.";
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Load danh sách loại khuyết tật active để render dropdown.
        /// 
        /// Đặt ở Controller vì đây là dữ liệu phục vụ View.
        /// Controller vẫn chỉ gọi Inbound Port, không gọi Repository trực tiếp.
        /// </summary>
        private async Task LoadDisabilityTypesAsync()
        {
            var disabilityTypes = await _catalogUseCase.GetActiveDisabilityTypesAsync();

            ViewBag.DisabilityTypes = disabilityTypes;
        }
    }
}
