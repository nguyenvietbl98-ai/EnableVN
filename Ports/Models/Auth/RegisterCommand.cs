using Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Auth
{
    /// <summary>
    /// Command dùng cho use case đăng ký tài khoản.
    /// 
    /// Đây là input model của Inbound Port.
    /// Presentation nhận request từ API rồi map sang command này.
    /// </summary>
    public sealed class RegisterCommand
    {
        /// <summary>
        /// Email đăng ký.
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Mật khẩu plain text từ người dùng.
        /// Application sẽ dùng IPasswordHasher để hash.
        /// Domain chỉ nhận passwordHash, không nhận plain password.
        /// </summary>
        public string Password { get; init; } = string.Empty;

        /// <summary>
        /// Vai trò đăng ký: Employer hoặc Candidate.
        /// Admin thường không nên tự đăng ký public.
        /// </summary>
        public UserRole Role { get; init; }
    }
}
