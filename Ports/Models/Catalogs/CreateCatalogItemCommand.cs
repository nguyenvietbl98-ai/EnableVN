using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Catalogs
{
    /// <summary>
    /// Command dùng chung để tạo dữ liệu danh mục đơn giản.
    /// Ví dụ:
    /// - DisabilityType
    /// - AssistiveDevice
    /// - JobCategory
    /// </summary>
    public sealed class CreateCatalogItemCommand
    {
        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }
    }
}
