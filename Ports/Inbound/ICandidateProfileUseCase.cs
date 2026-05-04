using Ports.Models.Candidates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Inbound
{

    /// <summary>
    /// Inbound Port cho nghiệp vụ hồ sơ ứng viên.
    /// </summary>
    public interface ICandidateProfileUseCase
    {
        /// <summary>
        /// Tạo hồ sơ cho Candidate đang đăng nhập.
        /// </summary>
        Task<Guid> CreateAsync(
            CreateCandidateProfileCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Cập nhật thông tin cơ bản của hồ sơ ứng viên hiện tại.
        /// </summary>
        Task UpdateMyProfileAsync(
            UpdateCandidateProfileCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Cập nhật thông tin khuyết tật.
        /// Bao gồm cả quyền hiển thị cho Employer.
        /// </summary>
        Task UpdateMyDisabilityInfoAsync(
            UpdateDisabilityInfoCommand command,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Ẩn thông tin khuyết tật khỏi Employer.
        /// </summary>
        Task HideMyDisabilityInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Cho phép Employer xem thông tin khuyết tật.
        /// </summary>
        Task ShowMyDisabilityInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Public profile để Employer có thể tìm thấy.
        /// Dùng cho giai đoạn 2.
        /// </summary>
        Task MakeMyProfilePublicAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Private profile để Employer không tìm thấy trong danh sách public.
        /// </summary>
        Task MakeMyProfilePrivateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Xem hồ sơ của chính Candidate đang đăng nhập.
        /// </summary>
        Task<CandidateProfileResult?> GetMyProfileAsync(
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Lấy danh sách profile public.
        /// Dùng cho Employer tìm ứng viên ở giai đoạn 2.
        /// </summary>
        Task<IReadOnlyList<CandidateProfileResult>> GetPublicProfilesAsync(
            CancellationToken cancellationToken = default
        );
    }
}
