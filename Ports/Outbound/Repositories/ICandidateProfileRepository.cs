using Domain.Candidates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Repositories
{
    /// <summary>
    /// Outbound Port cho hồ sơ ứng viên.
    /// Application dùng interface này để quản lý CandidateProfile.
    /// </summary>
    public interface ICandidateProfileRepository
    {
        /// <summary>
        /// Tìm hồ sơ ứng viên theo Id.
        /// </summary>
        Task<CandidateProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tìm hồ sơ ứng viên theo UserId.
        /// Dùng khi user đăng nhập và muốn xem hồ sơ của mình.
        /// </summary>
        Task<CandidateProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Kiểm tra User đã có CandidateProfile hay chưa.
        /// </summary>
        Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy danh sách ứng viên đã public profile.
        /// Dùng cho tính năng giai đoạn 2: Employer chủ động tìm ứng viên.
        /// </summary>
        Task<IReadOnlyList<CandidateProfile>> GetPublicProfilesAsync(
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Thêm hồ sơ ứng viên mới.
        /// </summary>
        Task AddAsync(CandidateProfile profile, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật hồ sơ ứng viên.
        /// Ví dụ: cập nhật CV, ẩn/hiện thông tin khuyết tật, public/private profile.
        /// </summary>
        Task UpdateAsync(CandidateProfile profile, CancellationToken cancellationToken = default);
    }
}
