EnableVN.Presentation
│
├── Controllers
│   ├── HomeController.cs
│   ├── AuthController.cs
│   ├── JobsController.cs
│   ├── EmployerProfileController.cs
│   ├── CandidateProfileController.cs
│   ├── EmployerJobsController.cs
│   └── JobApplicationsController.cs
│
├── Services
│   └── SessionCurrentUserService.cs
│
├── Views
│   ├── Shared
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   │
│   ├── Home
│   │   └── Index.cshtml
│   │
│   ├── Auth
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   │
│   ├── Jobs
│   │   ├── Index.cshtml
│   │   └── Details.cshtml
│   │
│   └── EmployerJobs
│       ├── Index.cshtml
│       └── Create.cshtml
│
├── wwwroot
│   └── css
│       └── site.css
│
├── Program.cs
├── _ViewImports.cshtml
└── _ViewStart.cshtml