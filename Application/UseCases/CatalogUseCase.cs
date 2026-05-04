using Application.Common;
using Application.Mappers;
using Domain.Catalogs;
using Ports.Inbound;
using Ports.Models.Catalogs;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases
{
    /// <summary>
    /// UseCase quản lý danh mục hệ thống.
    /// 
    /// Chủ yếu dành cho Admin:
    /// - Loại khuyết tật
    /// - Thiết bị hỗ trợ
    /// - Ngành nghề
    /// </summary>
    public sealed class CatalogUseCase : ICatalogUseCase
    {
        private readonly IDisabilityTypeRepository _disabilityTypeRepository;
        private readonly IAssistiveDeviceRepository _assistiveDeviceRepository;
        private readonly IJobCategoryRepository _jobCategoryRepository;
        private readonly ICurrentUserService _currentUser;

        public CatalogUseCase(
            IDisabilityTypeRepository disabilityTypeRepository,
            IAssistiveDeviceRepository assistiveDeviceRepository,
            IJobCategoryRepository jobCategoryRepository,
            ICurrentUserService currentUser
        )
        {
            _disabilityTypeRepository = disabilityTypeRepository;
            _assistiveDeviceRepository = assistiveDeviceRepository;
            _jobCategoryRepository = jobCategoryRepository;
            _currentUser = currentUser;
        }

        // =========================
        // Disability Types
        // =========================

        public async Task<Guid> CreateDisabilityTypeAsync(
            CreateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = DisabilityType.Create(
                command.Name,
                command.Description
            );

            await _disabilityTypeRepository.AddAsync(
                item,
                cancellationToken
            );

            return item.Id;
        }

        public async Task UpdateDisabilityTypeAsync(
            UpdateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = await _disabilityTypeRepository.GetByIdAsync(
                command.Id,
                cancellationToken
            );

            if (item is null)
                throw new UseCaseException("Không tìm thấy loại khuyết tật.");

            item.Update(
                command.Name,
                command.Description
            );

            await _disabilityTypeRepository.UpdateAsync(
                item,
                cancellationToken
            );
        }

        public async Task DeactivateDisabilityTypeAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = await _disabilityTypeRepository.GetByIdAsync(
                id,
                cancellationToken
            );

            if (item is null)
                throw new UseCaseException("Không tìm thấy loại khuyết tật.");

            item.Deactivate();

            await _disabilityTypeRepository.UpdateAsync(
                item,
                cancellationToken
            );
        }

        public async Task ActivateDisabilityTypeAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = await _disabilityTypeRepository.GetByIdAsync(
                id,
                cancellationToken
            );

            if (item is null)
                throw new UseCaseException("Không tìm thấy loại khuyết tật.");

            item.Activate();

            await _disabilityTypeRepository.UpdateAsync(
                item,
                cancellationToken
            );
        }

        public async Task<IReadOnlyList<CatalogItemResult>> GetActiveDisabilityTypesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var items = await _disabilityTypeRepository.GetActiveAsync(
                cancellationToken
            );

            return items
                .Select(CatalogMapper.ToResult)
                .ToList();
        }

        // =========================
        // Assistive Devices
        // =========================

        public async Task<Guid> CreateAssistiveDeviceAsync(
            CreateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = AssistiveDevice.Create(
                command.Name,
                command.Description
            );

            await _assistiveDeviceRepository.AddAsync(
                item,
                cancellationToken
            );

            return item.Id;
        }

        public async Task UpdateAssistiveDeviceAsync(
            UpdateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = await _assistiveDeviceRepository.GetByIdAsync(
                command.Id,
                cancellationToken
            );

            if (item is null)
                throw new UseCaseException("Không tìm thấy thiết bị hỗ trợ.");

            item.Update(
                command.Name,
                command.Description
            );

            await _assistiveDeviceRepository.UpdateAsync(
                item,
                cancellationToken
            );
        }

        public async Task DeactivateAssistiveDeviceAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = await _assistiveDeviceRepository.GetByIdAsync(
                id,
                cancellationToken
            );

            if (item is null)
                throw new UseCaseException("Không tìm thấy thiết bị hỗ trợ.");

            item.Deactivate();

            await _assistiveDeviceRepository.UpdateAsync(
                item,
                cancellationToken
            );
        }

        public async Task ActivateAssistiveDeviceAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = await _assistiveDeviceRepository.GetByIdAsync(
                id,
                cancellationToken
            );

            if (item is null)
                throw new UseCaseException("Không tìm thấy thiết bị hỗ trợ.");

            item.Activate();

            await _assistiveDeviceRepository.UpdateAsync(
                item,
                cancellationToken
            );
        }

        public async Task<IReadOnlyList<CatalogItemResult>> GetActiveAssistiveDevicesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var items = await _assistiveDeviceRepository.GetActiveAsync(
                cancellationToken
            );

            return items
                .Select(CatalogMapper.ToResult)
                .ToList();
        }

        // =========================
        // Job Categories
        // =========================

        public async Task<Guid> CreateJobCategoryAsync(
            CreateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = JobCategory.Create(
                command.Name,
                command.Description
            );

            await _jobCategoryRepository.AddAsync(
                item,
                cancellationToken
            );

            return item.Id;
        }

        public async Task UpdateJobCategoryAsync(
            UpdateCatalogItemCommand command,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = await _jobCategoryRepository.GetByIdAsync(
                command.Id,
                cancellationToken
            );

            if (item is null)
                throw new UseCaseException("Không tìm thấy ngành nghề.");

            item.Update(
                command.Name,
                command.Description
            );

            await _jobCategoryRepository.UpdateAsync(
                item,
                cancellationToken
            );
        }

        public async Task DeactivateJobCategoryAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = await _jobCategoryRepository.GetByIdAsync(
                id,
                cancellationToken
            );

            if (item is null)
                throw new UseCaseException("Không tìm thấy ngành nghề.");

            item.Deactivate();

            await _jobCategoryRepository.UpdateAsync(
                item,
                cancellationToken
            );
        }

        public async Task ActivateJobCategoryAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            AuthorizationGuard.RequireAdmin(_currentUser);

            var item = await _jobCategoryRepository.GetByIdAsync(
                id,
                cancellationToken
            );

            if (item is null)
                throw new UseCaseException("Không tìm thấy ngành nghề.");

            item.Activate();

            await _jobCategoryRepository.UpdateAsync(
                item,
                cancellationToken
            );
        }

        public async Task<IReadOnlyList<CatalogItemResult>> GetActiveJobCategoriesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var items = await _jobCategoryRepository.GetActiveAsync(
                cancellationToken
            );

            return items
                .Select(CatalogMapper.ToResult)
                .ToList();
        }
    }
}
