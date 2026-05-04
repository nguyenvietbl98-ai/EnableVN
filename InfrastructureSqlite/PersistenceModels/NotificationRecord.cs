using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureSqlite.PersistenceModels
{
    // Record này đại diện cho bảng Notifications trong SQLite.
    public sealed class NotificationRecord
    {
        public Guid Id { get; set; } // Primary key.

        public Guid UserId { get; set; } // Id người nhận thông báo.

        public string Title { get; set; } = string.Empty; // Tiêu đề.

        public string Message { get; set; } = string.Empty; // Nội dung.

        public string Type { get; set; } = string.Empty; // Lưu enum dạng string.

        public string Status { get; set; } = string.Empty; // Lưu enum dạng string.

        public DateTime CreatedAt { get; set; } // Ngày tạo.

        public DateTime? ReadAt { get; set; } // Ngày đọc, null nếu chưa đọc.
    }
}
