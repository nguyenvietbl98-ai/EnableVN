using Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    /// <summary>
    /// Inbound Port cho các thao tác quản lý User.
    /// Chủ yếu dành cho Admin hoặc chính user hiện tại.
    /// </summary>
    public interface IUserUseCase
    {
        /// <summary>
        /// Khóa tài khoản.
        /// Thường chỉ Admin được quyền gọi.
        /// </summary>
        Task LockUserAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Kích hoạt lại tài khoản.
        /// </summary>
        Task ActivateUserAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Xóa mềm tài khoản.
        /// </summary>
        Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy vai trò của user.
        /// Có thể dùng để kiểm tra hoặc debug phân quyền.
        /// </summary>
        Task<UserRole?> GetUserRoleAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
