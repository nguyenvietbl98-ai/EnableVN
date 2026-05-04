using Domain.Catalogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ports.Models.Catalogs
{
    /// <summary>
    /// Result trả về cho dữ liệu danh mục.
    /// </summary>
    public sealed class CatalogItemResult
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public CatalogStatus Status { get; init; }
    }
}
