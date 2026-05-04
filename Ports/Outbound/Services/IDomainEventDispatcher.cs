using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Outbound.Services
{
    /// <summary>
    /// Outbound Port dùng để dispatch Domain Event.
    /// 
    /// Domain chỉ tạo event.
    /// Application hoặc Infrastructure sẽ quyết định xử lý event như thế nào.
    /// 
    /// Ví dụ:
    /// - Gửi email
    /// - Tạo notification
    /// - Ghi audit log
    /// - Đồng bộ dữ liệu sang search engine
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Dispatch một domain event.
        /// </summary>
        Task DispatchAsync(
            IDomainEvent domainEvent,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Dispatch nhiều domain event cùng lúc.
        /// Thường dùng sau khi lưu Aggregate thành công.
        /// </summary>
        Task DispatchAsync(
            IEnumerable<IDomainEvent> domainEvents,
            CancellationToken cancellationToken = default
        );
    }
}
