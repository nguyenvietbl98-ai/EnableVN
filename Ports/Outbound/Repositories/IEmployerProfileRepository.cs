using Domain.Employers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Repositories
{
    /// <summary>
    /// Outbound Port cho hồ sơ doanh nghiệp.
    /// Application dùng interface này để đọc/ghi EmployerProfile.
    /// </summary>
    public interface IEmployerProfileRepository
    {
        /// <summary>
        /// Tìm hồ sơ doanh nghiệp theo Id.
        /// </summary>
        Task<EmployerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tìm hồ sơ doanh nghiệp theo UserId.
        /// Dùng khi cần kiểm tra một Employer đã có profile chưa.
        /// </summary>
        Task<EmployerProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Kiểm tra User đã có EmployerProfile hay chưa.
        /// Một Employer thường chỉ nên có một hồ sơ doanh nghiệp chính.
        /// </summary>
        Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm hồ sơ doanh nghiệp mới.
        /// </summary>
        Task AddAsync(EmployerProfile profile, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật hồ sơ doanh nghiệp.
        /// </summary>
        Task UpdateAsync(EmployerProfile profile, CancellationToken cancellationToken = default);
    }
}
