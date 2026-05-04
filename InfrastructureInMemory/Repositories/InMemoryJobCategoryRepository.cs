using Domain.Catalogs;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// Repository lưu JobCategory bằng RAM.
    /// 
    /// Dùng cho danh mục ngành nghề / nhóm công việc.
    /// </summary>
    public sealed class InMemoryJobCategoryRepository : IJobCategoryRepository
    {
        private readonly List<JobCategory> _items = new();

        public Task<JobCategory?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var item = _items.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(item);
        }

        public Task<IReadOnlyList<JobCategory>> GetAllAsync(
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<JobCategory> result = _items.ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<JobCategory>> GetActiveAsync(
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<JobCategory> result = _items
                .Where(x => x.Status == CatalogStatus.Active)
                .ToList();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            JobCategory category,
            CancellationToken cancellationToken = default
        )
        {
            _items.Add(category);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            JobCategory category,
            CancellationToken cancellationToken = default
        )
        {
            var index = _items.FindIndex(x => x.Id == category.Id);

            if (index >= 0)
            {
                _items[index] = category;
            }

            return Task.CompletedTask;
        }
    }
}
