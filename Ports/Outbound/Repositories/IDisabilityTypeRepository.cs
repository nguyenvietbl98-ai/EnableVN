using Domain.Catalogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Repositories
{
    /// <summary>
    /// Outbound Port cho danh mục loại khuyết tật.
    /// Danh mục này do Admin quản lý.
    /// </summary>
    public interface IDisabilityTypeRepository
    {
        /// <summary>
        /// Tìm loại khuyết tật theo Id.
        /// </summary>
        Task<DisabilityType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy toàn bộ loại khuyết tật.
        /// Dùng cho form tạo/cập nhật hồ sơ ứng viên.
        /// </summary>
        Task<IReadOnlyList<DisabilityType>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy các loại khuyết tật đang active.
        /// Dùng cho dropdown public.
        /// </summary>
        Task<IReadOnlyList<DisabilityType>> GetActiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Thêm loại khuyết tật mới.
        /// </summary>
        Task AddAsync(DisabilityType disabilityType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật loại khuyết tật.
        /// </summary>
        Task UpdateAsync(DisabilityType disabilityType, CancellationToken cancellationToken = default);
    }
}
