using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Catalogs
{
    public sealed class AssistiveDevice : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public CatalogStatus Status { get; private set; }

        private AssistiveDevice(
            Guid id,
            string name,
            string? description
        ) : base(id)
        {
            Name = name;
            Description = description;
            Status = CatalogStatus.Active;
        }

        public static AssistiveDevice Create(
            string name,
            string? description
        )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên thiết bị hỗ trợ không được để trống.");

            return new AssistiveDevice(
                Guid.NewGuid(),
                name.Trim(),
                description
            );
        }

        public void Update(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên thiết bị hỗ trợ không được để trống.");

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
    }
}
