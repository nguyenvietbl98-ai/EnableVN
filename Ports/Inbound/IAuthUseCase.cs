using Ports.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    /// <summary>
    /// Inbound Port cho nghiệp vụ xác thực.
    /// 
    /// Presentation/API sẽ gọi interface này.
    /// Application sẽ implement interface này.
    /// </summary>
    public interface IAuthUseCase
    {
        /// <summary>
        /// Đăng ký tài khoản mới.
        /// </summary>
        Task<AuthResult> RegisterAsync(
            RegisterCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Đăng nhập và trả về token.
        /// </summary>
        Task<AuthResult> LoginAsync(
            LoginCommand command,
            CancellationToken cancellationToken = default
        );
    }
}
