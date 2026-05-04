using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Jobs.Events;

namespace Domain.Jobs
{
    public sealed class JobPost : AggregateRoot<Guid>
    {
        public Guid EmployerId { get; private set; }
        public JobTitle Title { get; private set; }
        public string Description { get; private set; }
        public string Requirement { get; private set; }
        public WorkMode WorkMode { get; private set; }
        public SalaryRange SalaryRange { get; private set; }
        public JobAccessibilityInfo AccessibilityInfo { get; private set; }
        public JobStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }

        private JobPost(
            Guid id,
            Guid employerId,
            JobTitle title,
            string description,
            string requirement,
            WorkMode workMode,
            SalaryRange salaryRange,
            JobAccessibilityInfo accessibilityInfo
        ) : base(id)
        {
            EmployerId = employerId;
            Title = title;
            Description = description;
            Requirement = requirement;
            WorkMode = workMode;
            SalaryRange = salaryRange;
            AccessibilityInfo = accessibilityInfo;
            Status = JobStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public static JobPost CreateDraft(
            Guid employerId,
            string title,
            string description,
            string requirement,
            WorkMode workMode,
            SalaryRange salaryRange,
            JobAccessibilityInfo accessibilityInfo
        )
        {
            if (employerId == Guid.Empty)
                throw new DomainException("EmployerId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Mô tả công việc không được để trống.");

            if (string.IsNullOrWhiteSpace(requirement))
                throw new DomainException("Yêu cầu công việc không được để trống.");

            return new JobPost(
                Guid.NewGuid(),
                employerId,
                JobTitle.Create(title),
                description.Trim(),
                requirement.Trim(),
                workMode,
                salaryRange,
                accessibilityInfo
            );
        }

        public void UpdateContent(
            string title,
            string description,
            string requirement,
            WorkMode workMode,
            SalaryRange salaryRange,
            JobAccessibilityInfo accessibilityInfo
        )
        {
            if (Status == JobStatus.Deleted)
                throw new DomainException("Không thể chỉnh sửa tin tuyển dụng đã bị xóa.");

            Title = JobTitle.Create(title);
            Description = description.Trim();
            Requirement = requirement.Trim();
            WorkMode = workMode;
            SalaryRange = salaryRange;
            AccessibilityInfo = accessibilityInfo;
        }

        public void Publish()
        {
            if (Status == JobStatus.Deleted)
                throw new DomainException("Không thể đăng tin đã bị xóa.");

            if (Status == JobStatus.Published)
                throw new DomainException("Tin tuyển dụng đã được đăng.");

            Status = JobStatus.Published;
            PublishedAt = DateTime.UtcNow;

            AddDomainEvent(new JobPostedEvent(Id, EmployerId));
        }

        public void Close()
        {
            if (Status != JobStatus.Published)
                throw new DomainException("Chỉ có thể đóng tin đang được đăng.");

            Status = JobStatus.Closed;
            ClosedAt = DateTime.UtcNow;

            AddDomainEvent(new JobClosedEvent(Id, EmployerId));
        }

        public void Delete()
        {
            Status = JobStatus.Deleted;
        }

        public bool CanReceiveApplication()
        {
            return Status == JobStatus.Published;
        }
    }
}
