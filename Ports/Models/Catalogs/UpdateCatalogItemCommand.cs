using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Catalogs
{
    /// <summary>
    /// Command dùng chung để cập nhật dữ liệu danh mục.
    /// </summary>
    public sealed class UpdateCatalogItemCommand
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }
    }
}
