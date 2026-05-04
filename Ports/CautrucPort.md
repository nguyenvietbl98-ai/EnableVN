EnableVN.Ports
│
├── Inbound
│   ├── IAuthUseCase.cs
│   ├── IUserUseCase.cs
│   ├── IEmployerProfileUseCase.cs
│   ├── ICandidateProfileUseCase.cs
│   ├── IJobUseCase.cs
│   ├── IJobApplicationUseCase.cs
│   └── ICatalogUseCase.cs
│
├── Outbound
│   ├── Repositories
│   │   ├── IUserRepository.cs
│   │   ├── IEmployerProfileRepository.cs
│   │   ├── ICandidateProfileRepository.cs
│   │   ├── IJobRepository.cs
│   │   ├── IJobApplicationRepository.cs
│   │   ├── IDisabilityTypeRepository.cs
│   │   ├── IAssistiveDeviceRepository.cs
│   │   └── IJobCategoryRepository.cs
│   │
│   └── Services
│       ├── ICurrentUserService.cs
│       ├── IPasswordHasher.cs
│       ├── IEmailService.cs
│       ├── IFileStorageService.cs
│       └── IDomainEventDispatcher.cs
│
└── Models
    ├── Auth
    ├── Users
    ├── Employers
    ├── Candidates
    ├── Jobs
    ├── Applications
    └── Catalogs