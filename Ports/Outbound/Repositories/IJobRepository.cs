using Domain.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Repositories
{
    /// <summary>
    /// Outbound Port cho tin tuyển dụng.
    /// Đây là interface quan trọng nhất cho luồng:
    /// Employer đăng việc - Candidate tìm việc.
    /// </summary>
    public interface IJobRepository
    {
        /// <summary>
        /// Tìm job theo Id.
        /// </summary>
        Task<JobPost?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy danh sách job của một Employer.
        /// Dùng cho trang quản lý tin tuyển dụng của nhà tuyển dụng.
        /// </summary>
        Task<IReadOnlyList<JobPost>> GetByEmployerIdAsync(
            Guid employerId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Tìm các job đang được đăng.
        /// Có thể dùng cho trang danh sách việc làm public.
        /// 
        /// Các filter accessibility được để dạng nullable:
        /// - null nghĩa là không lọc theo tiêu chí đó
        /// - true nghĩa là chỉ lấy job có hỗ trợ tiêu chí đó
        /// </summary>
        Task<IReadOnlyList<JobPost>> SearchPublishedJobsAsync(
            string? keyword,
            WorkMode? workMode,
            bool? supportsWheelchairAccess,
            bool? supportsRemoteWork,
            bool? supportsFlexibleTime,
            bool? providesAssistiveDevices,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Thêm job mới.
        /// Thường là job ở trạng thái Draft.
        /// </summary>
        Task AddAsync(JobPost job, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cập nhật job.
        /// Ví dụ: sửa nội dung, publish, close, delete.
        /// </summary>
        Task UpdateAsync(JobPost job, CancellationToken cancellationToken = default);
    }
}
