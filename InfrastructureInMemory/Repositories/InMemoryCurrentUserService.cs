using Domain.Users;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// CurrentUserService bản InMemory.
    /// 
    /// Dùng cho test, demo, hoặc chạy thử use case không cần JWT thật.
    /// 
    /// Trong Presentation thật, ICurrentUserService thường sẽ đọc từ:
    /// - JWT claims
    /// - HttpContext.User
    /// - Session
    /// </summary>
    public sealed class InMemoryCurrentUserService : ICurrentUserService
    {
        public Guid? UserId { get; private set; }

        public UserRole? Role { get; private set; }

        public bool IsAuthenticated => UserId.HasValue && Role.HasValue;

        /// <summary>
        /// Set user hiện tại.
        /// Dùng trong test hoặc demo để giả lập đăng nhập.
        /// </summary>
        public void SetCurrentUser(Guid userId, UserRole role)
        {
            UserId = userId;
            Role = role;
        }

        /// <summary>
        /// Xóa user hiện tại.
        /// Dùng để giả lập trạng thái chưa đăng nhập.
        /// </summary>
        public void Clear()
        {
            UserId = null;
            Role = null;
        }
    }
}
