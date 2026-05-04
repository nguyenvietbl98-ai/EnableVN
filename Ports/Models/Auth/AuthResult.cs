using Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Auth
{
    /// <summary>
    /// Kết quả trả về sau khi đăng ký hoặc đăng nhập.
    /// 
    /// Token có thể là JWT hoặc session token tùy cách Presentation/Infrastructure implement.
    /// </summary>
    public sealed class AuthResult
    {
        public Guid UserId { get; init; }

        public string Email { get; init; } = string.Empty;

        public UserRole Role { get; init; }

        public string Token { get; init; } = string.Empty;
    }
}
