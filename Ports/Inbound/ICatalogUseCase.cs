using Ports.Models.Catalogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    /// <summary>
    /// Inbound Port cho quản lý danh mục hệ thống.
    /// Chủ yếu dành cho Admin.
    /// 
    /// Bao gồm:
    /// - Loại khuyết tật
    /// - Thiết bị hỗ trợ
    /// - Ngành nghề
    /// </summary>
    public interface ICatalogUseCase
    {
        // =========================
        // Disability Types
        // =========================

        /// <summary>
        /// Admin tạo loại khuyết tật.
        /// </summary>
        Task<Guid> CreateDisabilityTypeAsync(
            CreateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Admin cập nhật loại khuyết tật.
        /// </summary>
        Task UpdateDisabilityTypeAsync(
            UpdateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Admin tắt loại khuyết tật.
        /// </summary>
        Task DeactivateDisabilityTypeAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Admin bật lại loại khuyết tật.
        /// </summary>
        Task ActivateDisabilityTypeAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lấy danh sách loại khuyết tật đang active.
        /// Dùng cho dropdown ở form ứng viên.
        /// </summary>
        Task<IReadOnlyList<CatalogItemResult>> GetActiveDisabilityTypesAsync(
            CancellationToken cancellationToken = default
        );

        // =========================
        // Assistive Devices
        // =========================

        /// <summary>
        /// Admin tạo thiết bị hỗ trợ.
        /// </summary>
        Task<Guid> CreateAssistiveDeviceAsync(
            CreateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Admin cập nhật thiết bị hỗ trợ.
        /// </summary>
        Task UpdateAssistiveDeviceAsync(
            UpdateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Admin tắt thiết bị hỗ trợ.
        /// </summary>
        Task DeactivateAssistiveDeviceAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Admin bật lại thiết bị hỗ trợ.
        /// </summary>
        Task ActivateAssistiveDeviceAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lấy danh sách thiết bị hỗ trợ đang active.
        /// </summary>
        Task<IReadOnlyList<CatalogItemResult>> GetActiveAssistiveDevicesAsync(
            CancellationToken cancellationToken = default
        );

        // =========================
        // Job Categories
        // =========================

        /// <summary>
        /// Admin tạo ngành nghề.
        /// </summary>
        Task<Guid> CreateJobCategoryAsync(
            CreateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Admin cập nhật ngành nghề.
        /// </summary>
        Task UpdateJobCategoryAsync(
            UpdateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Admin tắt ngành nghề.
        /// </summary>
        Task DeactivateJobCategoryAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Admin bật lại ngành nghề.
        /// </summary>
        Task ActivateJobCategoryAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lấy danh sách ngành nghề đang active.
        /// </summary>
        Task<IReadOnlyList<CatalogItemResult>> GetActiveJobCategoriesAsync(
            CancellationToken cancellationToken = default
        );
    }
}
