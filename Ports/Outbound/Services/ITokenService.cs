using Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Services
{
    /// <summary>
    /// Outbound Port dùng để tạo access token sau khi đăng nhập/đăng ký.
    /// 
    /// Application chỉ biết rằng hệ thống có thể tạo token.
    /// Còn token là JWT, session token hay loại khác thì Infrastructure quyết định.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Tạo token cho user.
        /// </summary>
        string GenerateToken(Guid userId, string email, UserRole role);
    }
}
