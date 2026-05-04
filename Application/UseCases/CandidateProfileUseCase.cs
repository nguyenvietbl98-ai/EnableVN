using Application.Common;
using Application.Mappers;
using Domain.Candidates;
using Ports.Inbound;
using Ports.Models.Candidates;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    /// <summary>
    /// UseCase quản lý hồ sơ ứng viên.
    /// 
    /// Phần quan trọng nhất là bảo vệ quyền riêng tư thông tin khuyết tật.
    /// </summary>
    public sealed class CandidateProfileUseCase : ICandidateProfileUseCase
    {
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public CandidateProfileUseCase(
            ICandidateProfileRepository candidateProfileRepository,
            ICurrentUserService currentUser,
            IDomainEventDispatcher domainEventDispatcher
        )
        {
            _candidateProfileRepository = candidateProfileRepository;
            _currentUser = currentUser;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<Guid> CreateAsync(
            CreateCandidateProfileCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireCandidate(_currentUser);

            var exists = await _candidateProfileRepository.ExistsByUserIdAsync(
                userId,
                cancellationToken
            );

            if (exists)
                throw new UseCaseException("Ứng viên đã có hồ sơ.");

            var profile = CandidateProfile.Create(
                userId,
                command.FullName,
                command.Bio,
                command.CvUrl
            );

            await _candidateProfileRepository.AddAsync(profile, cancellationToken);

            await DomainEventHelper.DispatchAndClearEventsAsync(
                profile,
                _domainEventDispatcher,
                cancellationToken
            );

            return profile.Id;
        }

        public async Task UpdateMyProfileAsync(
            UpdateCandidateProfileCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var profile = await GetMyCandidateProfileOrThrowAsync(cancellationToken);

            profile.UpdateBasicInfo(
                command.FullName,
                command.Bio,
                command.CvUrl
            );

            await _candidateProfileRepository.UpdateAsync(profile, cancellationToken);
        }

        public async Task UpdateMyDisabilityInfoAsync(
            UpdateDisabilityInfoCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var profile = await GetMyCandidateProfileOrThrowAsync(cancellationToken);

            var disabilityInfo = DisabilityInfo.Create(
                command.DisabilityTypeId,
                command.Description,
                command.IsVisibleToEmployer
            );

            profile.UpdateDisabilityInfo(disabilityInfo);

            await _candidateProfileRepository.UpdateAsync(profile, cancellationToken);
        }

        public async Task HideMyDisabilityInfoAsync(
            CancellationToken cancellationToken = default
        )
        {
            var profile = await GetMyCandidateProfileOrThrowAsync(cancellationToken);

            profile.HideDisabilityInfo();

            await _candidateProfileRepository.UpdateAsync(profile, cancellationToken);
        }

        public async Task ShowMyDisabilityInfoAsync(
            CancellationToken cancellationToken = default
        )
        {
            var profile = await GetMyCandidateProfileOrThrowAsync(cancellationToken);

            profile.ShowDisabilityInfoToEmployer();

            await _candidateProfileRepository.UpdateAsync(profile, cancellationToken);
        }

        public async Task MakeMyProfilePublicAsync(
            CancellationToken cancellationToken = default
        )
        {
            var profile = await GetMyCandidateProfileOrThrowAsync(cancellationToken);

            profile.MakeProfilePublic();

            await _candidateProfileRepository.UpdateAsync(profile, cancellationToken);
        }

        public async Task MakeMyProfilePrivateAsync(
            CancellationToken cancellationToken = default
        )
        {
            var profile = await GetMyCandidateProfileOrThrowAsync(cancellationToken);

            profile.MakeProfilePrivate();

            await _candidateProfileRepository.UpdateAsync(profile, cancellationToken);
        }

        public async Task<CandidateProfileResult?> GetMyProfileAsync(
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireCandidate(_currentUser);

            var profile = await _candidateProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (profile is null)
                return null;

            // Chính ứng viên luôn được xem đầy đủ thông tin của mình.
            return CandidateProfileMapper.ToResult(
                profile,
                canViewDisabilityInfo: true
            );
        }

        public async Task<IReadOnlyList<CandidateProfileResult>> GetPublicProfilesAsync(
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireEmployer(_currentUser);

            var profiles = await _candidateProfileRepository.GetPublicProfilesAsync(
                cancellationToken
            );

            // Employer chỉ được xem thông tin khuyết tật nếu Candidate cho phép.
            return profiles
                .Select(profile =>
                    CandidateProfileMapper.ToResult(
                        profile,
                        canViewDisabilityInfo: profile.DisabilityInfo.IsVisibleToEmployer
                    )
                )
                .ToList();
        }

        private async Task<CandidateProfile> GetMyCandidateProfileOrThrowAsync(
            CancellationToken cancellationToken
        )
        {
            var userId = AuthorizationGuard.RequireCandidate(_currentUser);

            var profile = await _candidateProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (profile is null)
                throw new UseCaseException("Bạn chưa có hồ sơ ứng viên.");

            return profile;
        }
    }
}
