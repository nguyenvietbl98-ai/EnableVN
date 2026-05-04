using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Catalogs
{
    /// <summary>
    /// Danh mục ngành nghề / nhóm công việc.
    /// 
    /// Ví dụ:
    /// - Công nghệ thông tin
    /// - Marketing
    /// - Kế toán
    /// - Chăm sóc khách hàng
    /// - Thiết kế đồ họa
    /// 
    /// Class này thuộc Catalog Domain và thường do Admin quản lý.
    /// </summary>
    public sealed class JobCategory : AggregateRoot<Guid>
    {
        /// <summary>
        /// Tên ngành nghề / nhóm công việc.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Mô tả thêm về ngành nghề.
        /// Có thể null nếu không cần mô tả.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Trạng thái danh mục.
        /// Active: đang dùng.
        /// Inactive: tạm ẩn, không hiển thị cho người dùng chọn.
        /// </summary>
        public CatalogStatus Status { get; private set; }

        private JobCategory(
            Guid id,
            string name,
            string? description
        ) : base(id)
        {
            Name = name;
            Description = description;
            Status = CatalogStatus.Active;
        }

        /// <summary>
        /// Tạo ngành nghề mới.
        /// 
        /// Dùng Factory Method để đảm bảo object tạo ra luôn hợp lệ.
        /// </summary>
        public static JobCategory Create(
            string name,
            string? description
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên ngành nghề không được để trống.");

            name = name.Trim();

            if (name.Length < 2)
                throw new DomainException("Tên ngành nghề quá ngắn.");

            if (name.Length > 100)
                throw new DomainException("Tên ngành nghề không được vượt quá 100 ký tự.");

            return new JobCategory(
                Guid.NewGuid(),
                name,
                description?.Trim()
            );
        }

        /// <summary>
        /// Cập nhật tên và mô tả ngành nghề.
        /// </summary>
        public void Update(
            string name,
            string? description
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên ngành nghề không được để trống.");

            name = name.Trim();

            if (name.Length < 2)
                throw new DomainException("Tên ngành nghề quá ngắn.");

            if (name.Length > 100)
                throw new DomainException("Tên ngành nghề không được vượt quá 100 ký tự.");

            Name = name;
            Description = description?.Trim();
        }

        /// <summary>
        /// Kích hoạt ngành nghề.
        /// Sau khi active, ngành nghề có thể hiển thị cho người dùng chọn.
        /// </summary>
        public void Activate()
        {
            Status = CatalogStatus.Active;
        }

        /// <summary>
        /// Tạm ẩn ngành nghề.
        /// Không xóa cứng để tránh ảnh hưởng dữ liệu Job đã tham chiếu trước đó.
        /// </summary>
        public void Deactivate()
        {
            Status = CatalogStatus.Inactive;
        }
        public static JobCategory Restore(
    Guid id,
    string name,
    string? description,
    CatalogStatus status)
        {
            if (id == Guid.Empty)
                throw new DomainException("JobCategoryId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên ngành nghề không được để trống.");

            var item = new JobCategory(id, name.Trim(), description);
            item.Status = status;

            return item;
        }
    }
}
