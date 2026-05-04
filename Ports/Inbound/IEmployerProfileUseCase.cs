using Ports.Models.Employers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    /// <summary>
    /// Inbound Port cho nghiệp vụ hồ sơ doanh nghiệp.
    /// Presentation gọi interface này để tạo/cập nhật/xem hồ sơ doanh nghiệp.
    /// </summary>
    public interface IEmployerProfileUseCase
    {
        /// <summary>
        /// Tạo hồ sơ doanh nghiệp cho Employer đang đăng nhập.
        /// </summary>
        Task<Guid> CreateAsync(
            CreateEmployerProfileCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Cập nhật hồ sơ doanh nghiệp của Employer đang đăng nhập.
        /// </summary>
        Task UpdateMyProfileAsync(
            UpdateEmployerProfileCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lấy hồ sơ doanh nghiệp của user đang đăng nhập.
        /// </summary>
        Task<EmployerProfileResult?> GetMyProfileAsync(
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lấy hồ sơ doanh nghiệp theo Id.
        /// Dùng cho trang public hoặc Admin.
        /// </summary>
        Task<EmployerProfileResult?> GetByIdAsync(
            Guid employerProfileId,
            CancellationToken cancellationToken = default
        );
    }
}
