using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Employers.Events;

namespace Domain.Employers
{
    public sealed class EmployerProfile : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public CompanyName CompanyName { get; private set; }
        public string? Description { get; private set; }
        public string? WebsiteUrl { get; private set; }
        public InclusiveWorkplaceInfo WorkplaceInfo { get; private set; }

        private EmployerProfile(
            Guid id,
            Guid userId,
            CompanyName companyName,
            string? description,
            string? websiteUrl,
            InclusiveWorkplaceInfo workplaceInfo
        ) : base(id)
        {
            UserId = userId;
            CompanyName = companyName;
            Description = description;
            WebsiteUrl = websiteUrl;
            WorkplaceInfo = workplaceInfo;
        }

        public static EmployerProfile Create(
            Guid userId,
            string companyName,
            string? description,
            string? websiteUrl,
            InclusiveWorkplaceInfo workplaceInfo
        )
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId của nhà tuyển dụng không hợp lệ.");

            var profile = new EmployerProfile(
                Guid.NewGuid(),
                userId,
                CompanyName.Create(companyName),
                description,
                websiteUrl,
                workplaceInfo
            );

            profile.AddDomainEvent(
                new EmployerProfileCreatedEvent(profile.Id, userId)
            );

            return profile;
        }

        public void UpdateCompanyInfo(
            string companyName,
            string? description,
            string? websiteUrl
        )
        {
            CompanyName = CompanyName.Create(companyName);
            Description = description;
            WebsiteUrl = websiteUrl;
        }

        public void UpdateInclusiveWorkplaceInfo(InclusiveWorkplaceInfo workplaceInfo)
        {
            WorkplaceInfo = workplaceInfo;
        }
    }
}
