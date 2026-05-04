using Domain.Applications.Policies;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Applications.Events;

namespace Domain.Applications
{
    public sealed class JobApplication : AggregateRoot<Guid>
    {
        private readonly List<ApplicationStatusHistory> _statusHistories = new();

        public Guid JobId { get; private set; }
        public Guid CandidateId { get; private set; }
        public string? CoverLetter { get; private set; }
        public string? CvUrl { get; private set; }
        public ApplicationStatus Status { get; private set; }
        public DateTime SubmittedAt { get; private set; }

        public IReadOnlyCollection<ApplicationStatusHistory> StatusHistories
            => _statusHistories.AsReadOnly();

        private JobApplication(
            Guid id,
            Guid jobId,
            Guid candidateId,
            string? coverLetter,
            string? cvUrl
        ) : base(id)
        {
            JobId = jobId;
            CandidateId = candidateId;
            CoverLetter = coverLetter;
            CvUrl = cvUrl;
            Status = ApplicationStatus.Pending;
            SubmittedAt = DateTime.UtcNow;

            _statusHistories.Add(
                ApplicationStatusHistory.Create(ApplicationStatus.Pending, "Ứng viên đã nộp hồ sơ.")
            );
        }

        public static JobApplication Submit(
            Guid jobId,
            Guid candidateId,
            string? coverLetter,
            string? cvUrl
        )
        {
            if (jobId == Guid.Empty)
                throw new DomainException("JobId không hợp lệ.");

            if (candidateId == Guid.Empty)
                throw new DomainException("CandidateId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(cvUrl))
                throw new DomainException("Ứng viên cần có CV để nộp hồ sơ.");

            var application = new JobApplication(
                Guid.NewGuid(),
                jobId,
                candidateId,
                coverLetter,
                cvUrl
            );

            application.AddDomainEvent(
                new JobApplicationSubmittedEvent(
                    application.Id,
                    jobId,
                    candidateId
                )
            );

            return application;
        }

        public void ChangeStatus(
            ApplicationStatus newStatus,
            string? note
        )
        {
            ApplicationStatusPolicy.EnsureCanChangeStatus(Status, newStatus);

            var oldStatus = Status;
            Status = newStatus;

            _statusHistories.Add(
                ApplicationStatusHistory.Create(newStatus, note)
            );

            AddDomainEvent(
                new JobApplicationStatusChangedEvent(
                    Id,
                    JobId,
                    CandidateId,
                    oldStatus,
                    newStatus
                )
            );
        }

        public void Withdraw()
        {
            if (Status == ApplicationStatus.Accepted)
                throw new DomainException("Không thể rút hồ sơ đã được chấp nhận.");

            if (Status == ApplicationStatus.Rejected)
                throw new DomainException("Không cần rút hồ sơ đã bị từ chối.");

            Status = ApplicationStatus.Withdrawn;

            _statusHistories.Add(
                ApplicationStatusHistory.Create(
                    ApplicationStatus.Withdrawn,
                    "Ứng viên đã rút hồ sơ."
                )
            );
        }

        public static JobApplication Restore(
            Guid id,
            Guid jobId,
            Guid candidateId,
            string? coverLetter,
            string? cvUrl,
            ApplicationStatus status,
            DateTime submittedAt,
            IReadOnlyList<ApplicationStatusHistory> statusHistories
        )
        {
            if (id == Guid.Empty)
                throw new DomainException("JobApplicationId không hợp lệ.");

            if (jobId == Guid.Empty)
                throw new DomainException("JobId không hợp lệ.");

            if (candidateId == Guid.Empty)
                throw new DomainException("CandidateId không hợp lệ.");

            var application = new JobApplication(
                id,
                jobId,
                candidateId,
                coverLetter,
                cvUrl
            );

            application.Status = status;
            // Clear default history added by constructor
            application._statusHistories.Clear();
            // Add restored histories
            foreach (var history in statusHistories)
            {
                application._statusHistories.Add(history);
            }

            return application;
        }
    }
}
