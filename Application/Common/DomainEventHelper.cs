using Domain.Common;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    /// <summary>
    /// Helper xử lý Domain Event sau khi repository lưu dữ liệu.
    /// 
    /// Domain chỉ tạo event.
    /// Application quyết định thời điểm dispatch event.
    /// </summary>
    public static class DomainEventHelper
    {
        /// <summary>
        /// Dispatch toàn bộ domain event trong aggregate rồi clear event.
        /// </summary>
        public static async Task DispatchAndClearEventsAsync<TId>(
            AggregateRoot<TId> aggregate,
            IDomainEventDispatcher dispatcher,
            CancellationToken cancellationToken = default
        )
        {
            var events = aggregate.DomainEvents.ToList();

            if (events.Count == 0)
                return;

            await dispatcher.DispatchAsync(events, cancellationToken);

            aggregate.ClearDomainEvents();
        }
    }
}
