using Application.Common;
using Application.Mappers;
using Ports.Inbound;
using Ports.Models.Candidates;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    // UseCase này cho phép Employer xem danh sách ứng viên đã bật public profile.
    public sealed class EmployerCandidateSearchUseCase : IEmployerCandidateSearchUseCase
    {
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly ICurrentUserService _currentUser;

        public EmployerCandidateSearchUseCase(
            ICandidateProfileRepository candidateProfileRepository,
            ICurrentUserService currentUser)
        {
            _candidateProfileRepository = candidateProfileRepository;
            _currentUser = currentUser;
        }

        public async Task<IReadOnlyList<CandidateProfileResult>> SearchAsync(
            SearchPublicCandidatesQuery query,
            CancellationToken cancellationToken = default)
        {
            AuthorizationGuard.RequireEmployer(_currentUser);
            // Chỉ Employer được tìm ứng viên.

            var profiles = await _candidateProfileRepository.GetPublicProfilesAsync(
                cancellationToken
            );
            // Tận dụng method đã định hướng từ repository hiện tại.

            var filtered = profiles.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.Trim().ToLowerInvariant();

                filtered = filtered.Where(x =>
                    x.FullName.Value.ToLowerInvariant().Contains(keyword) ||
                    (!string.IsNullOrWhiteSpace(x.Bio) &&
                     x.Bio.ToLowerInvariant().Contains(keyword))
                );
                // Lọc theo tên hoặc mô tả bản thân.
            }

            if (query.DisabilityTypeId.HasValue)
            {
                filtered = filtered.Where(x =>
                    x.DisabilityInfo is not null &&
                    x.DisabilityInfo.IsVisibleToEmployer &&
                    x.DisabilityInfo.DisabilityTypeId == query.DisabilityTypeId.Value
                );
                // Chỉ lọc theo disability nếu ứng viên cho phép hiển thị.
            }

            if (query.HasCv.HasValue)
            {
                filtered = filtered.Where(x =>
                    query.HasCv.Value
                        ? !string.IsNullOrWhiteSpace(x.CvUrl)
                        : string.IsNullOrWhiteSpace(x.CvUrl)
                );
                // Lọc ứng viên có hoặc chưa có CV.
            }

            return filtered
                .Select(x =>
                    CandidateProfileMapper.ToResult(
                        x,
                        canViewDisabilityInfo: x.DisabilityInfo is not null && x.DisabilityInfo.IsVisibleToEmployer
                    )
                )
                .ToList();
        }
    }
}
