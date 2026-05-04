using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;

namespace Presentation.Controllers
{
    // Controller này xử lý màn hình thông báo của user đang đăng nhập.
    public sealed class NotificationsController : Controller
    {
        private readonly INotificationUseCase _notificationUseCase;

        public NotificationsController(INotificationUseCase notificationUseCase)
        {
            _notificationUseCase = notificationUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var notifications = await _notificationUseCase.GetMyNotificationsAsync();
                return View(notifications);
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                await _notificationUseCase.MarkAsReadAsync(id);
                TempData["Success"] = "Đã đánh dấu thông báo là đã đọc.";
            }
            catch (UseCaseException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
