# EnableVN — Tổng kết trạng thái dự án (Bản 8)

**Ngày cập nhật:** 05/05/2026  
**Đồng bộ:** Các file `EnableVN_*Summary_1..7*.md`, `SQLITE_IMPLEMENTATION_SUMMARY_6.md` và bản 8 đã được **rà soát và chỉnh trực tiếp** cho khớp code trong cùng đợt cập nhật.  
**Mục đích:** Làm “điểm neo” cho phiên làm việc tiếp theo — bám **code trong repo**, không thay thế bằng tài liệu cũ.

---

## 0. Nguyên tắc đọc dự án (đã thống nhất và mở rộng)

Thứ tự ưu tiên khi đối chiếu tài liệu với thực tế:

```text
Code hiện tại > Bản Summary 8 (file này) > Summary 7 > 6 > 5 > 4 > 3 > 2 > 1
```

**Chuỗi summary lịch sử (theo thời gian phát triển trong repo):**

| # | File |
|---|------|
| 1 | `EnableVN_Domain_Summary_1.md` |
| 2 | `EnableVN_Ports_2.md` |
| 3 | `EnableVN_Application_Summary_3.md` |
| 4 | `EnableVN_InfrastructureInMemory_Summary_4.md` |
| 5 | `EnableVN_Presentation_Summary_5.md` |
| 6 | `SQLITE_IMPLEMENTATION_SUMMARY_6.md` |
| 7 | `EnableVN_Phase2_Session_Summary_7.md` |
| **8** | **`EnableVN_Final_Project_Summary_8.md` (file này)** |

**Ghi chú so với Summary 7:** Summary 7 kết thúc ở trạng thái “đang triển khai Notification + hướng dẫn migration”. Trong code hiện tại đã có migration `AddNotifications`, tích hợp `JobApplicationUseCase`, UI Notifications, và nhiều phần Giai đoạn 2 khác (xem mục 4–5).

---

## 1. Solution và kiến trúc

**File solution:** `ENABLEVN.slnx`

**Các project (theo `ENABLEVN.slnx`):**

| Project | Vai trò |
|---------|---------|
| `Domain` | Entity, value object, policy, event — không phụ thuộc EF/UI |
| `Ports` | Inbound ports (UseCase interfaces), outbound ports (repository/service interfaces), DTO/command, **và** hai lớp InMemory `InMemoryCompanyReviewRepository`, `InMemoryViolationReportRepository` (file vật lý nằm dưới `Ports/Outbound/Repositories/`, namespace `InfrastructureInMemory.Repositories`) |
| `Application` | Use case + mapper + `DependencyInjection` |
| `InfrastructureInMemory` | Repository InMemory (Singleton), `SmtpEmailService`, password/token, domain event dispatcher |
| `InfrastructureSqlite` | EF Core SQLite: DbContext, persistence model, mapper, repository, migration, seed |
| `ENABLEVN` (`Presentation.csproj`) | ASP.NET Core MVC + Razor, SignalR, dịch vụ AI/moderation, session |

**Chiều phụ thuộc thực tế (đúng với .csproj):** Presentation → Application, InfrastructureInMemory, InfrastructureSqlite, Ports; Application → Ports; Infrastructure* → Domain + Ports; Ports → Domain.

---

## 2. Build và mức độ chắc chắn

- **Đã chạy:** `dotnet build` trên `ENABLEVN/Presentation.csproj` — **thành công**, 0 error (05/05/2026).
- **Cảnh báo:** `NU1903` — package `System.Security.Cryptography.Xml` 9.0.0 (transitive) có advisory — **cần xử lý dependency** (`dotnet list package --vulnerable`) trước khi coi là production-sạch.
- **Migration:** Trong `Development`, `Program.cs` gọi `MigrateAsync()` khi khởi động — **đã có cấu trúc + đã build**; môi trường khác **cần chiến lược apply migration** tương đương (không tự động trong đoạn code đã đọc cho non-Development).
- **Chạy end-to-end:** Không chạy `dotnet run`/test tự động trong phiên này — luồng nghiệp vụ **đã có đủ controller/use case**; một số DI thiếu (mục 10) **cần sửa trước khi xác nhận runtime** cho module báo cáo.

---

## 3. Domain (đối chiếu code)

**Thư mục và file `.cs` (43 file trong `Domain/`):**

- `Common/`: `AggregateRoot.cs`, `DomainException.cs`, `Entity.cs`, `IDomainEvent.cs`, `ValueObject.cs`
- `Users/`: `User.cs`, `Email.cs`, `UserRole.cs`, `UserStatus.cs`, `Events/UserRegisteredEvent.cs`
- `Employers/`: `EmployerProfile.cs`, `CompanyName.cs`, `InclusiveWorkplaceInfo.cs`, `Events/EmployerProfileCreatedEvent.cs`
- `Candidates/`: `CandidateProfile.cs`, `FullName.cs`, `DisabilityInfo.cs`, `Events/CandidateProfileCreatedEvent.cs`
- `Jobs/`: `JobPost.cs`, `JobTitle.cs`, `SalaryRange.cs`, `JobAccessibilityInfo.cs`, `WorkMode.cs`, `JobStatus.cs`, `Events/JobPostedEvent.cs`, `Events/JobClosedEvent.cs`
- `Applications/`: `JobApplication.cs`, `ApplicationStatus.cs`, `ApplicationStatusHistory.cs`, `Events/JobApplicationSubmittedEvent.cs`, `Events/JobApplicationStatusChangedEvent.cs`, `Policies/ApplicationStatusPolicy.cs`
- `Catalogs/`: `DisabilityType.cs`, `AssistiveDevice.cs`, `JobCategory.cs`, `CatalogStatus.cs`
- `Notifications/`: `Notification.cs`, `NotificationType.cs`, `NotificationStatus.cs`
- `Reviews/`: `CompanyReview.cs`
- `Reports/`: `ViolationReport.cs`, `ReportStatus.cs`, `ReportTargetType.cs`

**So với Summary 1 / Ports_2 (chỉ tài liệu cũ):**

- **`JobPostingPolicy.cs`:** Summary 1 và `EnableVN_Ports_2.md` có nhắc đường dẫn `Domain/Jobs/Policies/JobPostingPolicy.cs` — **trong code hiện tại không còn file này**; logic đăng tin nằm trong `JobPost` và use case tương ứng. Ghi nhận: *summary cũ có nhắc, code hiện tại không còn policy file tách riêng đó.*

---

## 4. Ports — Inbound / Outbound / Models

**Inbound (`Ports/Inbound/`) — 11 interface:**

`IAuthUseCase`, `IUserUseCase`, `IEmployerProfileUseCase`, `ICandidateProfileUseCase`, `IJobUseCase`, `IJobApplicationUseCase`, `ICatalogUseCase`, `INotificationUseCase`, `IEmployerCandidateSearchUseCase`, `ICompanyReviewUseCase`, `IViolationReportUseCase`

**Outbound repository interfaces (`Ports/Outbound/Repositories/I*.cs`):**

`IUserRepository`, `IEmployerProfileRepository`, `ICandidateProfileRepository`, `IJobRepository`, `IJobApplicationRepository`, `IDisabilityTypeRepository`, `IAssistiveDeviceRepository`, `IJobCategoryRepository`, `INotificationRepository`, `IApplicationChatRepository`, `ICompanyReviewRepository`, `IViolationReportRepository`

**Outbound services (`Ports/Outbound/Services/`):**

`ICurrentUserService`, `IPasswordHasher`, `ITokenService`, `IDomainEventDispatcher`, `IEmailService`, `IFileStorageService`

- **`IFileStorageService`:** Chỉ có interface trong `Ports` — **không thấy implementation hay đăng ký DI** trong các project đã quét. Coi là **API dự trữ / chưa triển khai**.

**Models:** Auth, Employers, Candidates, Jobs, Applications, Catalogs, Notifications, Reports, Reviews, Chat (`ApplicationChatMessageDto`, `ApplicationChatThreadDto`).

**In-memory đặc biệt:** `InMemoryCompanyReviewRepository.cs`, `InMemoryViolationReportRepository.cs` biên dịch trong project **Ports** (namespace `InfrastructureInMemory.Repositories`).

---

## 5. Application — Use case và đăng ký DI

**Use case (`Application/UseCases/`):**

`AuthUseCase`, `UserUseCase`, `EmployerProfileUseCase`, `CandidateProfileUseCase`, `JobUseCase`, `JobApplicationUseCase`, `CatalogUseCase`, `NotificationUseCase`, `EmployerCandidateSearchUseCase`, `CompanyReviewUseCase`, `ViolationReportUseCase`

**Mapper (`Application/Mappers/`):**

`CandidateProfileMapper`, `CatalogMapper`, `EmployerProfileMapper`, `JobMapper`, `JobApplicationMapper`, `NotificationMapper`

**Common:** `AuthorizationGuard`, `DomainEventHelper`, `UseCaseException`

**`Application/DependencyInjection.cs` đăng ký:** 10 use case (`IAuthUseCase` … `ICompanyReviewUseCase`, bao gồm `IEmployerCandidateSearchUseCase`).

**Thiếu so với interface có implementation trong cùng assembly:**

- **`IViolationReportUseCase` / `ViolationReportUseCase` không được `AddScoped` trong `Application/DependencyInjection.cs`.**  
  Đây là **lỗ hổng runtime** cho `ViolationReportsController` và `AdminDashboardController` (cả hai inject `IViolationReportUseCase`). Build vẫn pass vì DI không kiểm tra tại compile-time.

**`JobApplicationUseCase` (đã đọc một phần):**

- SQLite repositories + `INotificationRepository`: tạo thông báo khi nộp hồ sơ / đổi trạng thái (khớp hướng Summary 7).
- `IEmailService`: có gọi `SendNotificationEmailBestEffortAsync` — gửi email **best-effort** (cần cấu hình SMTP trong `appsettings` để xác nhận hành vi thật).
- Chat: interface `IJobApplicationUseCase` có `EnsureCurrentUserCanChatOnApplicationAsync`, `GetChatThreadForCurrentUserAsync` (và các method ứng tuyển khác) — **đã có trong code**; **cần chạy app + migration** để xác nhận tích hợp SignalR + SQLite chat.

---

## 6. InfrastructureInMemory

**Repository / service (file vật lý trong `InfrastructureInMemory/`):**

`InMemoryUserRepository`, `InMemoryEmployerProfileRepository`, `InMemoryCandidateProfileRepository`, `InMemoryJobRepository`, `InMemoryJobApplicationRepository`, `InMemoryDisabilityTypeRepository`, `InMemoryAssistiveDeviceRepository`, `InMemoryJobCategoryRepository`, `InMemoryCurrentUserService`, `InMemoryDomainEventDispatcher`, `SimplePasswordHasher`, `SimpleTokenService`, `SmtpEmailService` (trong `Services/`).

**Lưu ý:** `InMemoryCompanyReviewRepository` và `InMemoryViolationReportRepository` **không** nằm dưới thư mục `InfrastructureInMemory/` mà nằm trong project **Ports** (`Ports/Outbound/Repositories/…`, namespace `InfrastructureInMemory.Repositories`). `DependencyInjection` của InfrastructureInMemory vẫn đăng ký `ICompanyReviewRepository` → type đó vì **Ports** được reference.

**Đăng ký trong `InfrastructureInMemory/DependencyInjection.cs`:**

- Có `ICompanyReviewRepository` → InMemory.
- **Không có** `IViolationReportRepository` → `InMemoryViolationReportRepository` (dù class đã tồn tại trong Ports). **Cần bổ sung đăng ký** để `ViolationReportUseCase` hoạt động sau khi đã đăng ký use case.

**Ghi đè khi chạy web:** `ICurrentUserService` đăng ký `InMemoryCurrentUserService` trong InMemory, sau đó Presentation ghi đè bằng `SessionCurrentUserService` (`Program.cs`).

---

## 7. InfrastructureSqlite

**DbContext:** `Persistence/EnableVnDbContext.cs`  
**DbSet:** Users, EmployerProfiles, CandidateProfiles, JobPosts, JobApplications, ApplicationStatusHistories, DisabilityTypes, AssistiveDevices, JobCategories, **Notifications**, **ApplicationChatMessages**

**Persistence models (`PersistenceModels/`):**  
`UserRecord`, `EmployerProfileRecord`, `CandidateProfileRecord`, `JobPostRecord`, `JobApplicationRecord`, `ApplicationStatusHistoryRecord`, `DisabilityTypeRecord`, `AssistiveDeviceRecord`, `JobCategoryRecord`, `NotificationRecord`, `ApplicationChatMessageRecord`

**Repositories:** `SqliteUserRepository`, `SqliteEmployerProfileRepository`, `SqliteCandidateProfileRepository`, `SqliteJobRepository`, `SqliteJobApplicationRepository`, `SqliteDisabilityTypeRepository`, `SqliteAssistiveDeviceRepository`, `SqliteJobCategoryRepository`, `SqliteNotificationRepository`, `SqliteApplicationChatRepository`

**Không có trong SQLite:** `CompanyReview`, `ViolationReport` — **không có bảng/repository SQLite**; review vẫn InMemory (Singleton), báo cáo vi phạm: repository InMemory **chưa gắn DI** (mục 6 + 10).

**Migrations (đã có file trong repo):**

1. `20260504170824_InitialSqliteCreate`
2. `20260504181737_AddNotifications`
3. `20260504220206_AddApplicationChatMessages`

**Seed:** `SqliteAdminSeeder`, `SqliteCatalogSeeder` — admin mặc định ghi trong code: `admin@enablevn.local` / `Admin@123` (chỉ nên dùng Development).

**Connection string:** `appsettings.json` — `EnableVnSqlite`: `Data Source=enablevn.db` (file DB cạnh process khi chạy web project).

---

## 8. Presentation (`ENABLEVN/`)

**Khởi tạo (`Program.cs`):**

- MVC + Session + HttpContextAccessor + Antiforgery (header `X-XSRF-TOKEN`).
- `AddEnableVNApplication()`, `AddEnableVNInMemoryInfrastructure()`, `AddEnableVNSqliteInfrastructure()`, `ICurrentUserService` → `SessionCurrentUserService`.
- SignalR + `MapHub<RecruiterChatHub>`.
- Gemini / AI: `GeminiOptions`, `HttpClient` cho `GeminiClient`, `AiRecruitmentService`, `ToxicCommentMlClassifier` (singleton), `ChatModerationService` (scoped).
- Development: `MigrateAsync`, seed admin + catalog.

**Controllers (19 file):**

`AdminDashboard`, `AiAssistant`, `AiRecruitment`, `ApplicationChat`, `Auth`, `CandidateDashboard`, `CandidateProfile`, `Candidates`, `Catalog`, `CompanyReviews`, `Diagnostics`, `EmployerDashboard`, `EmployerJobs`, `EmployerProfile`, `Home`, `JobApplications`, `Jobs`, `Notifications`, `ViolationReports`

**Hub:** `Hubs/RecruiterChatHub.cs` — đường dẫn hub: `/hubs/recruiter-chat`.

**Services / Options:** `SessionCurrentUserService`, `AiRecruitmentService`, `GeminiClient`, `ChatModerationService`, `ToxicCommentMlClassifier`, `GeminiOptions`, `ChatModerationOptions`.

**ViewComponents:** `NotificationBellViewComponent`.

**Views Razor (theo cây thư mục, không kể trùng lặp hiển thị OS):**  
AdminDashboard, AiAssistant, ApplicationChat/Thread, Auth, CandidateDashboard, CandidateProfile (Create/Edit/Index/Disability), Candidates (Index/Details), Catalog, EmployerDashboard, EmployerJobs (CRUD + Index/Details), EmployerProfile (+ partial `_EmployerProfileForm`), Home, JobApplications (ByJob, MyApplications), Jobs (Index, Details), Notifications/Index, Shared (_Layout, _Header, _Footer, _Breadcrumb, _ValidationScriptsPartial, Error, Components/NotificationBell/Default), ViolationReports (Create, Index), `_ViewImports`, `_ViewStart`.

**Ghi chú UI:** Không có thư mục `Views/CompanyReviews/` — `CompanyReviewsController` chỉ có action **POST** `Create` (form đánh giá gắn trên trang Employer profile).

**wwwroot (file ứng dụng tự viết):**  
`css/enablevn.css`, `css/site.css`, `js/site.js`, `js/enablevn-accessibility.js`, `js/ai-recruitment.js`, `js/ai-assistant-page.js`, `js/application-chat.js`  
Cộng thêm thư viện vendor dưới `wwwroot/lib/` (Bootstrap, jQuery, jquery-validation, …) theo chuẩn template.

**API AI:** `AiRecruitmentController` — route prefix `api/ai` (status, chat ứng viên, JD, v.v. — đọc file để mở rộng).

**Cấu hình:** `appsettings.json`, `appsettings.Development.json`, `Properties/launchSettings.json`.

**Bảo mật cấu hình:** Trong `appsettings.json` có section `Gemini` với `ApiKey`. **Không nên commit secret lên repo** — cần chuyển sang User Secrets / biến môi trường và rotate key nếu đã lộ.

**ML:** `Presentation.csproj` copy `MLModels/TextClassifierModel.zip` khi tồn tại — `ChatModeration` mode `ml` cần file model; **cần có file trên disk để xác nhận moderation ML**.

---

## 9. Chức năng nghiệp vụ theo luồng (theo code, không theo summary)

| Nhóm | Trạng thái trong code |
|------|------------------------|
| Đăng ký / đăng nhập / session | `AuthController`, `AuthUseCase`, session keys |
| Admin / Employer / Candidate | Role trong Domain + `AuthorizationGuard` |
| Employer profile + job post | Use case + SQLite |
| Candidate profile, privacy disability | Use case + SQLite |
| Tìm việc, filter accessibility | `JobsController`, `SqliteJobRepository.SearchPublishedJobsAsync` |
| Ứng tuyển, đổi trạng thái, rút hồ sơ | `JobApplicationUseCase` + views |
| Thông báo in-app | Domain + SQLite + `NotificationsController` + bell component |
| Employer tìm ứng viên public | `EmployerCandidateSearchUseCase`, `CandidatesController` |
| Đánh giá doanh nghiệp | `CompanyReviewUseCase`, InMemory repo, POST từ `CompanyReviewsController` |
| Báo cáo vi phạm | Domain + `ViolationReportUseCase` + views/controller — **DI use case + repository chưa đủ** (mục 10) |
| Chat ứng tuyển (SignalR + lưu SQLite) | Hub + `ApplicationChatController` + `IApplicationChatRepository` |
| Trợ lý AI (Gemini) + moderation | API + services + views `AiAssistant` |

**Không thấy:** project test tự động (`*Test*.cs` không có trong repo).

---

## 10. Nợ kỹ thuật / việc nên làm ngay (ưu tiên từ code)

1. **`Application/DependencyInjection.cs`:** thêm `services.AddScoped<IViolationReportUseCase, ViolationReportUseCase>();`
2. **`InfrastructureInMemory/DependencyInjection.cs`:** thêm `services.AddSingleton<IViolationReportRepository, InMemoryViolationReportRepository>();` (hoặc chuyển implementation về đúng project InfrastructureInMemory cho đồng nhất kiến trúc).
3. **SQLite cho ViolationReport / CompanyReview (tuỳ mục tiêu):** hiện review và (khi sửa DI) report dùng RAM — **restart app mất dữ liệu** phần đó; cần thiết kế bảng + migration nếu muốn persistence đồng nhất.
4. **Bảo mật:** đưa Gemini API key và SMTP password ra khỏi file commit; xử lý `NU1903`.
5. **Kiểm thử tay:** toàn luồng sau khi sửa DI — Admin dashboard, tạo/khử báo cáo, chat, notification.

---

## 11. Lệnh tham chiếu (Windows / PowerShell)

Startup project thực tế là **`ENABLEVN/Presentation.csproj`** (không phải `ENABLEVN.csproj`).

```powershell
dotnet build .\ENABLEVN\Presentation.csproj
dotnet run --project .\ENABLEVN\Presentation.csproj
```

Migration (khi thêm mới):

```powershell
dotnet ef migrations add <TenMigration> --project .\InfrastructureSqlite\InfrastructureSqlite.csproj --startup-project .\ENABLEVN\Presentation.csproj --context EnableVnDbContext
dotnet ef database update --project .\InfrastructureSqlite\InfrastructureSqlite.csproj --startup-project .\ENABLEVN\Presentation.csproj --context EnableVnDbContext
```

---

## 12. Phụ lục A — Danh sách đầy đủ file C# nguồn (217 file, đã loại `bin/` và `obj/`)

**Ghi chú đối chiếu:** Bản trước ghi nhầm **181 file**; đếm lại toàn repo (PowerShell, `FullName` không chứa `\bin\` hoặc `\obj\`) = **217** file — **khớp** với số đường dẫn `.cs` trong khối code ngay dưới. Các file **`.csproj` / `.slnx`** không nằm trong phụ lục này vì chỉ liệt kê `.cs`.

Danh sách dưới đây là **sự thật từ repo** tại thời điểm tạo bản summary 8 (sắp xếp theo đường dẫn).

```
Application/Common/AuthorizationGuard.cs
Application/Common/DomainEventHelper.cs
Application/Common/UseCaseException.cs
Application/DependencyInjection.cs
Application/Mappers/CandidateProfileMapper.cs
Application/Mappers/CatalogMapper.cs
Application/Mappers/EmployerProfileMapper.cs
Application/Mappers/JobApplicationMapper.cs
Application/Mappers/JobMapper.cs
Application/Mappers/NotificationMapper.cs
Application/UseCases/AuthUseCase.cs
Application/UseCases/CandidateProfileUseCase.cs
Application/UseCases/CatalogUseCase.cs
Application/UseCases/CompanyReviewUseCase.cs
Application/UseCases/EmployerCandidateSearchUseCase.cs
Application/UseCases/EmployerProfileUseCase.cs
Application/UseCases/JobApplicationUseCase.cs
Application/UseCases/JobUseCase.cs
Application/UseCases/NotificationUseCase.cs
Application/UseCases/UserUseCase.cs
Application/UseCases/ViolationReportUseCase.cs
Domain/Applications/ApplicationStatus.cs
Domain/Applications/ApplicationStatusHistory.cs
Domain/Applications/Events/JobApplicationStatusChangedEvent.cs
Domain/Applications/Events/JobApplicationSubmittedEvent.cs
Domain/Applications/JobApplication.cs
Domain/Applications/Policies/ApplicationStatusPolicy.cs
Domain/Candidates/CandidateProfile.cs
Domain/Candidates/DisabilityInfo.cs
Domain/Candidates/Events/CandidateProfileCreatedEvent.cs
Domain/Candidates/FullName.cs
Domain/Catalogs/AssistiveDevice.cs
Domain/Catalogs/CatalogStatus.cs
Domain/Catalogs/DisabilityType.cs
Domain/Catalogs/JobCategory.cs
Domain/Common/AggregateRoot.cs
Domain/Common/DomainException.cs
Domain/Common/Entity.cs
Domain/Common/IDomainEvent.cs
Domain/Common/ValueObject.cs
Domain/Employers/CompanyName.cs
Domain/Employers/EmployerProfile.cs
Domain/Employers/Events/EmployerProfileCreatedEvent.cs
Domain/Employers/InclusiveWorkplaceInfo.cs
Domain/Jobs/Events/JobClosedEvent.cs
Domain/Jobs/Events/JobPostedEvent.cs
Domain/Jobs/JobAccessibilityInfo.cs
Domain/Jobs/JobPost.cs
Domain/Jobs/JobStatus.cs
Domain/Jobs/JobTitle.cs
Domain/Jobs/SalaryRange.cs
Domain/Jobs/WorkMode.cs
Domain/Notifications/Notification.cs
Domain/Notifications/NotificationStatus.cs
Domain/Notifications/NotificationType.cs
Domain/Reports/ReportStatus.cs
Domain/Reports/ReportTargetType.cs
Domain/Reports/ViolationReport.cs
Domain/Reviews/CompanyReview.cs
Domain/Users/Email.cs
Domain/Users/Events/UserRegisteredEvent.cs
Domain/Users/User.cs
Domain/Users/UserRole.cs
Domain/Users/UserStatus.cs
ENABLEVN/Controllers/AdminDashboardController.cs
ENABLEVN/Controllers/AiAssistantController.cs
ENABLEVN/Controllers/AiRecruitmentController.cs
ENABLEVN/Controllers/ApplicationChatController.cs
ENABLEVN/Controllers/AuthController.cs
ENABLEVN/Controllers/CandidateDashboardController.cs
ENABLEVN/Controllers/CandidateProfileController.cs
ENABLEVN/Controllers/CandidatesController.cs
ENABLEVN/Controllers/CatalogController.cs
ENABLEVN/Controllers/CompanyReviewsController.cs
ENABLEVN/Controllers/DiagnosticsController.cs
ENABLEVN/Controllers/EmployerDashboardController.cs
ENABLEVN/Controllers/EmployerJobsController.cs
ENABLEVN/Controllers/EmployerProfileController.cs
ENABLEVN/Controllers/HomeController.cs
ENABLEVN/Controllers/JobApplicationsController.cs
ENABLEVN/Controllers/JobsController.cs
ENABLEVN/Controllers/NotificationsController.cs
ENABLEVN/Controllers/ViolationReportsController.cs
ENABLEVN/Hubs/RecruiterChatHub.cs
ENABLEVN/Models/Ai/AiChatDtos.cs
ENABLEVN/Models/ErrorViewModel.cs
ENABLEVN/Options/ChatModerationOptions.cs
ENABLEVN/Options/GeminiOptions.cs
ENABLEVN/Program.cs
ENABLEVN/SeedData/InMemoryAdminSeeder.cs
ENABLEVN/SeedData/InMemoryCatalogSeeder.cs
ENABLEVN/Services/AiRecruitmentService.cs
ENABLEVN/Services/ChatModerationService.cs
ENABLEVN/Services/GeminiClient.cs
ENABLEVN/Services/SessionCurrentUserService.cs
ENABLEVN/Services/ToxicCommentMlClassifier.cs
ENABLEVN/ViewComponents/NotificationBellViewComponent.cs
ENABLEVN/ViewModels/Dashboard/AdminDashboardViewModel.cs
ENABLEVN/ViewModels/Dashboard/CandidateDashboardViewModel.cs
ENABLEVN/ViewModels/Dashboard/EmployerDashboardViewModel.cs
ENABLEVN/ViewModels/Home/HomeIndexViewModel.cs
ENABLEVN/ViewModels/Shared/BreadcrumbModel.cs
InfrastructureInMemory/DependencyInjection.cs
InfrastructureInMemory/Repositories/InMemoryAssistiveDeviceRepository.cs
InfrastructureInMemory/Repositories/InMemoryCandidateProfileRepository.cs
InfrastructureInMemory/Repositories/InMemoryCurrentUserService.cs
InfrastructureInMemory/Repositories/InMemoryDisabilityTypeRepository.cs
InfrastructureInMemory/Repositories/InMemoryDomainEventDispatcher.cs
InfrastructureInMemory/Repositories/InMemoryEmployerProfileRepository.cs
InfrastructureInMemory/Repositories/InMemoryJobApplicationRepository.cs
InfrastructureInMemory/Repositories/InMemoryJobCategoryRepository.cs
InfrastructureInMemory/Repositories/InMemoryJobRepository.cs
InfrastructureInMemory/Repositories/InMemoryUserRepository.cs
InfrastructureInMemory/Repositories/SimplePasswordHasher.cs
InfrastructureInMemory/Repositories/SimpleTokenService.cs
InfrastructureInMemory/Services/SmtpEmailService.cs
InfrastructureSqlite/DependencyInjection.cs
InfrastructureSqlite/Mappers/ApplicationStatusHistoryPersistenceMapper.cs
InfrastructureSqlite/Mappers/CandidateProfilePersistenceMapper.cs
InfrastructureSqlite/Mappers/CatalogPersistenceMapper.cs
InfrastructureSqlite/Mappers/EmployerProfilePersistenceMapper.cs
InfrastructureSqlite/Mappers/JobApplicationPersistenceMapper.cs
InfrastructureSqlite/Mappers/JobPostPersistenceMapper.cs
InfrastructureSqlite/Mappers/NotificationPersistenceMapper.cs
InfrastructureSqlite/Mappers/UserPersistenceMapper.cs
InfrastructureSqlite/Migrations/20260504170824_InitialSqliteCreate.cs
InfrastructureSqlite/Migrations/20260504170824_InitialSqliteCreate.Designer.cs
InfrastructureSqlite/Migrations/20260504181737_AddNotifications.cs
InfrastructureSqlite/Migrations/20260504181737_AddNotifications.Designer.cs
InfrastructureSqlite/Migrations/20260504220206_AddApplicationChatMessages.cs
InfrastructureSqlite/Migrations/20260504220206_AddApplicationChatMessages.Designer.cs
InfrastructureSqlite/Migrations/EnableVnDbContextModelSnapshot.cs
InfrastructureSqlite/Persistence/EnableVnDbContext.cs
InfrastructureSqlite/Persistence/EnableVnDbContextFactory.cs
InfrastructureSqlite/PersistenceModels/ApplicationChatMessageRecord.cs
InfrastructureSqlite/PersistenceModels/ApplicationStatusHistoryRecord.cs
InfrastructureSqlite/PersistenceModels/AssistiveDeviceRecord.cs
InfrastructureSqlite/PersistenceModels/CandidateProfileRecord.cs
InfrastructureSqlite/PersistenceModels/DisabilityTypeRecord.cs
InfrastructureSqlite/PersistenceModels/EmployerProfileRecord.cs
InfrastructureSqlite/PersistenceModels/JobApplicationRecord.cs
InfrastructureSqlite/PersistenceModels/JobCategoryRecord.cs
InfrastructureSqlite/PersistenceModels/JobPostRecord.cs
InfrastructureSqlite/PersistenceModels/NotificationRecord.cs
InfrastructureSqlite/PersistenceModels/UserRecord.cs
InfrastructureSqlite/Repositories/SqliteApplicationChatRepository.cs
InfrastructureSqlite/Repositories/SqliteAssistiveDeviceRepository.cs
InfrastructureSqlite/Repositories/SqliteCandidateProfileRepository.cs
InfrastructureSqlite/Repositories/SqliteDisabilityTypeRepository.cs
InfrastructureSqlite/Repositories/SqliteEmployerProfileRepository.cs
InfrastructureSqlite/Repositories/SqliteJobApplicationRepository.cs
InfrastructureSqlite/Repositories/SqliteJobCategoryRepository.cs
InfrastructureSqlite/Repositories/SqliteJobRepository.cs
InfrastructureSqlite/Repositories/SqliteNotificationRepository.cs
InfrastructureSqlite/Repositories/SqliteUserRepository.cs
InfrastructureSqlite/SeedData/SqliteAdminSeeder.cs
InfrastructureSqlite/SeedData/SqliteCatalogSeeder.cs
Ports/Inbound/IAuthUseCase.cs
Ports/Inbound/ICandidateProfileUseCase.cs
Ports/Inbound/ICatalogUseCase.cs
Ports/Inbound/ICompanyReviewUseCase.cs
Ports/Inbound/IEmployerCandidateSearchUseCase.cs
Ports/Inbound/IEmployerProfileUseCase.cs
Ports/Inbound/IJobApplicationUseCase.cs
Ports/Inbound/IJobUseCase.cs
Ports/Inbound/INotificationUseCase.cs
Ports/Inbound/IUserUseCase.cs
Ports/Inbound/IViolationReportUseCase.cs
Ports/Models/Applications/ChangeApplicationStatusCommand.cs
Ports/Models/Applications/JobApplicationResult.cs
Ports/Models/Applications/SubmitJobApplicationCommand.cs
Ports/Models/Auth/AuthResult.cs
Ports/Models/Auth/LoginCommand.cs
Ports/Models/Auth/RegisterCommand.cs
Ports/Models/Candidates/CandidateProfileResult.cs
Ports/Models/Candidates/CreateCandidateProfileCommand.cs
Ports/Models/Candidates/SearchPublicCandidatesQuery.cs
Ports/Models/Candidates/UpdateCandidateProfileCommand.cs
Ports/Models/Candidates/UpdateDisabilityInfoCommand.cs
Ports/Models/Catalogs/CatalogItemResult.cs
Ports/Models/Catalogs/CreateCatalogItemCommand.cs
Ports/Models/Catalogs/UpdateCatalogItemCommand.cs
Ports/Models/Chat/ApplicationChatMessageDto.cs
Ports/Models/Chat/ApplicationChatThreadDto.cs
Ports/Models/Employers/CreateEmployerProfileCommand.cs
Ports/Models/Employers/EmployerProfileResult.cs
Ports/Models/Employers/UpdateEmployerProfileCommand.cs
Ports/Models/Jobs/CreateJobCommand.cs
Ports/Models/Jobs/JobResult.cs
Ports/Models/Jobs/SearchJobQuery.cs
Ports/Models/Jobs/UpdateJobCommand.cs
Ports/Models/Notifications/NotificationResult.cs
Ports/Models/Reports/CreateViolationReportCommand.cs
Ports/Models/Reports/HandleViolationReportCommand.cs
Ports/Models/Reports/ViolationReportResult.cs
Ports/Models/Reviews/CompanyReviewResult.cs
Ports/Models/Reviews/CreateCompanyReviewCommand.cs
Ports/Outbound/Repositories/IApplicationChatRepository.cs
Ports/Outbound/Repositories/IAssistiveDeviceRepository.cs
Ports/Outbound/Repositories/ICandidateProfileRepository.cs
Ports/Outbound/Repositories/ICompanyReviewRepository.cs
Ports/Outbound/Repositories/IDisabilityTypeRepository.cs
Ports/Outbound/Repositories/IEmployerProfileRepository.cs
Ports/Outbound/Repositories/IJobApplicationRepository.cs
Ports/Outbound/Repositories/IJobCategoryRepository.cs
Ports/Outbound/Repositories/IJobRepository.cs
Ports/Outbound/Repositories/INotificationRepository.cs
Ports/Outbound/Repositories/InMemoryCompanyReviewRepository.cs
Ports/Outbound/Repositories/InMemoryViolationReportRepository.cs
Ports/Outbound/Repositories/IUserRepository.cs
Ports/Outbound/Repositories/IViolationReportRepository.cs
Ports/Outbound/Services/ICurrentUserService.cs
Ports/Outbound/Services/IDomainEventDispatcher.cs
Ports/Outbound/Services/IEmailService.cs
Ports/Outbound/Services/IFileStorageService.cs
Ports/Outbound/Services/IPasswordHasher.cs
Ports/Outbound/Services/ITokenService.cs
```

---

## 13. Phụ lục B — Giao diện và cấu hình web (`ENABLEVN/`, không gồm `wwwroot/lib/`)

**JSON / launch:**  
`appsettings.json`, `appsettings.Development.json`, `Properties/launchSettings.json`

**Razor:**  
`Views/_ViewImports.cshtml`, `Views/_ViewStart.cshtml`,  
`Views/AdminDashboard/Index.cshtml`, `Views/AiAssistant/Index.cshtml`, `Views/ApplicationChat/Thread.cshtml`,  
`Views/Auth/Login.cshtml`, `Views/Auth/Register.cshtml`,  
`Views/CandidateDashboard/Index.cshtml`,  
`Views/CandidateProfile/Create.cshtml`, `Views/CandidateProfile/Disability.cshtml`, `Views/CandidateProfile/Edit.cshtml`, `Views/CandidateProfile/Index.cshtml`,  
`Views/Candidates/Details.cshtml`, `Views/Candidates/Index.cshtml`,  
`Views/Catalog/CatalogEditForm.cshtml`, `Views/Catalog/CatalogForm.cshtml`, `Views/Catalog/CatalogList.cshtml`, `Views/Catalog/Index.cshtml`,  
`Views/EmployerDashboard/Index.cshtml`,  
`Views/EmployerJobs/Create.cshtml`, `Views/EmployerJobs/Details.cshtml`, `Views/EmployerJobs/Edit.cshtml`, `Views/EmployerJobs/Index.cshtml`,  
`Views/EmployerProfile/_EmployerProfileForm.cshtml`, `Views/EmployerProfile/Create.cshtml`, `Views/EmployerProfile/Details.cshtml`, `Views/EmployerProfile/Edit.cshtml`, `Views/EmployerProfile/Index.cshtml`,  
`Views/Home/Index.cshtml`, `Views/Home/Privacy.cshtml`,  
`Views/JobApplications/ByJob.cshtml`, `Views/JobApplications/MyApplications.cshtml`,  
`Views/Jobs/Details.cshtml`, `Views/Jobs/Index.cshtml`,  
`Views/Notifications/Index.cshtml`,  
`Views/Shared/_Breadcrumb.cshtml`, `Views/Shared/_Footer.cshtml`, `Views/Shared/_Header.cshtml`, `Views/Shared/_Layout.cshtml`, `Views/Shared/_Layout.cshtml.css`, `Views/Shared/_ValidationScriptsPartial.cshtml`, `Views/Shared/Components/NotificationBell/Default.cshtml`, `Views/Shared/Error.cshtml`,  
`Views/ViolationReports/Create.cshtml`, `Views/ViolationReports/Index.cshtml`

**Static tự phát triển:**  
`wwwroot/css/enablevn.css`, `wwwroot/css/site.css`,  
`wwwroot/js/ai-assistant-page.js`, `wwwroot/js/ai-recruitment.js`, `wwwroot/js/application-chat.js`, `wwwroot/js/enablevn-accessibility.js`, `wwwroot/js/site.js`

**Ghi chú:** `wwwroot/lib/**` gồm Bootstrap, jQuery, jquery-validation (đầy đủ trong repo theo template); không liệt kê từng file tại đây để tránh trùng lặp hàng trăm dòng — khi cần inventory tuyệt đối, chạy `Get-ChildItem -Recurse` trên `wwwroot/lib`.

---

## 14. Đối chiếu bảy file markdown lịch sử (Summary 1→7) với **code hiện tại**

**Cập nhật:** Các file summary **1→7** đã được sửa nội dung chính (trạng thái, cây thư mục, số liệu SQLite) để **khớp repo**; bảng dưới ghi lại **phạm vi lịch sử** (phần văn dài/kỹ thuật chi tiết vẫn có thể là bối cảnh thiết kế, không phải diff từng dòng).

| # | File | Đã chỉnh trong đợt đồng bộ | Vẫn đọc được (không thay bằng bản 8) |
|---|------|-----------------------------|--------------------------------------|
| **1** | `EnableVN_Domain_Summary_1.md` | Banner; cây §2 (thêm Notifications/Reviews/Reports, `ValueObject`, bỏ `JobPostingPolicy` file); §7.7 + bảng pattern → mô tả “rule trong JobPost”. | Các mục giải thích Entity/VO/Event chi tiết. |
| **2** | `EnableVN_Ports_2.md` | Banner; cây Jobs bỏ `JobPostingPolicy`; cây Ports Inbound/Outbound/Models; bảng use case bổ sung; method `IJobApplicationUseCase` (chat). | Các phần kiến trúc, ví dụ luồng dài. |
| **3** | `EnableVN_Application_Summary_3.md` | Trạng thái dự án + cây Mappers/UseCases; ghi nợ DI ViolationReport. | Nguyên tắc Application, UseCaseException, AuthorizationGuard. |
| **4** | `EnableVN_InfrastructureInMemory_Summary_4.md` | Trạng thái; cây project + `SmtpEmailService`; ghi Ports chứa InMemory review/report + nợ DI. | Mô tả Singleton `List<T>`, từng repository. |
| **5** | `EnableVN_Presentation_Summary_5.md` | Trạng thái; stack (SignalR, API); mở rộng danh sách controller; mục 22/23/24 tổng kết; bỏ checklist “chưa SQLite”. | Nguyên tắc MVC, Session, Bootstrap. |
| **6** | `SQLITE_IMPLEMENTATION_SUMMARY_6.md` | Số file model/mapper/repo; 11 DbSet; migration 3 bước; index; checklist; known issues (NU1903, DI). | Chi tiết cột bảng MVP ban đầu, seed catalog. |
| **7** | `EnableVN_Phase2_Session_Summary_7.md` | Thứ tự ưu tiên (thêm bản 8); §2 migration; **§15** trạng thái Giai đoạn 2 theo code. | Hướng dẫn notification, migration PowerShell, `Entity<Guid>`, bảo mật. |

**Tên folder web:** Summary cũ ghi `EnableVN.Presentation` — repo dùng **`ENABLEVN/`** + `Presentation.csproj` (mục 1, 11).

---

**Kết thúc bản 8.** Khi chỉnh code, ưu tiên cập nhật lại mục 10, **mục 14** (nếu sửa lại summary 1–7 hoặc đổi kiến trúc), và các phụ lục nếu thêm/xóa file `.cs` hoặc đổi DI.
