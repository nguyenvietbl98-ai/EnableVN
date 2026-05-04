using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Applications
{
    public sealed class ApplicationStatusHistory
    {
        public ApplicationStatus Status { get; private set; }
        public string? Note { get; private set; }
        public DateTime ChangedAt { get; private set; }

        private ApplicationStatusHistory(
            ApplicationStatus status,
            string? note,
            DateTime changedAt
        )
        {
            Status = status;
            Note = note;
            ChangedAt = changedAt;
        }

        public static ApplicationStatusHistory Create(
            ApplicationStatus status,
            string? note
        )
        {
            return new ApplicationStatusHistory(
                status,
                note,
                DateTime.UtcNow
            );
        }
    }
}
