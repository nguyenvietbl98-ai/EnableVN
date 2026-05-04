using Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Services
{
    /// <summary>
    /// Outbound Port dùng để Application biết user hiện tại là ai.
    /// 
    /// Interface này thường được implement ở Presentation hoặc Infrastructure,
    /// dựa trên JWT, Session hoặc HttpContext.
    /// 
    /// Domain không biết current user.
    /// Application cần biết current user để kiểm tra quyền.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Id của user đang đăng nhập.
        /// Null nếu request hiện tại chưa đăng nhập.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// Vai trò của user đang đăng nhập.
        /// Null nếu chưa đăng nhập.
        /// </summary>
        UserRole? Role { get; }

        /// <summary>
        /// Kiểm tra request hiện tại đã đăng nhập hay chưa.
        /// </summary>
        bool IsAuthenticated { get; }
    }
}
