using Domain.Catalogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Repositories
{
    /// <summary>
    /// Outbound Port cho danh mục thiết bị hỗ trợ.
    /// Ví dụ: screen reader, xe lăn, bàn phím đặc biệt, thiết bị trợ thính.
    /// </summary>
    public interface IAssistiveDeviceRepository
    {
        /// <summary>
        /// Tìm thiết bị hỗ trợ theo Id.
        /// </summary>
        Task<AssistiveDevice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy toàn bộ thiết bị hỗ trợ.
        /// </summary>
        Task<IReadOnlyList<AssistiveDevice>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy các thiết bị hỗ trợ đang active.
        /// </summary>
        Task<IReadOnlyList<AssistiveDevice>> GetActiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm thiết bị hỗ trợ mới.
        /// </summary>
        Task AddAsync(AssistiveDevice assistiveDevice, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật thiết bị hỗ trợ.
        /// </summary>
        Task UpdateAsync(AssistiveDevice assistiveDevice, CancellationToken cancellationToken = default);
    }
}
