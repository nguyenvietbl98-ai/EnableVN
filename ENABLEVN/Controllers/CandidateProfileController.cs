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
        public async Task<IActionResult> Create(CreateCandidateProfileCommand command)
        {
            try
            {
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
                    Bio = profile.Bio,
                    CvUrl = profile.CvUrl
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
        public async Task<IActionResult> Edit(UpdateCandidateProfileCommand command)
        {
            try
            {
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
