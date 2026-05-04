using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Services
{
    /// <summary>
    /// Outbound Port cho việc hash và verify password.
    /// 
    /// Application chỉ cần biết có thể hash/verify password.
    /// Còn dùng BCrypt, PBKDF2, Argon2 hay ASP.NET Identity
    /// là việc của Infrastructure.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hash mật khẩu dạng plain text trước khi lưu.
        /// Không bao giờ lưu plain password vào database.
        /// </summary>
        string Hash(string plainPassword);

        /// <summary>
        /// Kiểm tra mật khẩu plain text có khớp với password hash không.
        /// Dùng trong use case đăng nhập.
        /// </summary>
        bool Verify(string plainPassword, string passwordHash);
    }
}
