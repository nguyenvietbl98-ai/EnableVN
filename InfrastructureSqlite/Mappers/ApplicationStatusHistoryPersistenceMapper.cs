using InfrastructureSqlite.PersistenceModels;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Applications;

namespace InfrastructureSqlite.Mappers
{
    public static class ApplicationStatusHistoryPersistenceMapper
    {
        public static ApplicationStatusHistoryRecord ToRecord(
            Guid jobApplicationId,
            ApplicationStatusHistory history)
        {
            return new ApplicationStatusHistoryRecord
            {
                Id = Guid.NewGuid(),
                JobApplicationId = jobApplicationId,
                Status = history.Status.ToString(),
                Note = history.Note,
                ChangedAt = history.ChangedAt
            };
        }

        public static ApplicationStatusHistory ToDomain(ApplicationStatusHistoryRecord record)
        {
            var status = Enum.Parse<ApplicationStatus>(record.Status);
            return ApplicationStatusHistory.Restore(status, record.Note, record.ChangedAt);
        }
    }
}
