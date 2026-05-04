using Domain.Applications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Repositories
{
    /// <summary>
    /// Outbound Port cho hồ sơ ứng tuyển.
    /// Dùng trong luồng Candidate nộp CV và Employer quản lý trạng thái hồ sơ.
    /// </summary>
    public interface IJobApplicationRepository
    {
        /// <summary>
        /// Tìm hồ sơ ứng tuyển theo Id.
        /// </summary>
        Task<JobApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy danh sách hồ sơ ứng tuyển theo JobId.
        /// Dùng cho Employer xem danh sách ứng viên đã nộp vào một tin tuyển dụng.
        /// </summary>
        Task<IReadOnlyList<JobApplication>> GetByJobIdAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lấy danh sách hồ sơ ứng tuyển của một Candidate.
        /// Dùng cho Candidate xem lịch sử ứng tuyển của mình.
        /// </summary>
        Task<IReadOnlyList<JobApplication>> GetByCandidateIdAsync(
            Guid candidateId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Kiểm tra Candidate đã nộp vào Job này chưa.
        /// Rule này cần Repository vì phải kiểm tra dữ liệu đã lưu.
        /// Không nên đặt trực tiếp trong Domain Entity.
        /// </summary>
        Task<bool> ExistsByJobIdAndCandidateIdAsync(
            Guid jobId,
            Guid candidateId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Thêm hồ sơ ứng tuyển mới.
        /// </summary>
        Task AddAsync(JobApplication application, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật hồ sơ ứng tuyển.
        /// Ví dụ: đổi trạng thái, rút hồ sơ.
        /// </summary>
        Task UpdateAsync(JobApplication application, CancellationToken cancellationToken = default);
    }
}
