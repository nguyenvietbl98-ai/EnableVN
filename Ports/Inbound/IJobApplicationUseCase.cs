using Ports.Models.Applications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{
    /// <summary>
    /// Inbound Port cho nghiệp vụ ứng tuyển.
    /// Dùng cho luồng Candidate nộp CV và Employer quản lý trạng thái hồ sơ.
    /// </summary>
    public interface IJobApplicationUseCase
    {
        /// <summary>
        /// Candidate nộp hồ sơ vào một job.
        /// 
        /// Rule cần xử lý trong Application:
        /// - User hiện tại phải là Candidate
        /// - Candidate phải có CandidateProfile
        /// - Job phải tồn tại
        /// - Job phải đang Published
        /// - Candidate chưa từng nộp vào job này
        /// </summary>
        Task<Guid> SubmitAsync(
            SubmitJobApplicationCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Employer đổi trạng thái hồ sơ ứng tuyển.
        /// 
        /// Rule cần xử lý trong Application:
        /// - User hiện tại phải là Employer
        /// - Application phải tồn tại
        /// - Job của application phải thuộc Employer hiện tại
        /// - Trạng thái mới phải hợp lệ theo Domain Policy
        /// </summary>
        Task ChangeStatusAsync(
            ChangeApplicationStatusCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Candidate rút hồ sơ đã nộp.
        /// </summary>
        Task WithdrawAsync(
            Guid applicationId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Employer xem danh sách hồ sơ ứng tuyển theo Job.
        /// </summary>
        Task<IReadOnlyList<JobApplicationResult>> GetByJobIdAsync(
            Guid jobId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Candidate xem danh sách hồ sơ mình đã nộp.
        /// </summary>
        Task<IReadOnlyList<JobApplicationResult>> GetMyApplicationsAsync(
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Xem chi tiết một hồ sơ ứng tuyển.
        /// </summary>
        Task<JobApplicationResult?> GetByIdAsync(
            Guid applicationId,
            CancellationToken cancellationToken = default
        );
    }
}
