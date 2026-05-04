using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Ports.Inbound;

namespace Presentation.ViewComponents
{
    public sealed class NotificationBellViewComponent : ViewComponent
    {
        private readonly INotificationUseCase _notificationUseCase;

        public NotificationBellViewComponent(INotificationUseCase notificationUseCase)
        {
            _notificationUseCase = notificationUseCase;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var unread = await _notificationUseCase.CountMyUnreadAsync();
                return View(unread);
            }
            catch (UseCaseException)
            {
                // Không đăng nhập hoặc không có quyền -> không render badge.
                return View(0);
            }
        }
    }
}

