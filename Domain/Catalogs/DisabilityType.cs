using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Catalogs
{
    public sealed class DisabilityType : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public CatalogStatus Status { get; private set; }

        private DisabilityType(
            Guid id,
            string name,
            string? description
        ) : base(id)
        {
            Name = name;
            Description = description;
            Status = CatalogStatus.Active;
        }

        public static DisabilityType Create(
            string name,
            string? description
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên loại khuyết tật không được để trống.");

            return new DisabilityType(
                Guid.NewGuid(),
                name.Trim(),
                description
            );
        }

        public void Update(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên loại khuyết tật không được để trống.");

            Name = name.Trim();
            Description = description;
        }

        public void Activate()
        {
            Status = CatalogStatus.Active;
        }

        public void Deactivate()
        {
            Status = CatalogStatus.Inactive;
        }
        public static DisabilityType Restore(
    Guid id,
    string name,
    string? description,
    CatalogStatus status)
        {
            if (id == Guid.Empty)
                throw new DomainException("DisabilityTypeId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên loại khuyết tật không được để trống.");

            var item = new DisabilityType(id, name.Trim(), description);
            item.Status = status;

            return item;
        }
    }
}
