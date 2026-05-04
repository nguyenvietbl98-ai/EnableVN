using Ports.Models.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    /// <summary>
    /// Inbound Port cho nghiệp vụ tin tuyển dụng.
    /// Đây là use case chính của MVP.
    /// </summary>
    public interface IJobUseCase
    {
        /// <summary>
        /// Employer tạo tin tuyển dụng dạng Draft.
        /// </summary>
        Task<Guid> CreateDraftAsync(
            CreateJobCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Employer cập nhật nội dung tin tuyển dụng.
        /// </summary>
        Task UpdateAsync(
            UpdateJobCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Employer publish tin tuyển dụng.
        /// Chỉ owner của job mới được publish.
        /// </summary>
        Task PublishAsync(Guid jobId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Employer đóng tin tuyển dụng.
        /// </summary>
        Task CloseAsync(Guid jobId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Employer xóa mềm tin tuyển dụng.
        /// </summary>
        Task DeleteAsync(Guid jobId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Xem chi tiết job.
        /// </summary>
        Task<JobResult?> GetByIdAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lấy danh sách job của Employer đang đăng nhập.
        /// </summary>
        Task<IReadOnlyList<JobResult>> GetMyJobsAsync(
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Candidate hoặc guest tìm kiếm các job đang Published.
        /// Có hỗ trợ filter theo accessibility.
        /// </summary>
        Task<IReadOnlyList<JobResult>> SearchPublishedJobsAsync(
            SearchJobQuery query,
            CancellationToken cancellationToken = default
        );
    }
}
