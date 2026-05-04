using System;
using System.Collections.Generic;
using System.Text;
using Domain.Catalogs;

namespace Ports.Outbound.Repositories
{
    /// <summary>
    /// Outbound Port cho danh mục ngành nghề / nhóm công việc.
    /// Ví dụ: IT, Marketing, Kế toán, Chăm sóc khách hàng.
    /// </summary>
    public interface IJobCategoryRepository
    {
        /// <summary>
        /// Tìm ngành nghề theo Id.
        /// </summary>
        Task<JobCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy toàn bộ ngành nghề.
        /// </summary>
        Task<IReadOnlyList<JobCategory>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy các ngành nghề đang active.
        /// </summary>
        Task<IReadOnlyList<JobCategory>> GetActiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm ngành nghề mới.
        /// </summary>
        Task AddAsync(JobCategory category, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật ngành nghề.
        /// </summary>
        Task UpdateAsync(JobCategory category, CancellationToken cancellationToken = default);
    }
}
