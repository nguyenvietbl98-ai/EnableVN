using System;
using System.Collections.Generic;
using System.Text;
using Domain.Users;

namespace Ports.Outbound.Repositories
{
    /// <summary>
    /// Outbound Port dùng để Application làm việc với dữ liệu User.
    /// 
    /// Interface này chỉ định nghĩa "cần làm gì", không quan tâm lưu bằng SQLite,
    /// InMemory, SQL Server hay API bên ngoài.
    /// 
    /// InfrastructureSqlite hoặc InfrastructureInMemory sẽ implement interface này.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Tìm User theo Id.
        /// Dùng khi cần kiểm tra tài khoản có tồn tại hay không.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tìm User theo email.
        /// Dùng trong đăng nhập hoặc kiểm tra email đã tồn tại khi đăng ký.
        /// </summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Kiểm tra email đã tồn tại trong hệ thống hay chưa.
        /// Dùng để tránh đăng ký trùng email.
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm User mới vào nơi lưu trữ.
        /// </summary>
        Task AddAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật User đã tồn tại.
        /// Ví dụ: khóa tài khoản, kích hoạt lại, xóa mềm.
        /// </summary>
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    }
}
