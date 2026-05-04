using Domain.Catalogs;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// Repository lưu DisabilityType bằng RAM.
    /// 
    /// Dùng cho danh mục loại khuyết tật do Admin quản lý.
    /// </summary>
    public sealed class InMemoryDisabilityTypeRepository : IDisabilityTypeRepository
    {
        private readonly List<DisabilityType> _items = new();

        public Task<DisabilityType?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var item = _items.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(item);
        }

        public Task<IReadOnlyList<DisabilityType>> GetAllAsync(
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<DisabilityType> result = _items.ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<DisabilityType>> GetActiveAsync(
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<DisabilityType> result = _items
                .Where(x => x.Status == CatalogStatus.Active)
                .ToList();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            DisabilityType disabilityType,
            CancellationToken cancellationToken = default
        )
        {
            _items.Add(disabilityType);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            DisabilityType disabilityType,
            CancellationToken cancellationToken = default
        )
        {
            var index = _items.FindIndex(x => x.Id == disabilityType.Id);

            if (index >= 0)
            {
                _items[index] = disabilityType;
            }

            return Task.CompletedTask;
        }
    }
}
