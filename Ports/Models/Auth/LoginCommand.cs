using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Auth
{
    /// <summary>
    /// Command dùng cho use case đăng nhập.
    /// </summary>
    public sealed class LoginCommand
    {
        /// <summary>
        /// Email đăng nhập.
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Mật khẩu plain text để verify với password hash.
        /// </summary>
        public string Password { get; init; } = string.Empty;
    }
}
