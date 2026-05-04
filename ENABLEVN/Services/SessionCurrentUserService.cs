using Domain.Users;
using Ports.Outbound.Services;

namespace Presentation.Services
{
    /// <summary>
    /// Implementation của ICurrentUserService cho tầng MVC.
    /// 
    /// Service này đọc thông tin user hiện tại từ Session.
    /// Application chỉ biết ICurrentUserService,
    /// không biết HttpContext hay Session.
    /// </summary>
    public sealed class SessionCurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionCurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var value = _httpContextAccessor
                    .HttpContext?
                    .Session
                    .GetString("UserId");

                return Guid.TryParse(value, out var userId)
                    ? userId
                    : null;
            }
        }

        public UserRole? Role
        {
            get
            {
                var value = _httpContextAccessor
                    .HttpContext?
                    .Session
                    .GetString("UserRole");

                return Enum.TryParse<UserRole>(value, out var role)
                    ? role
                    : null;
            }
        }

        public bool IsAuthenticated => UserId.HasValue && Role.HasValue;
    }
}
