using Domain.Catalogs;
using Ports.Models.Catalogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mappers
{
    /// <summary>
    /// Mapper cho dữ liệu danh mục.
    /// </summary>
    public static class CatalogMapper
    {
        public static CatalogItemResult ToResult(DisabilityType item)
        {
            return new CatalogItemResult
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status
            };
        }

        public static CatalogItemResult ToResult(AssistiveDevice item)
        {
            return new CatalogItemResult
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status
            };
        }

        public static CatalogItemResult ToResult(JobCategory item)
        {
            return new CatalogItemResult
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status
            };
        }
    }
}
