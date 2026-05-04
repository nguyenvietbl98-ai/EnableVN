using Application.Common;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;
using Ports.Models.Catalogs;

namespace Presentation.Controllers
{
    /// <summary>
    /// Controller quản lý danh mục hệ thống cho Admin.
    /// 
    /// Controller này chỉ gọi ICatalogUseCase.
    /// Không gọi repository trực tiếp.
    /// 
    /// Quản lý:
    /// - DisabilityType
    /// - AssistiveDevice
    /// - JobCategory
    /// </summary>
    public sealed class CatalogController : Controller
    {
        private readonly ICatalogUseCase _catalogUseCase;

        public CatalogController(ICatalogUseCase catalogUseCase)
        {
            _catalogUseCase = catalogUseCase;
        }

        // =========================
        // Dashboard
        // =========================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // =========================
        // Disability Types
        // =========================

        [HttpGet]
        public async Task<IActionResult> DisabilityTypes()
        {
            try
            {
                var items = await _catalogUseCase.GetActiveDisabilityTypesAsync();

                ViewBag.CatalogTitle = "Loại khuyết tật";
                ViewBag.CreateAction = nameof(CreateDisabilityType);
                ViewBag.EditAction = nameof(EditDisabilityType);
                ViewBag.DeactivateAction = nameof(DeactivateDisabilityType);

                return View("CatalogList", items);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult CreateDisabilityType()
        {
            ViewBag.Title = "Tạo loại khuyết tật";
            ViewBag.PostAction = nameof(CreateDisabilityType);

            return View("CatalogForm", new CreateCatalogItemCommand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDisabilityType(CreateCatalogItemCommand command)
        {
            try
            {
                await _catalogUseCase.CreateDisabilityTypeAsync(command);

                TempData["Success"] = "Đã tạo loại khuyết tật.";

                return RedirectToAction(nameof(DisabilityTypes));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Title = "Tạo loại khuyết tật";
                ViewBag.PostAction = nameof(CreateDisabilityType);

                return View("CatalogForm", command);
            }
        }

        [HttpGet]
        public IActionResult EditDisabilityType(Guid id, string name, string? description)
        {
            ViewBag.Title = "Cập nhật loại khuyết tật";
            ViewBag.PostAction = nameof(EditDisabilityType);

            return View("CatalogEditForm", new UpdateCatalogItemCommand
            {
                Id = id,
                Name = name,
                Description = description
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDisabilityType(UpdateCatalogItemCommand command)
        {
            try
            {
                await _catalogUseCase.UpdateDisabilityTypeAsync(command);

                TempData["Success"] = "Đã cập nhật loại khuyết tật.";

                return RedirectToAction(nameof(DisabilityTypes));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Title = "Cập nhật loại khuyết tật";
                ViewBag.PostAction = nameof(EditDisabilityType);

                return View("CatalogEditForm", command);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateDisabilityType(Guid id)
        {
            try
            {
                await _catalogUseCase.DeactivateDisabilityTypeAsync(id);

                TempData["Success"] = "Đã tắt loại khuyết tật.";
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(DisabilityTypes));
        }

        // =========================
        // Assistive Devices
        // =========================

        [HttpGet]
        public async Task<IActionResult> AssistiveDevices()
        {
            try
            {
                var items = await _catalogUseCase.GetActiveAssistiveDevicesAsync();

                ViewBag.CatalogTitle = "Thiết bị hỗ trợ";
                ViewBag.CreateAction = nameof(CreateAssistiveDevice);
                ViewBag.EditAction = nameof(EditAssistiveDevice);
                ViewBag.DeactivateAction = nameof(DeactivateAssistiveDevice);

                return View("CatalogList", items);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult CreateAssistiveDevice()
        {
            ViewBag.Title = "Tạo thiết bị hỗ trợ";
            ViewBag.PostAction = nameof(CreateAssistiveDevice);

            return View("CatalogForm", new CreateCatalogItemCommand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssistiveDevice(CreateCatalogItemCommand command)
        {
            try
            {
                await _catalogUseCase.CreateAssistiveDeviceAsync(command);

                TempData["Success"] = "Đã tạo thiết bị hỗ trợ.";

                return RedirectToAction(nameof(AssistiveDevices));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Title = "Tạo thiết bị hỗ trợ";
                ViewBag.PostAction = nameof(CreateAssistiveDevice);

                return View("CatalogForm", command);
            }
        }

        [HttpGet]
        public IActionResult EditAssistiveDevice(Guid id, string name, string? description)
        {
            ViewBag.Title = "Cập nhật thiết bị hỗ trợ";
            ViewBag.PostAction = nameof(EditAssistiveDevice);

            return View("CatalogEditForm", new UpdateCatalogItemCommand
            {
                Id = id,
                Name = name,
                Description = description
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssistiveDevice(UpdateCatalogItemCommand command)
        {
            try
            {
                await _catalogUseCase.UpdateAssistiveDeviceAsync(command);

                TempData["Success"] = "Đã cập nhật thiết bị hỗ trợ.";

                return RedirectToAction(nameof(AssistiveDevices));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Title = "Cập nhật thiết bị hỗ trợ";
                ViewBag.PostAction = nameof(EditAssistiveDevice);

                return View("CatalogEditForm", command);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateAssistiveDevice(Guid id)
        {
            try
            {
                await _catalogUseCase.DeactivateAssistiveDeviceAsync(id);

                TempData["Success"] = "Đã tắt thiết bị hỗ trợ.";
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(AssistiveDevices));
        }

        // =========================
        // Job Categories
        // =========================

        [HttpGet]
        public async Task<IActionResult> JobCategories()
        {
            try
            {
                var items = await _catalogUseCase.GetActiveJobCategoriesAsync();

                ViewBag.CatalogTitle = "Ngành nghề";
                ViewBag.CreateAction = nameof(CreateJobCategory);
                ViewBag.EditAction = nameof(EditJobCategory);
                ViewBag.DeactivateAction = nameof(DeactivateJobCategory);

                return View("CatalogList", items);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult CreateJobCategory()
        {
            ViewBag.Title = "Tạo ngành nghề";
            ViewBag.PostAction = nameof(CreateJobCategory);

            return View("CatalogForm", new CreateCatalogItemCommand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJobCategory(CreateCatalogItemCommand command)
        {
            try
            {
                await _catalogUseCase.CreateJobCategoryAsync(command);

                TempData["Success"] = "Đã tạo ngành nghề.";

                return RedirectToAction(nameof(JobCategories));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Title = "Tạo ngành nghề";
                ViewBag.PostAction = nameof(CreateJobCategory);

                return View("CatalogForm", command);
            }
        }

        [HttpGet]
        public IActionResult EditJobCategory(Guid id, string name, string? description)
        {
            ViewBag.Title = "Cập nhật ngành nghề";
            ViewBag.PostAction = nameof(EditJobCategory);

            return View("CatalogEditForm", new UpdateCatalogItemCommand
            {
                Id = id,
                Name = name,
                Description = description
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJobCategory(UpdateCatalogItemCommand command)
        {
            try
            {
                await _catalogUseCase.UpdateJobCategoryAsync(command);

                TempData["Success"] = "Đã cập nhật ngành nghề.";

                return RedirectToAction(nameof(JobCategories));
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Title = "Cập nhật ngành nghề";
                ViewBag.PostAction = nameof(EditJobCategory);

                return View("CatalogEditForm", command);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateJobCategory(Guid id)
        {
            try
            {
                await _catalogUseCase.DeactivateJobCategoryAsync(id);

                TempData["Success"] = "Đã tắt ngành nghề.";
            }
            catch (Exception ex) when (ex is UseCaseException or DomainException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(JobCategories));
        }
    }
}
