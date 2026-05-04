# EnableVN_Ports_2.md

Tài liệu này dùng để gửi lại cho ChatGPT ở phiên làm việc mới nhằm tiếp tục phát triển dự án EnableVN mà không bị mất ngữ cảnh.

---

# 1. Bối cảnh dự án

EnableVN là web tuyển dụng hướng tới khả năng tiếp cận cho người khuyết tật.

MVP tập trung vào luồng chính:

```txt
Nhà tuyển dụng đăng việc
Ứng viên tìm việc
Ứng viên nộp hồ sơ
Nhà tuyển dụng quản lý trạng thái hồ sơ
Ứng viên có quyền riêng tư với thông tin khuyết tật
```

Dự án đang được thiết kế theo:

```txt
Hexagonal Architecture
Domain Driven Design
SOLID
Có áp dụng Design Pattern phù hợp, không ép đủ 23 pattern
```

---

# 2. Solution Architecture

Các project nhỏ dự kiến:

```txt
EnableVN.Domain
EnableVN.Ports
EnableVN.Application
EnableVN.InfrastructureSqlite
EnableVN.InfrastructureInMemory
EnableVN.Presentation
```

Chiều phụ thuộc mong muốn:

```txt
Application               -> Domain + Ports
InfrastructureSqlite      -> Domain + Ports
InfrastructureInMemory    -> Domain + Ports
Presentation              -> Ports
Ports                     -> Domain
Domain                    -> không phụ thuộc project nào
```

Lưu ý quan trọng:

```txt
Domain không phụ thuộc Ports.
Domain không phụ thuộc Application.
Domain không phụ thuộc Infrastructure.
Domain không phụ thuộc Presentation.
```

---

# 3. Domain đã làm

Project:

```txt
EnableVN.Domain
```

Domain hiện tại gồm:

```txt
Common
Users
Employers
Candidates
Jobs
Applications
Catalogs
```

---

# 4. Domain/Common

Các file đã có:

```txt
EnableVN.Domain/Common/Entity.cs
EnableVN.Domain/Common/AggregateRoot.cs
EnableVN.Domain/Common/IDomainEvent.cs
EnableVN.Domain/Common/DomainException.cs
```

## Entity

Base class cho các đối tượng có định danh.

Áp dụng cho:

```txt
User
EmployerProfile
CandidateProfile
JobPost
JobApplication
DisabilityType
AssistiveDevice
JobCategory
```

## AggregateRoot

Base class cho các Aggregate Root.

Có quản lý danh sách Domain Event:

```txt
DomainEvents
AddDomainEvent()
ClearDomainEvents()
```

## IDomainEvent

Interface chung cho các event nghiệp vụ.

Có thuộc tính:

```txt
OccurredOn
```

## DomainException

Exception riêng cho lỗi nghiệp vụ.

---

# 5. Domain/Users

Các file đã có:

```txt
EnableVN.Domain/Users/User.cs
EnableVN.Domain/Users/Email.cs
EnableVN.Domain/Users/UserRole.cs
EnableVN.Domain/Users/UserStatus.cs
EnableVN.Domain/Users/Events/UserRegisteredEvent.cs
```

## User

Đại diện tài khoản đăng nhập.

Thuộc tính chính:

```txt
Id
Email
PasswordHash
Role
Status
```

Method chính:

```txt
Register()
Lock()
Activate()
Delete()
```

Event phát sinh:

```txt
UserRegisteredEvent
```

## Email

Value Object để kiểm tra email:

```txt
Không rỗng
Trim
Lowercase
Đúng định dạng email cơ bản
```

## UserRole

```txt
Admin
Employer
Candidate
```

## UserStatus

```txt
Active
Locked
Deleted
```

---

# 6. Domain/Employers

Các file đã có:

```txt
EnableVN.Domain/Employers/EmployerProfile.cs
EnableVN.Domain/Employers/CompanyName.cs
EnableVN.Domain/Employers/InclusiveWorkplaceInfo.cs
EnableVN.Domain/Employers/Events/EmployerProfileCreatedEvent.cs
```

## EmployerProfile

Đại diện hồ sơ doanh nghiệp.

Thuộc tính chính:

```txt
Id
UserId
CompanyName
Description
WebsiteUrl
WorkplaceInfo
```

Method chính:

```txt
Create()
UpdateCompanyInfo()
UpdateInclusiveWorkplaceInfo()
```

Event phát sinh:

```txt
EmployerProfileCreatedEvent
```

## InclusiveWorkplaceInfo

Thông tin môi trường làm việc bao trùm.

Thuộc tính:

```txt
HasWheelchairAccess
HasAccessibleRestroom
SupportsFlexibleWorkingTime
SupportsRemoteWork
ProvidesAssistiveDevices
```

---

# 7. Domain/Candidates

Các file đã có:

```txt
EnableVN.Domain/Candidates/CandidateProfile.cs
EnableVN.Domain/Candidates/FullName.cs
EnableVN.Domain/Candidates/DisabilityInfo.cs
EnableVN.Domain/Candidates/Events/CandidateProfileCreatedEvent.cs
```

## CandidateProfile

Đại diện hồ sơ ứng viên.

Thuộc tính chính:

```txt
Id
UserId
FullName
Bio
CvUrl
DisabilityInfo
IsPublicProfile
```

Method chính:

```txt
Create()
UpdateBasicInfo()
UpdateDisabilityInfo()
HideDisabilityInfo()
ShowDisabilityInfoToEmployer()
MakeProfilePublic()
MakeProfilePrivate()
```

Event phát sinh:

```txt
CandidateProfileCreatedEvent
```

## DisabilityInfo

Quản lý thông tin khuyết tật và quyền hiển thị.

Thuộc tính:

```txt
DisabilityTypeId
Description
IsVisibleToEmployer
```

Method:

```txt
Hidden()
Create()
Hide()
Show()
```

Lưu ý:

```txt
Thông tin khuyết tật mặc định nên ẩn.
Đây là rule quyền riêng tư quan trọng của dự án.
```

---

# 8. Domain/Jobs

Các file đã có:

```txt
EnableVN.Domain/Jobs/JobPost.cs
EnableVN.Domain/Jobs/JobTitle.cs
EnableVN.Domain/Jobs/SalaryRange.cs
EnableVN.Domain/Jobs/JobAccessibilityInfo.cs
EnableVN.Domain/Jobs/WorkMode.cs
EnableVN.Domain/Jobs/JobStatus.cs
EnableVN.Domain/Jobs/Policies/JobPostingPolicy.cs
EnableVN.Domain/Jobs/Events/JobPostedEvent.cs
EnableVN.Domain/Jobs/Events/JobClosedEvent.cs
```

## JobPost

Đại diện tin tuyển dụng.

Thuộc tính chính:

```txt
Id
EmployerId
Title
Description
Requirement
WorkMode
SalaryRange
AccessibilityInfo
Status
CreatedAt
PublishedAt
ClosedAt
```

Method chính:

```txt
CreateDraft()
UpdateContent()
Publish()
Close()
Delete()
CanReceiveApplication()
```

Event phát sinh:

```txt
JobPostedEvent
JobClosedEvent
```

## JobAccessibilityInfo

Thông tin hỗ trợ tiếp cận của job.

Thuộc tính:

```txt
SupportsWheelchairAccess
SupportsRemoteWork
SupportsFlexibleTime
ProvidesAssistiveDevices
AdditionalSupportDescription
```

## WorkMode

```txt
Onsite
Remote
Hybrid
```

## JobStatus

```txt
Draft
Published
Closed
Deleted
```

---

# 9. Domain/Applications

Lưu ý: `Applications` ở đây là nghiệp vụ ứng tuyển, không phải Project `Application`.

Các file đã có:

```txt
EnableVN.Domain/Applications/JobApplication.cs
EnableVN.Domain/Applications/ApplicationStatus.cs
EnableVN.Domain/Applications/ApplicationStatusHistory.cs
EnableVN.Domain/Applications/Policies/ApplicationStatusPolicy.cs
EnableVN.Domain/Applications/Events/JobApplicationSubmittedEvent.cs
EnableVN.Domain/Applications/Events/JobApplicationStatusChangedEvent.cs
```

## JobApplication

Đại diện hồ sơ ứng tuyển của Candidate vào một Job.

Thuộc tính chính:

```txt
Id
JobId
CandidateId
CoverLetter
CvUrl
Status
SubmittedAt
StatusHistories
```

Method chính:

```txt
Submit()
ChangeStatus()
Withdraw()
```

Event phát sinh:

```txt
JobApplicationSubmittedEvent
JobApplicationStatusChangedEvent
```

## ApplicationStatus

```txt
Pending
Reviewing
Interview
Rejected
Accepted
Withdrawn
```

## ApplicationStatusPolicy

Rule chuyển trạng thái:

```txt
Không đổi trạng thái nếu hồ sơ đã Withdrawn
Không đổi trạng thái nếu hồ sơ đã Rejected
Không đổi trạng thái nếu hồ sơ đã Accepted
Không đổi sang chính trạng thái hiện tại
```

---

# 10. Domain/Catalogs

Các file đã có hoặc cần có:

```txt
EnableVN.Domain/Catalogs/DisabilityType.cs
EnableVN.Domain/Catalogs/AssistiveDevice.cs
EnableVN.Domain/Catalogs/JobCategory.cs
EnableVN.Domain/Catalogs/CatalogStatus.cs
```

## DisabilityType

Danh mục loại khuyết tật do Admin quản lý.

Thuộc tính:

```txt
Id
Name
Description
Status
```

Method:

```txt
Create()
Update()
Activate()
Deactivate()
```

## AssistiveDevice

Danh mục thiết bị hỗ trợ.

Thuộc tính:

```txt
Id
Name
Description
Status
```

Method:

```txt
Create()
Update()
Activate()
Deactivate()
```

## JobCategory

Đây là class đã bị thiếu ở lần trước và đã được xác nhận là nên bổ sung.

Lý do có `JobCategory`:

```txt
Ports có IJobCategoryRepository gọi đến JobCategory.
JobCategory là danh mục ngành nghề / nhóm công việc.
Nó giúp lọc job theo ngành nghề ở MVP hoặc các phase sau.
```

Code cần có:

```csharp
namespace EnableVN.Domain.Catalogs;

using EnableVN.Domain.Common;

/// <summary>
/// Danh mục ngành nghề / nhóm công việc.
/// Ví dụ: IT, Marketing, Kế toán, Chăm sóc khách hàng.
/// </summary>
public sealed class JobCategory : AggregateRoot<Guid>
{
    public string Name { get; private set; }

    public string? Description { get; private set; }

    public CatalogStatus Status { get; private set; }

    private JobCategory(
        Guid id,
        string name,
        string? description
    ) : base(id)
    {
        Name = name;
        Description = description;
        Status = CatalogStatus.Active;
    }

    public static JobCategory Create(
        string name,
        string? description
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tên ngành nghề không được để trống.");

        name = name.Trim();

        if (name.Length < 2)
            throw new DomainException("Tên ngành nghề quá ngắn.");

        if (name.Length > 100)
            throw new DomainException("Tên ngành nghề không được vượt quá 100 ký tự.");

        return new JobCategory(
            Guid.NewGuid(),
            name,
            description?.Trim()
        );
    }

    public void Update(
        string name,
        string? description
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tên ngành nghề không được để trống.");

        name = name.Trim();

        if (name.Length < 2)
            throw new DomainException("Tên ngành nghề quá ngắn.");

        if (name.Length > 100)
            throw new DomainException("Tên ngành nghề không được vượt quá 100 ký tự.");

        Name = name;
        Description = description?.Trim();
    }

    public void Activate()
    {
        Status = CatalogStatus.Active;
    }

    public void Deactivate()
    {
        Status = CatalogStatus.Inactive;
    }
}
```

## CatalogStatus

```txt
Active
Inactive
```

---

# 11. Domain Events đã bổ sung

Các event đã thêm:

```txt
JobApplicationStatusChangedEvent
JobApplicationSubmittedEvent
CandidateProfileCreatedEvent
EmployerProfileCreatedEvent
JobClosedEvent
JobPostedEvent
UserRegisteredEvent
```

Hiện tại MVP chưa bắt buộc phải xử lý event ngay, nhưng vì các Aggregate đã gọi:

```csharp
AddDomainEvent(...)
```

nên cần tạo class event để code compile.

Sau này Domain Event dùng cho:

```txt
Gửi email
Tạo notification
Ghi audit log
Đồng bộ search index
Thống kê nghiệp vụ
```

---

# 12. Kỹ thuật đã áp dụng trong Domain

```txt
Entity Pattern
Value Object Pattern
Aggregate Root Pattern
Factory Method Pattern
Domain Event Pattern
Policy Pattern
State Management bằng Enum
Encapsulation
Fail Fast Validation
Privacy by Design
```

Các nguyên lý SOLID áp dụng:

```txt
Single Responsibility Principle
Open/Closed Principle
Liskov Substitution Principle
Interface Segregation Principle
Dependency Inversion Principle
```

---

# 13. Ports đã làm

Project:

```txt
EnableVN.Ports
```

Mục tiêu:

```txt
Ports chứa hợp đồng giao tiếp của hệ thống.
Inbound Ports cho Presentation gọi vào Application.
Outbound Ports cho Application gọi ra Infrastructure.
```

Cấu trúc đề xuất:

```txt
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
    ├── Employers
    ├── Candidates
    ├── Jobs
    ├── Applications
    └── Catalogs
```

---

# 14. Ports dependency

`EnableVN.Ports` được phép reference:

```txt
EnableVN.Domain
```

Vì Ports cần dùng các kiểu Domain như:

```txt
User
UserRole
EmployerProfile
CandidateProfile
JobPost
JobApplication
DisabilityType
AssistiveDevice
JobCategory
WorkMode
JobStatus
ApplicationStatus
CatalogStatus
```

`EnableVN.Ports` không được reference:

```txt
EnableVN.Application
EnableVN.InfrastructureSqlite
EnableVN.InfrastructureInMemory
EnableVN.Presentation
```

---

# 15. Outbound Repositories đã thiết kế

## IUserRepository

File:

```txt
EnableVN.Ports/Outbound/Repositories/IUserRepository.cs
```

Method:

```txt
GetByIdAsync
GetByEmailAsync
ExistsByEmailAsync
AddAsync
UpdateAsync
```

Dùng cho:

```txt
Đăng ký
Đăng nhập
Khóa tài khoản
Kích hoạt tài khoản
Xóa mềm tài khoản
```

---

## IEmployerProfileRepository

File:

```txt
EnableVN.Ports/Outbound/Repositories/IEmployerProfileRepository.cs
```

Method:

```txt
GetByIdAsync
GetByUserIdAsync
ExistsByUserIdAsync
AddAsync
UpdateAsync
```

---

## ICandidateProfileRepository

File:

```txt
EnableVN.Ports/Outbound/Repositories/ICandidateProfileRepository.cs
```

Method:

```txt
GetByIdAsync
GetByUserIdAsync
ExistsByUserIdAsync
GetPublicProfilesAsync
AddAsync
UpdateAsync
```

---

## IJobRepository

File:

```txt
EnableVN.Ports/Outbound/Repositories/IJobRepository.cs
```

Method:

```txt
GetByIdAsync
GetByEmployerIdAsync
SearchPublishedJobsAsync
AddAsync
UpdateAsync
```

SearchPublishedJobsAsync có filter:

```txt
keyword
workMode
supportsWheelchairAccess
supportsRemoteWork
supportsFlexibleTime
providesAssistiveDevices
```

---

## IJobApplicationRepository

File:

```txt
EnableVN.Ports/Outbound/Repositories/IJobApplicationRepository.cs
```

Method:

```txt
GetByIdAsync
GetByJobIdAsync
GetByCandidateIdAsync
ExistsByJobIdAndCandidateIdAsync
AddAsync
UpdateAsync
```

Rule `Candidate đã nộp trùng job chưa` cần repository nên xử lý ở Application, không xử lý thuần trong Domain Entity.

---

## IDisabilityTypeRepository

File:

```txt
EnableVN.Ports/Outbound/Repositories/IDisabilityTypeRepository.cs
```

Method:

```txt
GetByIdAsync
GetAllAsync
GetActiveAsync
AddAsync
UpdateAsync
```

---

## IAssistiveDeviceRepository

File:

```txt
EnableVN.Ports/Outbound/Repositories/IAssistiveDeviceRepository.cs
```

Method:

```txt
GetByIdAsync
GetAllAsync
GetActiveAsync
AddAsync
UpdateAsync
```

---

## IJobCategoryRepository

File:

```txt
EnableVN.Ports/Outbound/Repositories/IJobCategoryRepository.cs
```

Method:

```txt
GetByIdAsync
GetAllAsync
GetActiveAsync
AddAsync
UpdateAsync
```

Lưu ý:

```txt
Interface này cần Domain class JobCategory.
JobCategory đã được xác nhận là bị quên gửi code ở lần trước và nên bổ sung vào Domain/Catalogs.
```

---

# 16. Outbound Services đã thiết kế

## ICurrentUserService

File:

```txt
EnableVN.Ports/Outbound/Services/ICurrentUserService.cs
```

Thuộc tính:

```txt
UserId
Role
IsAuthenticated
```

Dùng để Application biết user hiện tại là ai.

Lưu ý:

```txt
UserId không nên nhận từ client trong command.
Nên lấy từ ICurrentUserService để tránh giả mạo.
```

---

## IPasswordHasher

File:

```txt
EnableVN.Ports/Outbound/Services/IPasswordHasher.cs
```

Method:

```txt
Hash
Verify
```

---

## IEmailService

File:

```txt
EnableVN.Ports/Outbound/Services/IEmailService.cs
```

Method:

```txt
SendAsync
```

Dùng cho phase 2:

```txt
Gửi email khi trạng thái hồ sơ thay đổi
Gửi email chào mừng
Gửi thông báo job/application
```

---

## IFileStorageService

File:

```txt
EnableVN.Ports/Outbound/Services/IFileStorageService.cs
```

Method:

```txt
SaveAsync
DeleteAsync
```

Dùng cho:

```txt
CV ứng viên
Logo công ty
File đính kèm
```

---

## IDomainEventDispatcher

File:

```txt
EnableVN.Ports/Outbound/Services/IDomainEventDispatcher.cs
```

Method:

```txt
DispatchAsync(IDomainEvent)
DispatchAsync(IEnumerable<IDomainEvent>)
```

Dùng để Application/Infrastructure xử lý Domain Events.

---

# 17. Ports Models đã thiết kế

Các model này dùng cho Inbound UseCase.

Lý do đặt trong `Ports.Models`:

```txt
Inbound interface cần command/query/result.
Nếu đặt trong Application.DTO thì Ports sẽ phải phụ thuộc Application, sai chiều dependency.
Vì vậy contract model nên nằm ở Ports.Models.
```

---

# 18. Models/Auth

File:

```txt
EnableVN.Ports/Models/Auth/RegisterCommand.cs
EnableVN.Ports/Models/Auth/LoginCommand.cs
EnableVN.Ports/Models/Auth/AuthResult.cs
```

## RegisterCommand

Thuộc tính:

```txt
Email
Password
Role
```

Lưu ý:

```txt
Password là plain text từ người dùng.
Application dùng IPasswordHasher để hash.
Domain chỉ nhận PasswordHash.
```

## LoginCommand

Thuộc tính:

```txt
Email
Password
```

## AuthResult

Thuộc tính:

```txt
UserId
Email
Role
Token
```

---

# 19. Models/Employers

File:

```txt
CreateEmployerProfileCommand.cs
UpdateEmployerProfileCommand.cs
EmployerProfileResult.cs
```

## CreateEmployerProfileCommand

Thuộc tính:

```txt
CompanyName
Description
WebsiteUrl
HasWheelchairAccess
HasAccessibleRestroom
SupportsFlexibleWorkingTime
SupportsRemoteWork
ProvidesAssistiveDevices
```

Lưu ý:

```txt
Không có UserId.
UserId lấy từ ICurrentUserService.
```

## UpdateEmployerProfileCommand

Tương tự Create.

## EmployerProfileResult

Thuộc tính:

```txt
Id
UserId
CompanyName
Description
WebsiteUrl
HasWheelchairAccess
HasAccessibleRestroom
SupportsFlexibleWorkingTime
SupportsRemoteWork
ProvidesAssistiveDevices
```

---

# 20. Models/Candidates

File:

```txt
CreateCandidateProfileCommand.cs
UpdateCandidateProfileCommand.cs
UpdateDisabilityInfoCommand.cs
CandidateProfileResult.cs
```

## CreateCandidateProfileCommand

Thuộc tính:

```txt
FullName
Bio
CvUrl
```

Không có UserId vì lấy từ ICurrentUserService.

## UpdateCandidateProfileCommand

Thuộc tính:

```txt
FullName
Bio
CvUrl
```

## UpdateDisabilityInfoCommand

Thuộc tính:

```txt
DisabilityTypeId
Description
IsVisibleToEmployer
```

Đây là command quan trọng cho quyền riêng tư.

## CandidateProfileResult

Thuộc tính:

```txt
Id
UserId
FullName
Bio
CvUrl
DisabilityTypeId
DisabilityDescription
IsDisabilityInfoVisibleToEmployer
IsPublicProfile
```

Lưu ý:

```txt
Nếu viewer là Employer và ứng viên không cho xem thông tin khuyết tật,
Application không nên map DisabilityDescription ra response.
```

---

# 21. Models/Jobs

File:

```txt
CreateJobCommand.cs
UpdateJobCommand.cs
SearchJobQuery.cs
JobResult.cs
```

## CreateJobCommand

Thuộc tính:

```txt
Title
Description
Requirement
WorkMode
MinSalary
MaxSalary
SupportsWheelchairAccess
SupportsRemoteWork
SupportsFlexibleTime
ProvidesAssistiveDevices
AdditionalSupportDescription
```

Không có EmployerId vì lấy từ current user rồi tìm EmployerProfile.

## UpdateJobCommand

Thuộc tính:

```txt
JobId
Title
Description
Requirement
WorkMode
MinSalary
MaxSalary
SupportsWheelchairAccess
SupportsRemoteWork
SupportsFlexibleTime
ProvidesAssistiveDevices
AdditionalSupportDescription
```

## SearchJobQuery

Thuộc tính:

```txt
Keyword
WorkMode
SupportsWheelchairAccess
SupportsRemoteWork
SupportsFlexibleTime
ProvidesAssistiveDevices
```

Các filter nullable:

```txt
null = không lọc
true = chỉ lấy job có hỗ trợ tiêu chí đó
```

## JobResult

Thuộc tính:

```txt
Id
EmployerId
Title
Description
Requirement
WorkMode
MinSalary
MaxSalary
SupportsWheelchairAccess
SupportsRemoteWork
SupportsFlexibleTime
ProvidesAssistiveDevices
AdditionalSupportDescription
Status
CreatedAt
PublishedAt
ClosedAt
```

---

# 22. Models/Applications

File:

```txt
SubmitJobApplicationCommand.cs
ChangeApplicationStatusCommand.cs
JobApplicationResult.cs
```

## SubmitJobApplicationCommand

Thuộc tính:

```txt
JobId
CoverLetter
CvUrl
```

Không có CandidateId vì lấy từ current user rồi tìm CandidateProfile.

## ChangeApplicationStatusCommand

Thuộc tính:

```txt
ApplicationId
NewStatus
Note
```

## JobApplicationResult

Thuộc tính:

```txt
Id
JobId
CandidateId
CoverLetter
CvUrl
Status
SubmittedAt
```

---

# 23. Models/Catalogs

File:

```txt
CreateCatalogItemCommand.cs
UpdateCatalogItemCommand.cs
CatalogItemResult.cs
```

## CreateCatalogItemCommand

Thuộc tính:

```txt
Name
Description
```

Dùng chung cho:

```txt
DisabilityType
AssistiveDevice
JobCategory
```

## UpdateCatalogItemCommand

Thuộc tính:

```txt
Id
Name
Description
```

## CatalogItemResult

Thuộc tính:

```txt
Id
Name
Description
Status
```

---

# 24. Inbound UseCases đã thiết kế

## IAuthUseCase

File:

```txt
EnableVN.Ports/Inbound/IAuthUseCase.cs
```

Method:

```txt
RegisterAsync
LoginAsync
```

---

## IUserUseCase

File:

```txt
EnableVN.Ports/Inbound/IUserUseCase.cs
```

Method:

```txt
LockUserAsync
ActivateUserAsync
DeleteUserAsync
GetUserRoleAsync
```

---

## IEmployerProfileUseCase

File:

```txt
EnableVN.Ports/Inbound/IEmployerProfileUseCase.cs
```

Method:

```txt
CreateAsync
UpdateMyProfileAsync
GetMyProfileAsync
GetByIdAsync
```

---

## ICandidateProfileUseCase

File:

```txt
EnableVN.Ports/Inbound/ICandidateProfileUseCase.cs
```

Method:

```txt
CreateAsync
UpdateMyProfileAsync
UpdateMyDisabilityInfoAsync
HideMyDisabilityInfoAsync
ShowMyDisabilityInfoAsync
MakeMyProfilePublicAsync
MakeMyProfilePrivateAsync
GetMyProfileAsync
GetPublicProfilesAsync
```

---

## IJobUseCase

File:

```txt
EnableVN.Ports/Inbound/IJobUseCase.cs
```

Method:

```txt
CreateDraftAsync
UpdateAsync
PublishAsync
CloseAsync
DeleteAsync
GetByIdAsync
GetMyJobsAsync
SearchPublishedJobsAsync
```

---

## IJobApplicationUseCase

File:

```txt
EnableVN.Ports/Inbound/IJobApplicationUseCase.cs
```

Method:

```txt
SubmitAsync
ChangeStatusAsync
WithdrawAsync
GetByJobIdAsync
GetMyApplicationsAsync
GetByIdAsync
```

Rule cần xử lý trong Application khi Submit:

```txt
User hiện tại phải là Candidate
Candidate phải có CandidateProfile
Job phải tồn tại
Job phải Published
Candidate chưa từng nộp vào job này
```

Rule cần xử lý trong Application khi ChangeStatus:

```txt
User hiện tại phải là Employer
Application phải tồn tại
Job của application phải thuộc Employer hiện tại
Trạng thái mới hợp lệ theo Domain Policy
```

---

## ICatalogUseCase

File:

```txt
EnableVN.Ports/Inbound/ICatalogUseCase.cs
```

Method cho DisabilityType:

```txt
CreateDisabilityTypeAsync
UpdateDisabilityTypeAsync
DeactivateDisabilityTypeAsync
ActivateDisabilityTypeAsync
GetActiveDisabilityTypesAsync
```

Method cho AssistiveDevice:

```txt
CreateAssistiveDeviceAsync
UpdateAssistiveDeviceAsync
DeactivateAssistiveDeviceAsync
ActivateAssistiveDeviceAsync
GetActiveAssistiveDevicesAsync
```

Method cho JobCategory:

```txt
CreateJobCategoryAsync
UpdateJobCategoryAsync
DeactivateJobCategoryAsync
ActivateJobCategoryAsync
GetActiveJobCategoriesAsync
```

---

# 25. Kỹ thuật áp dụng trong Ports

```txt
Hexagonal Architecture
Dependency Inversion
Interface Segregation
Repository Pattern
Service Port Pattern
Command Pattern nhẹ
Query Object Pattern
DTO/Result Pattern
```

## Inbound Port

Presentation/API gọi vào Inbound Port.

Application implement Inbound Port.

Ví dụ:

```txt
Presentation gọi IJobUseCase.CreateDraftAsync()
Application implement JobUseCase : IJobUseCase
```

## Outbound Port

Application gọi ra Outbound Port.

Infrastructure implement Outbound Port.

Ví dụ:

```txt
Application gọi IJobRepository.AddAsync()
InfrastructureSqlite implement SqliteJobRepository : IJobRepository
InfrastructureInMemory implement InMemoryJobRepository : IJobRepository
```

---

# 26. Lưu ý quan trọng khi tiếp tục

## Không để Domain phụ thuộc Ports

Domain đã là lõi nghiệp vụ thuần.

Không được thêm vào Domain:

```txt
Repository interface
DTO
Command
Query
Service interface
EF Core attribute
DbContext
```

## Không nhận UserId từ client

Các command như:

```txt
CreateEmployerProfileCommand
CreateCandidateProfileCommand
CreateJobCommand
SubmitJobApplicationCommand
```

không nên có UserId, EmployerId, CandidateId do client gửi.

Application phải lấy từ:

```txt
ICurrentUserService
```

để tránh giả mạo quyền.

## Không trả Domain Entity trực tiếp ra Presentation

Nên map sang Result model:

```txt
JobResult
CandidateProfileResult
EmployerProfileResult
JobApplicationResult
CatalogItemResult
AuthResult
```

## Rule nào đặt ở Domain?

Đặt ở Domain nếu rule chỉ cần dữ liệu trong chính Aggregate.

Ví dụ:

```txt
SalaryRange min không lớn hơn max
Email phải đúng định dạng
Không đổi status nếu application đã Accepted
Job Published mới nhận hồ sơ
```

## Rule nào đặt ở Application?

Đặt ở Application nếu rule cần repository hoặc current user.

Ví dụ:

```txt
Email đã tồn tại chưa
Candidate đã nộp job này chưa
Job có thuộc Employer hiện tại không
User hiện tại có đúng role không
CandidateProfile có tồn tại không
EmployerProfile có tồn tại không
```

---

# 27. Vấn đề đã phát hiện và cách xử lý

## Vấn đề: thiếu JobCategory

Trong Ports có:

```txt
IJobCategoryRepository
ICatalogUseCase có các method JobCategory
```

Nhưng Domain ban đầu chưa có code `JobCategory`.

Kết luận:

```txt
Đây là do quên gửi code chi tiết.
JobCategory không phải class thừa.
Nên bổ sung vào Domain/Catalogs.
```

Lý do nên giữ:

```txt
JobCategory giúp lọc job theo ngành nghề.
Code đơn giản.
Phù hợp Catalog Domain.
Sẽ cần cho tìm kiếm việc làm.
```

Nếu muốn MVP cực tối giản thì có thể xóa `IJobCategoryRepository` và các method liên quan trong `ICatalogUseCase`, nhưng khuyến nghị là giữ.

---

# 28. Bước tiếp theo nên làm

Hiện tại đã xong:

```txt
Domain
Ports
```

Bước tiếp theo nên làm:

```txt
EnableVN.Application
```

Trong Application sẽ implement các Inbound Port:

```txt
AuthUseCase : IAuthUseCase
UserUseCase : IUserUseCase
EmployerProfileUseCase : IEmployerProfileUseCase
CandidateProfileUseCase : ICandidateProfileUseCase
JobUseCase : IJobUseCase
JobApplicationUseCase : IJobApplicationUseCase
CatalogUseCase : ICatalogUseCase
```

Application sẽ sử dụng các Outbound Port:

```txt
IUserRepository
IEmployerProfileRepository
ICandidateProfileRepository
IJobRepository
IJobApplicationRepository
IDisabilityTypeRepository
IAssistiveDeviceRepository
IJobCategoryRepository
ICurrentUserService
IPasswordHasher
IDomainEventDispatcher
IEmailService
IFileStorageService
```

---

# 29. Thứ tự nên code Application

Khuyến nghị làm theo thứ tự:

```txt
1. Application/Common
   - ApplicationException hoặc UseCaseException
   - Authorization helpers nếu cần
   - Mapper helpers nếu không dùng AutoMapper

2. AuthUseCase
   - RegisterAsync
   - LoginAsync

3. EmployerProfileUseCase
   - CreateAsync
   - UpdateMyProfileAsync
   - GetMyProfileAsync
   - GetByIdAsync

4. CandidateProfileUseCase
   - CreateAsync
   - UpdateMyProfileAsync
   - UpdateMyDisabilityInfoAsync
   - Hide/Show disability info
   - Public/Private profile
   - GetMyProfileAsync
   - GetPublicProfilesAsync

5. JobUseCase
   - CreateDraftAsync
   - UpdateAsync
   - PublishAsync
   - CloseAsync
   - DeleteAsync
   - GetByIdAsync
   - GetMyJobsAsync
   - SearchPublishedJobsAsync

6. JobApplicationUseCase
   - SubmitAsync
   - ChangeStatusAsync
   - WithdrawAsync
   - GetByJobIdAsync
   - GetMyApplicationsAsync
   - GetByIdAsync

7. CatalogUseCase
   - CRUD mềm cho DisabilityType
   - CRUD mềm cho AssistiveDevice
   - CRUD mềm cho JobCategory
```

---

# 30. Câu lệnh nên gửi trong phiên mới

Có thể gửi nguyên file này cho ChatGPT kèm yêu cầu:

```txt
Dựa trên file EnableVN_Ports_2.md này, hãy tiếp tục làm Project EnableVN.Application.
Yêu cầu:
- Code C#
- Có comment giải thích trong code
- Tuân thủ Hexagonal Architecture
- Tuân thủ DDD
- Tuân thủ SOLID
- Application implement các Inbound Port từ EnableVN.Ports
- Application chỉ gọi Outbound Port, không phụ thuộc Infrastructure
- Không đưa business rule thuần Domain vào Application nếu Domain đã xử lý
- Ưu tiên làm từng use case rõ ràng
```

---

# 31. Tóm tắt trạng thái hiện tại

```txt
Đã phân tích phase MVP.
Đã thiết kế Domain.
Đã bổ sung Domain Events.
Đã phát hiện và bổ sung thiếu JobCategory.
Đã thiết kế Ports gồm:
- Inbound UseCase interfaces
- Outbound Repository interfaces
- Outbound Service interfaces
- Ports Models: Command, Query, Result
```

Trạng thái:

```txt
Domain: xong nền tảng
Ports: xong thiết kế
Application: chưa làm
InfrastructureInMemory: chưa làm
InfrastructureSqlite: chưa làm
Presentation: chưa làm
```

Việc tiếp theo:

```txt
Làm EnableVN.Application
```
