using Application.Common;
using Application.Mappers;
using Domain.Employers;
using Ports.Inbound;
using Ports.Models.Employers;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    /// <summary>
    /// UseCase quản lý hồ sơ doanh nghiệp.
    /// 
    /// UserId luôn lấy từ ICurrentUserService, không nhận từ client.
    /// </summary>
    public sealed class EmployerProfileUseCase : IEmployerProfileUseCase
    {
        private readonly IEmployerProfileRepository _employerProfileRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public EmployerProfileUseCase(
            IEmployerProfileRepository employerProfileRepository,
            ICurrentUserService currentUser,
            IDomainEventDispatcher domainEventDispatcher
        )
        {
            _employerProfileRepository = employerProfileRepository;
            _currentUser = currentUser;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<Guid> CreateAsync(
            CreateEmployerProfileCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireEmployer(_currentUser);

            var exists = await _employerProfileRepository.ExistsByUserIdAsync(
                userId,
                cancellationToken
            );

            if (exists)
                throw new UseCaseException("Nhà tuyển dụng đã có hồ sơ doanh nghiệp.");

            var workplaceInfo = InclusiveWorkplaceInfo.Create(
                command.HasWheelchairAccess,
                command.HasAccessibleRestroom,
                command.SupportsFlexibleWorkingTime,
                command.SupportsRemoteWork,
                command.ProvidesAssistiveDevices
            );

            var profile = EmployerProfile.Create(
                userId,
                command.CompanyName,
                command.Description,
                command.WebsiteUrl,
                workplaceInfo
            );

            await _employerProfileRepository.AddAsync(profile, cancellationToken);

            await DomainEventHelper.DispatchAndClearEventsAsync(
                profile,
                _domainEventDispatcher,
                cancellationToken
            );

            return profile.Id;
        }

        public async Task UpdateMyProfileAsync(
            UpdateEmployerProfileCommand command,
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireEmployer(_currentUser);

            var profile = await _employerProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            if (profile is null)
                throw new UseCaseException("Bạn chưa có hồ sơ doanh nghiệp.");

            var workplaceInfo = InclusiveWorkplaceInfo.Create(
                command.HasWheelchairAccess,
                command.HasAccessibleRestroom,
                command.SupportsFlexibleWorkingTime,
                command.SupportsRemoteWork,
                command.ProvidesAssistiveDevices
            );

            profile.UpdateCompanyInfo(
                command.CompanyName,
                command.Description,
                command.WebsiteUrl
            );

            profile.UpdateInclusiveWorkplaceInfo(workplaceInfo);

            await _employerProfileRepository.UpdateAsync(profile, cancellationToken);
        }

        public async Task<EmployerProfileResult?> GetMyProfileAsync(
            CancellationToken cancellationToken = default
        )
        {
            var userId = AuthorizationGuard.RequireEmployer(_currentUser);

            var profile = await _employerProfileRepository.GetByUserIdAsync(
                userId,
                cancellationToken
            );

            return profile is null
                ? null
                : EmployerProfileMapper.ToResult(profile);
        }

        public async Task<EmployerProfileResult?> GetByIdAsync(
            Guid employerProfileId,
            CancellationToken cancellationToken = default
        )
        {
            var profile = await _employerProfileRepository.GetByIdAsync(
                employerProfileId,
                cancellationToken
            );

            return profile is null
                ? null
                : EmployerProfileMapper.ToResult(profile);
        }
    }
}
