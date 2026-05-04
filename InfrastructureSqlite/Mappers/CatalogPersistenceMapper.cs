using InfrastructureSqlite.PersistenceModels;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Catalogs;

namespace InfrastructureSqlite.Mappers
{
    public static class CatalogPersistenceMapper
    {
        public static DisabilityTypeRecord ToRecord(DisabilityType disabilityType)
        {
            return new DisabilityTypeRecord
            {
                Id = disabilityType.Id,
                Name = disabilityType.Name,
                Description = disabilityType.Description,
                Status = disabilityType.Status.ToString()
            };
        }

        public static DisabilityType ToDomainDisabilityType(DisabilityTypeRecord record)
        {
            var status = Enum.Parse<CatalogStatus>(record.Status);
            return DisabilityType.Restore(record.Id, record.Name, record.Description, status);
        }

        public static void UpdateRecord(DisabilityTypeRecord record, DisabilityType disabilityType)
        {
            record.Name = disabilityType.Name;
            record.Description = disabilityType.Description;
            record.Status = disabilityType.Status.ToString();
        }

        // AssistiveDevice
        public static AssistiveDeviceRecord ToRecord(AssistiveDevice device)
        {
            return new AssistiveDeviceRecord
            {
                Id = device.Id,
                Name = device.Name,
                Description = device.Description,
                Status = device.Status.ToString()
            };
        }

        public static AssistiveDevice ToDomainAssistiveDevice(AssistiveDeviceRecord record)
        {
            var status = Enum.Parse<CatalogStatus>(record.Status);
            return AssistiveDevice.Restore(record.Id, record.Name, record.Description, status);
        }

        public static void UpdateRecord(AssistiveDeviceRecord record, AssistiveDevice device)
        {
            record.Name = device.Name;
            record.Description = device.Description;
            record.Status = device.Status.ToString();
        }

        // JobCategory
        public static JobCategoryRecord ToRecord(JobCategory category)
        {
            return new JobCategoryRecord
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status.ToString()
            };
        }

        public static JobCategory ToDomainJobCategory(JobCategoryRecord record)
        {
            var status = Enum.Parse<CatalogStatus>(record.Status);
            return JobCategory.Restore(record.Id, record.Name, record.Description, status);
        }

        public static void UpdateRecord(JobCategoryRecord record, JobCategory category)
        {
            record.Name = category.Name;
            record.Description = category.Description;
            record.Status = category.Status.ToString();
        }
    }
}
