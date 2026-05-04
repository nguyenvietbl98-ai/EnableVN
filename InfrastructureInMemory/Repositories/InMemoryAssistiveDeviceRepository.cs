using Domain.Catalogs;
using Ports.Outbound.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// Repository lưu AssistiveDevice bằng RAM.
    /// 
    /// Dùng cho danh mục thiết bị hỗ trợ.
    /// </summary>
    public sealed class InMemoryAssistiveDeviceRepository : IAssistiveDeviceRepository
    {
        private readonly List<AssistiveDevice> _items = new();

        public Task<AssistiveDevice?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var item = _items.FirstOrDefault(x => x.Id == id);

            return Task.FromResult(item);
        }

        public Task<IReadOnlyList<AssistiveDevice>> GetAllAsync(
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<AssistiveDevice> result = _items.ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<AssistiveDevice>> GetActiveAsync(
            CancellationToken cancellationToken = default
        )
        {
            IReadOnlyList<AssistiveDevice> result = _items
                .Where(x => x.Status == CatalogStatus.Active)
                .ToList();

            return Task.FromResult(result);
        }

        public Task AddAsync(
            AssistiveDevice assistiveDevice,
            CancellationToken cancellationToken = default
        )
        {
            _items.Add(assistiveDevice);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            AssistiveDevice assistiveDevice,
            CancellationToken cancellationToken = default
        )
        {
            var index = _items.FindIndex(x => x.Id == assistiveDevice.Id);

            if (index >= 0)
            {
                _items[index] = assistiveDevice;
            }

            return Task.CompletedTask;
        }
    }
}
