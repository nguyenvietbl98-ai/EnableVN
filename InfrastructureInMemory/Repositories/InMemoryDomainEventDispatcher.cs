using Domain.Common;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory.Repositories
{
    /// <summary>
    /// DomainEventDispatcher bản InMemory.
    /// 
    /// Hiện tại chỉ lưu lại các event đã dispatch vào RAM.
    /// Dùng để debug/test xem event có được tạo và dispatch hay không.
    /// 
    /// Sau này Infrastructure thật có thể:
    /// - Gửi email
    /// - Tạo notification
    /// - Ghi audit log
    /// - Publish message queue
    /// </summary>
    public sealed class InMemoryDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly List<IDomainEvent> _dispatchedEvents = new();

        public IReadOnlyList<IDomainEvent> DispatchedEvents => _dispatchedEvents.AsReadOnly();

        public Task DispatchAsync(
            IDomainEvent domainEvent,
            CancellationToken cancellationToken = default
        )
        {
            _dispatchedEvents.Add(domainEvent);

            return Task.CompletedTask;
        }

        public Task DispatchAsync(
            IEnumerable<IDomainEvent> domainEvents,
            CancellationToken cancellationToken = default
        )
        {
            _dispatchedEvents.AddRange(domainEvents);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Xóa danh sách event đã dispatch.
        /// Hữu ích khi viết test.
        /// </summary>
        public void Clear()
        {
            _dispatchedEvents.Clear();
        }
    }
}
