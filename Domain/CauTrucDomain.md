EnableVN.Domain
│
├── Common
│   ├── Entity.cs
│   ├── AggregateRoot.cs
│   ├── ValueObject.cs
│   ├── IDomainEvent.cs
│   ├── DomainException.cs
│   └── Guard.cs
│
├── Users
│   ├── User.cs
│   ├── UserId.cs
│   ├── Email.cs
│   ├── PasswordHash.cs
│   ├── UserRole.cs
│   ├── UserStatus.cs
│   └── Events
│       └── UserRegisteredEvent.cs
│
├── Employers
│   ├── EmployerProfile.cs
│   ├── EmployerId.cs
│   ├── CompanyName.cs
│   ├── CompanySize.cs
│   ├── InclusiveWorkplaceInfo.cs
│   └── Events
│       └── EmployerProfileCreatedEvent.cs
│
├── Candidates
│   ├── CandidateProfile.cs
│   ├── CandidateId.cs
│   ├── FullName.cs
│   ├── DisabilityInfo.cs
│   ├── CandidatePrivacySetting.cs
│   ├── AssistiveNeed.cs
│   └── Events
│       └── CandidateProfileCreatedEvent.cs
│
├── Jobs
│   ├── JobPost.cs
│   ├── JobId.cs
│   ├── JobTitle.cs
│   ├── SalaryRange.cs
│   ├── JobAccessibilityInfo.cs
│   ├── WorkMode.cs
│   ├── JobStatus.cs
│   ├── Policies
│   │   └── JobPostingPolicy.cs
│   └── Events
│       ├── JobPostedEvent.cs
│       └── JobClosedEvent.cs
│
├── Applications
│   ├── JobApplication.cs
│   ├── ApplicationId.cs
│   ├── ApplicationStatus.cs
│   ├── ApplicationStatusHistory.cs
│   ├── Policies
│   │   └── ApplicationStatusPolicy.cs
│   └── Events
│       ├── JobApplicationSubmittedEvent.cs
│       └── JobApplicationStatusChangedEvent.cs
│
└── Catalogs
    ├── DisabilityType.cs
    ├── AssistiveDevice.cs
    ├── JobCategory.cs
    └── CatalogStatus.cs