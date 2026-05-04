# EnableVN_Application_Summary.md

Tài liệu này dùng để gửi lại cho ChatGPT ở phiên làm việc mới nhằm tiếp tục phát triển dự án EnableVN mà không bị mất ngữ cảnh.

---

# 1. Trạng thái hiện tại của dự án

Dự án EnableVN đang được xây dựng theo:

```txt
Hexagonal Architecture
Domain Driven Design
SOLID
Design Pattern nếu phù hợp
```

Các project chính:

```txt
EnableVN.Domain
EnableVN.Ports
EnableVN.Application
EnableVN.InfrastructureInMemory
EnableVN.InfrastructureSqlite
EnableVN.Presentation
```

Hiện tại đã làm:

```txt
EnableVN.Domain       ✅ Đã thiết kế xong nền tảng Domain
EnableVN.Ports        ✅ Đã thiết kế xong hợp đồng Inbound/Outbound/Models
EnableVN.Application  ✅ Đã viết code UseCase nền tảng
```

Chưa làm:

```txt
EnableVN.InfrastructureInMemory   ❌
EnableVN.InfrastructureSqlite     ❌
EnableVN.Presentation             ❌
```

---

# 2. Vai trò của EnableVN.Application

`EnableVN.Application` là tầng điều phối use case.

Application chịu trách nhiệm:

```txt
Nhận command/query từ Inbound Port
Kiểm tra quyền user hiện tại
Gọi repository/service qua Outbound Port
Gọi Domain Entity/Aggregate để thực thi nghiệp vụ
Map Domain Entity sang Result model
Dispatch Domain Event sau khi lưu dữ liệu
```

Application không được phụ thuộc vào:

```txt
SQLite
Entity Framework
DbContext
Controller
HttpContext trực tiếp
JWT implementation
File system implementation
Email provider implementation
```

Application được phép phụ thuộc vào:

```txt
EnableVN.Domain
EnableVN.Ports
```

---

# 3. Chiều phụ thuộc đúng

```txt
Presentation
    ↓ gọi
Ports.Inbound
    ↑ implement bởi
Application
    ↓ gọi
Ports.Outbound
    ↑ implement bởi
InfrastructureSqlite / InfrastructureInMemory
```

Application implement các interface trong:

```txt
EnableVN.Ports.Inbound
```

Application gọi các interface trong:

```txt
EnableVN.Ports.Outbound.Repositories
EnableVN.Ports.Outbound.Services
```

---

# 4. Cấu trúc EnableVN.Application đã thiết kế

```txt
EnableVN.Application
│
├── Common
│   ├── UseCaseException.cs
│   ├── AuthorizationGuard.cs
│   └── DomainEventHelper.cs
│
├── Mappers
│   ├── EmployerProfileMapper.cs
│   ├── CandidateProfileMapper.cs
│   ├── JobMapper.cs
│   ├── JobApplicationMapper.cs
│   └── CatalogMapper.cs
│
└── UseCases
    ├── AuthUseCase.cs
    ├── UserUseCase.cs
    ├── EmployerProfileUseCase.cs
    ├── CandidateProfileUseCase.cs
    ├── JobUseCase.cs
    ├── JobApplicationUseCase.cs
    └── CatalogUseCase.cs
```

---

# 5. Bổ sung nhỏ đã cần thêm vào Ports

Trong quá trình làm `AuthUseCase`, phát hiện `AuthResult` có `Token`, nên Application cần một abstraction để tạo token.

Đã đề xuất bổ sung vào Ports:

```txt
EnableVN.Ports/Outbound/Services/ITokenService.cs
```

Code:

```csharp
namespace EnableVN.Ports.Outbound.Services;

using EnableVN.Domain.Users;

/// <summary>
/// Outbound Port dùng để tạo access token sau khi đăng nhập/đăng ký.
/// Application không biết token là JWT, session token hay loại khác.
/// Infrastructure sẽ implement chi tiết.
/// </summary>
public interface ITokenService
{
    string GenerateToken(Guid userId, string email, UserRole role);
}
```

Lý do:

```txt
Tạo token là chi tiết kỹ thuật.
Không nên đặt trong Domain.
Không nên hard-code trong Application.
Không nên để Application phụ thuộc JWT library.
```

---

# 6. Lỗi đã phát hiện: ApplicationException bị ambiguous

Ban đầu Application có class:

```txt
EnableVN.Application.Common.ApplicationException
```

Nhưng C# cũng có sẵn:

```txt
System.ApplicationException
```

Nên khi viết:

```csharp
throw new ApplicationException("...");
```

compiler báo lỗi ambiguous:

```txt
'ApplicationException' is an ambiguous reference between
'EnableVN.Application.Common.ApplicationException'
and
'System.ApplicationException'
```

## Cách sửa đã thống nhất

Đổi tên class tự định nghĩa từ:

```txt
ApplicationException
```

thành:

```txt
UseCaseException
```

File mới:

```txt
EnableVN.Application/Common/UseCaseException.cs
```

Code:

```csharp
namespace EnableVN.Application.Common;

/// <summary>
/// Exception riêng cho tầng Application.
/// Dùng cho lỗi điều phối use case, phân quyền,
/// dữ liệu không tồn tại, hoặc thao tác không hợp lệ ở cấp ứng dụng.
/// </summary>
public sealed class UseCaseException : Exception
{
    public UseCaseException(string message) : base(message)
    {
    }
}
```

Sau đó replace toàn bộ:

```csharp
throw new ApplicationException(...)
```

thành:

```csharp
throw new UseCaseException(...)
```

---

# 7. Common trong Application

## 7.1. UseCaseException

File:

```txt
EnableVN.Application/Common/UseCaseException.cs
```

Vai trò:

```txt
Biểu diễn lỗi ở tầng Application UseCase
```

Dùng cho:

```txt
User chưa đăng nhập
User không đủ quyền
Không tìm thấy dữ liệu từ repository
Email đã tồn tại
Candidate đã nộp job này rồi
Job không thuộc Employer hiện tại
Candidate chưa có profile
Employer chưa có profile
```

Phân biệt với DomainException:

```txt
DomainException  = lỗi rule nghiệp vụ thuần trong Domain
UseCaseException = lỗi điều phối use case, quyền, dữ liệu repository
```

---

## 7.2. AuthorizationGuard

File:

```txt
EnableVN.Application/Common/AuthorizationGuard.cs
```

Vai trò:

```txt
Helper kiểm tra đăng nhập và phân quyền
```

Các method:

```txt
RequireAuthenticatedUser(ICurrentUserService currentUser)
RequireRole(ICurrentUserService currentUser, UserRole requiredRole)
RequireAdmin(ICurrentUserService currentUser)
RequireEmployer(ICurrentUserService currentUser)
RequireCandidate(ICurrentUserService currentUser)
```

Lý do đặt ở Application:

```txt
Current user là khái niệm của use case/runtime.
Domain không được biết HttpContext/JWT/session.
```

Ví dụ dùng:

```csharp
var userId = AuthorizationGuard.RequireEmployer(_currentUser);
```

Ý nghĩa:

```txt
Đảm bảo user hiện tại đã đăng nhập
Đảm bảo user có role Employer
Trả về UserId để Application tiếp tục xử lý
```

---

## 7.3. DomainEventHelper

File:

```txt
EnableVN.Application/Common/DomainEventHelper.cs
```

Vai trò:

```txt
Dispatch Domain Event sau khi repository lưu dữ liệu
```

Method:

```txt
DispatchAndClearEventsAsync<TId>(
    AggregateRoot<TId> aggregate,
    IDomainEventDispatcher dispatcher,
    CancellationToken cancellationToken = default
)
```

Luồng xử lý:

```txt
Aggregate tạo Domain Event
Application lưu Aggregate qua Repository
Application dispatch Event qua IDomainEventDispatcher
Application clear event khỏi Aggregate
```

Lý do:

```txt
Domain chỉ tạo event.
Application quyết định thời điểm dispatch event.
Infrastructure quyết định event được xử lý như thế nào.
```

---

# 8. Mappers đã làm

Application không trả Domain Entity trực tiếp ra Presentation.

Thay vào đó map sang Result model trong Ports.Models.

---

## 8.1. EmployerProfileMapper

File:

```txt
EnableVN.Application/Mappers/EmployerProfileMapper.cs
```

Map:

```txt
EmployerProfile -> EmployerProfileResult
```

Các field map:

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

## 8.2. CandidateProfileMapper

File:

```txt
EnableVN.Application/Mappers/CandidateProfileMapper.cs
```

Map:

```txt
CandidateProfile -> CandidateProfileResult
```

Có tham số quan trọng:

```txt
canViewDisabilityInfo
```

Ý nghĩa:

```txt
Nếu canViewDisabilityInfo = true:
    map DisabilityTypeId và DisabilityDescription

Nếu canViewDisabilityInfo = false:
    DisabilityTypeId = null
    DisabilityDescription = null
```

Mục tiêu:

```txt
Bảo vệ quyền riêng tư thông tin khuyết tật của Candidate.
```

---

## 8.3. JobMapper

File:

```txt
EnableVN.Application/Mappers/JobMapper.cs
```

Map:

```txt
JobPost -> JobResult
```

Các field map:

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

## 8.4. JobApplicationMapper

File:

```txt
EnableVN.Application/Mappers/JobApplicationMapper.cs
```

Map:

```txt
JobApplication -> JobApplicationResult
```

Các field map:

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

## 8.5. CatalogMapper

File:

```txt
EnableVN.Application/Mappers/CatalogMapper.cs
```

Map các catalog entity sang `CatalogItemResult`:

```txt
DisabilityType -> CatalogItemResult
AssistiveDevice -> CatalogItemResult
JobCategory -> CatalogItemResult
```

Các field map:

```txt
Id
Name
Description
Status
```

---

# 9. UseCases đã làm

---

# 9.1. AuthUseCase

File:

```txt
EnableVN.Application/UseCases/AuthUseCase.cs
```

Implement:

```txt
IAuthUseCase
```

Dependencies:

```txt
IUserRepository
IPasswordHasher
ITokenService
IDomainEventDispatcher
```

## RegisterAsync

Input:

```txt
RegisterCommand
```

Luồng xử lý:

```txt
1. Không cho đăng ký Admin từ luồng public
2. Kiểm tra email đã tồn tại chưa bằng IUserRepository.ExistsByEmailAsync
3. Hash password bằng IPasswordHasher.Hash
4. Tạo User bằng User.Register
5. Lưu User bằng IUserRepository.AddAsync
6. Dispatch UserRegisteredEvent
7. Tạo token bằng ITokenService.GenerateToken
8. Trả AuthResult
```

Rule ở Application:

```txt
Email đã tồn tại chưa
Không cho public register Admin
```

Rule ở Domain:

```txt
Email hợp lệ
PasswordHash không được rỗng
User được tạo với trạng thái Active
UserRegisteredEvent được add vào Aggregate
```

## LoginAsync

Input:

```txt
LoginCommand
```

Luồng xử lý:

```txt
1. Tìm user theo email
2. Nếu không có user -> báo sai email/mật khẩu
3. Nếu user không Active -> báo tài khoản không hoạt động
4. Verify password bằng IPasswordHasher.Verify
5. Nếu sai password -> báo sai email/mật khẩu
6. Tạo token bằng ITokenService.GenerateToken
7. Trả AuthResult
```

---

# 9.2. UserUseCase

File:

```txt
EnableVN.Application/UseCases/UserUseCase.cs
```

Implement:

```txt
IUserUseCase
```

Dependencies:

```txt
IUserRepository
ICurrentUserService
```

Các method:

```txt
LockUserAsync
ActivateUserAsync
DeleteUserAsync
GetUserRoleAsync
```

## LockUserAsync

Rule:

```txt
Chỉ Admin được khóa user
User phải tồn tại
```

Gọi Domain:

```txt
user.Lock()
```

Sau đó:

```txt
IUserRepository.UpdateAsync(user)
```

## ActivateUserAsync

Rule:

```txt
Chỉ Admin được kích hoạt user
User phải tồn tại
```

Gọi Domain:

```txt
user.Activate()
```

## DeleteUserAsync

Rule:

```txt
Chỉ Admin được xóa mềm user
User phải tồn tại
```

Gọi Domain:

```txt
user.Delete()
```

## GetUserRoleAsync

Rule:

```txt
User hiện tại phải đăng nhập
```

Trả về role của user được truy vấn.

---

# 9.3. EmployerProfileUseCase

File:

```txt
EnableVN.Application/UseCases/EmployerProfileUseCase.cs
```

Implement:

```txt
IEmployerProfileUseCase
```

Dependencies:

```txt
IEmployerProfileRepository
ICurrentUserService
IDomainEventDispatcher
```

Các method:

```txt
CreateAsync
UpdateMyProfileAsync
GetMyProfileAsync
GetByIdAsync
```

## CreateAsync

Input:

```txt
CreateEmployerProfileCommand
```

Luồng xử lý:

```txt
1. Require role Employer
2. Lấy UserId từ ICurrentUserService
3. Kiểm tra Employer đã có profile chưa
4. Tạo InclusiveWorkplaceInfo
5. Tạo EmployerProfile bằng EmployerProfile.Create
6. Lưu bằng IEmployerProfileRepository.AddAsync
7. Dispatch EmployerProfileCreatedEvent
8. Trả về profile.Id
```

Rule ở Application:

```txt
User hiện tại phải là Employer
Mỗi Employer chỉ nên có một profile
Không nhận UserId từ client
```

Rule ở Domain:

```txt
CompanyName hợp lệ
UserId không rỗng
Tạo EmployerProfileCreatedEvent
```

## UpdateMyProfileAsync

Luồng xử lý:

```txt
1. Require role Employer
2. Lấy profile theo current UserId
3. Nếu chưa có profile -> lỗi
4. Tạo InclusiveWorkplaceInfo mới
5. Gọi profile.UpdateCompanyInfo
6. Gọi profile.UpdateInclusiveWorkplaceInfo
7. Update repository
```

## GetMyProfileAsync

Rule:

```txt
User hiện tại phải là Employer
```

Trả:

```txt
EmployerProfileResult?
```

## GetByIdAsync

Dùng cho:

```txt
Trang public hoặc Admin xem hồ sơ doanh nghiệp
```

Trả:

```txt
EmployerProfileResult?
```

---

# 9.4. CandidateProfileUseCase

File:

```txt
EnableVN.Application/UseCases/CandidateProfileUseCase.cs
```

Implement:

```txt
ICandidateProfileUseCase
```

Dependencies:

```txt
ICandidateProfileRepository
ICurrentUserService
IDomainEventDispatcher
```

Các method:

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

## CreateAsync

Luồng xử lý:

```txt
1. Require role Candidate
2. Lấy UserId từ ICurrentUserService
3. Kiểm tra Candidate đã có profile chưa
4. Tạo CandidateProfile bằng CandidateProfile.Create
5. Lưu bằng repository
6. Dispatch CandidateProfileCreatedEvent
7. Trả profile.Id
```

Rule ở Application:

```txt
User hiện tại phải là Candidate
Mỗi Candidate chỉ nên có một profile
Không nhận UserId từ client
```

Rule ở Domain:

```txt
FullName hợp lệ
DisabilityInfo mặc định Hidden
IsPublicProfile mặc định false
CandidateProfileCreatedEvent được tạo
```

## UpdateMyProfileAsync

Luồng:

```txt
1. Require Candidate
2. Lấy profile của current user
3. Gọi profile.UpdateBasicInfo
4. Update repository
```

## UpdateMyDisabilityInfoAsync

Luồng:

```txt
1. Require Candidate
2. Lấy profile của current user
3. Tạo DisabilityInfo từ command
4. Gọi profile.UpdateDisabilityInfo
5. Update repository
```

## HideMyDisabilityInfoAsync

Gọi Domain:

```txt
profile.HideDisabilityInfo()
```

Ý nghĩa:

```txt
Ẩn thông tin khuyết tật khỏi Employer.
```

## ShowMyDisabilityInfoAsync

Gọi Domain:

```txt
profile.ShowDisabilityInfoToEmployer()
```

Ý nghĩa:

```txt
Cho phép Employer xem thông tin khuyết tật.
```

## MakeMyProfilePublicAsync

Gọi Domain:

```txt
profile.MakeProfilePublic()
```

Dùng cho phase 2:

```txt
Employer tìm ứng viên public profile
```

## MakeMyProfilePrivateAsync

Gọi Domain:

```txt
profile.MakeProfilePrivate()
```

## GetMyProfileAsync

Chính Candidate được xem đầy đủ thông tin của mình:

```txt
canViewDisabilityInfo = true
```

## GetPublicProfilesAsync

Rule:

```txt
Chỉ Employer được xem danh sách candidate public
```

Khi map:

```txt
canViewDisabilityInfo = profile.DisabilityInfo.IsVisibleToEmployer
```

Ý nghĩa:

```txt
Employer chỉ xem được thông tin khuyết tật nếu Candidate cho phép.
```

---

# 9.5. JobUseCase

File:

```txt
EnableVN.Application/UseCases/JobUseCase.cs
```

Implement:

```txt
IJobUseCase
```

Dependencies:

```txt
IJobRepository
IEmployerProfileRepository
ICurrentUserService
IDomainEventDispatcher
```

Các method:

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

## CreateDraftAsync

Input:

```txt
CreateJobCommand
```

Luồng xử lý:

```txt
1. Require role Employer
2. Lấy EmployerProfile theo current UserId
3. Nếu chưa có EmployerProfile -> lỗi
4. Tạo SalaryRange
5. Tạo JobAccessibilityInfo
6. Tạo JobPost bằng JobPost.CreateDraft
7. Lưu bằng IJobRepository.AddAsync
8. Trả job.Id
```

Rule ở Application:

```txt
User hiện tại phải là Employer
Employer phải có profile trước khi đăng tin
EmployerId lấy từ EmployerProfile.Id, không nhận từ client
```

Rule ở Domain:

```txt
Title hợp lệ
Description không rỗng
Requirement không rỗng
SalaryRange hợp lệ
Job tạo ở trạng thái Draft
```

## UpdateAsync

Luồng:

```txt
1. Require Employer
2. Lấy job và đảm bảo job thuộc Employer hiện tại
3. Tạo SalaryRange
4. Tạo JobAccessibilityInfo
5. Gọi job.UpdateContent
6. Update repository
```

## PublishAsync

Luồng:

```txt
1. Require Employer
2. Lấy job và đảm bảo thuộc Employer hiện tại
3. Gọi job.Publish()
4. Update repository
5. Dispatch JobPostedEvent
```

Rule ở Domain:

```txt
Không publish job đã Deleted
Không publish job đã Published
Set Status = Published
Set PublishedAt
Add JobPostedEvent
```

## CloseAsync

Luồng:

```txt
1. Require Employer
2. Lấy job và đảm bảo thuộc Employer hiện tại
3. Gọi job.Close()
4. Update repository
5. Dispatch JobClosedEvent
```

Rule ở Domain:

```txt
Chỉ job Published mới được Close
Set Status = Closed
Set ClosedAt
Add JobClosedEvent
```

## DeleteAsync

Luồng:

```txt
1. Require Employer
2. Lấy job và đảm bảo thuộc Employer hiện tại
3. Gọi job.Delete()
4. Update repository
```

## GetByIdAsync

Trả:

```txt
JobResult?
```

Dùng cho:

```txt
Xem chi tiết job
```

## GetMyJobsAsync

Rule:

```txt
User hiện tại phải là Employer
Employer phải có profile
```

Trả:

```txt
Danh sách JobResult của Employer hiện tại
```

## SearchPublishedJobsAsync

Input:

```txt
SearchJobQuery
```

Gọi repository:

```txt
SearchPublishedJobsAsync(
    keyword,
    workMode,
    supportsWheelchairAccess,
    supportsRemoteWork,
    supportsFlexibleTime,
    providesAssistiveDevices
)
```

Dùng cho:

```txt
Candidate hoặc guest tìm job đang Published
Filter theo accessibility
```

---

# 9.6. JobApplicationUseCase

File:

```txt
EnableVN.Application/UseCases/JobApplicationUseCase.cs
```

Implement:

```txt
IJobApplicationUseCase
```

Dependencies:

```txt
IJobApplicationRepository
IJobRepository
ICandidateProfileRepository
IEmployerProfileRepository
ICurrentUserService
IDomainEventDispatcher
```

Các method:

```txt
SubmitAsync
ChangeStatusAsync
WithdrawAsync
GetByJobIdAsync
GetMyApplicationsAsync
GetByIdAsync
```

## SubmitAsync

Input:

```txt
SubmitJobApplicationCommand
```

Luồng xử lý:

```txt
1. Require role Candidate
2. Lấy CandidateProfile theo current UserId
3. Nếu chưa có CandidateProfile -> lỗi
4. Lấy Job theo JobId
5. Nếu không tìm thấy Job -> lỗi
6. Kiểm tra job.CanReceiveApplication()
7. Kiểm tra Candidate đã nộp job này chưa
8. Nếu command.CvUrl rỗng thì dùng candidateProfile.CvUrl
9. Tạo JobApplication bằng JobApplication.Submit
10. Lưu bằng IJobApplicationRepository.AddAsync
11. Dispatch JobApplicationSubmittedEvent
12. Trả application.Id
```

Rule ở Application:

```txt
User hiện tại phải là Candidate
Candidate phải có profile
Job phải tồn tại
Job phải đang nhận hồ sơ
Candidate chưa từng nộp job này
CandidateId lấy từ CandidateProfile.Id, không nhận từ client
```

Rule ở Domain:

```txt
JobId không rỗng
CandidateId không rỗng
CvUrl không rỗng
Status mặc định Pending
SubmittedAt = now
Add JobApplicationSubmittedEvent
```

## ChangeStatusAsync

Input:

```txt
ChangeApplicationStatusCommand
```

Luồng xử lý:

```txt
1. Require role Employer
2. Lấy EmployerProfile theo current UserId
3. Lấy JobApplication theo ApplicationId
4. Lấy Job của Application
5. Đảm bảo Job.EmployerId == EmployerProfile.Id
6. Gọi application.ChangeStatus(newStatus, note)
7. Update repository
8. Dispatch JobApplicationStatusChangedEvent
```

Rule ở Application:

```txt
User hiện tại phải là Employer
Employer phải có profile
Application phải tồn tại
Job phải tồn tại
Job phải thuộc Employer hiện tại
```

Rule ở Domain:

```txt
Không đổi status nếu application đã Withdrawn
Không đổi status nếu application đã Rejected
Không đổi status nếu application đã Accepted
Không đổi sang chính status hiện tại
Add ApplicationStatusHistory
Add JobApplicationStatusChangedEvent
```

## WithdrawAsync

Luồng:

```txt
1. Require role Candidate
2. Lấy CandidateProfile theo current UserId
3. Lấy Application theo applicationId
4. Đảm bảo application.CandidateId == candidateProfile.Id
5. Gọi application.Withdraw()
6. Update repository
```

Rule ở Domain:

```txt
Không rút hồ sơ đã Accepted
Không cần rút hồ sơ đã Rejected
Set Status = Withdrawn
Add ApplicationStatusHistory
```

## GetByJobIdAsync

Rule:

```txt
User hiện tại phải là Employer
Employer phải có profile
Job phải tồn tại
Job phải thuộc Employer hiện tại
```

Trả:

```txt
Danh sách JobApplicationResult theo JobId
```

## GetMyApplicationsAsync

Rule:

```txt
User hiện tại phải là Candidate
Candidate phải có profile
```

Trả:

```txt
Danh sách JobApplicationResult của Candidate hiện tại
```

## GetByIdAsync

Hiện tại rule:

```txt
Chỉ yêu cầu user đã đăng nhập
```

Lưu ý cải tiến sau:

```txt
Nên kiểm tra thêm:
- Candidate chỉ xem application của mình
- Employer chỉ xem application thuộc job của mình
- Admin có thể xem tất cả nếu cần
```

---

# 9.7. CatalogUseCase

File:

```txt
EnableVN.Application/UseCases/CatalogUseCase.cs
```

Implement:

```txt
ICatalogUseCase
```

Dependencies:

```txt
IDisabilityTypeRepository
IAssistiveDeviceRepository
IJobCategoryRepository
ICurrentUserService
```

Quản lý 3 nhóm danh mục:

```txt
DisabilityType
AssistiveDevice
JobCategory
```

---

## DisabilityType methods

```txt
CreateDisabilityTypeAsync
UpdateDisabilityTypeAsync
DeactivateDisabilityTypeAsync
ActivateDisabilityTypeAsync
GetActiveDisabilityTypesAsync
```

Rule:

```txt
Create/Update/Activate/Deactivate yêu cầu Admin
GetActiveDisabilityTypesAsync không yêu cầu Admin để phục vụ dropdown public
```

---

## AssistiveDevice methods

```txt
CreateAssistiveDeviceAsync
UpdateAssistiveDeviceAsync
DeactivateAssistiveDeviceAsync
ActivateAssistiveDeviceAsync
GetActiveAssistiveDevicesAsync
```

Rule:

```txt
Create/Update/Activate/Deactivate yêu cầu Admin
GetActiveAssistiveDevicesAsync không yêu cầu Admin
```

---

## JobCategory methods

```txt
CreateJobCategoryAsync
UpdateJobCategoryAsync
DeactivateJobCategoryAsync
ActivateJobCategoryAsync
GetActiveJobCategoriesAsync
```

Rule:

```txt
Create/Update/Activate/Deactivate yêu cầu Admin
GetActiveJobCategoriesAsync không yêu cầu Admin
```

Lưu ý:

```txt
JobCategory đã được bổ sung ở Domain vì Ports có IJobCategoryRepository.
```

---

# 10. Rule quan trọng đã thống nhất trong Application

## 10.1. Không nhận UserId từ client

Các command không nhận UserId/EmployerId/CandidateId từ client:

```txt
CreateEmployerProfileCommand
CreateCandidateProfileCommand
CreateJobCommand
SubmitJobApplicationCommand
```

Application lấy user hiện tại từ:

```txt
ICurrentUserService
```

Mục tiêu:

```txt
Tránh client giả mạo UserId, EmployerId, CandidateId
```

---

## 10.2. Không trả Domain Entity trực tiếp

Không trả:

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

Mà map sang:

```txt
AuthResult
EmployerProfileResult
CandidateProfileResult
JobResult
JobApplicationResult
CatalogItemResult
```

---

## 10.3. Rule cần repository thì đặt ở Application

Ví dụ:

```txt
Email đã tồn tại chưa
Employer đã có profile chưa
Candidate đã có profile chưa
Candidate đã nộp job này chưa
Job có thuộc Employer hiện tại không
Application có thuộc Candidate hiện tại không
```

---

## 10.4. Rule thuần Aggregate thì đặt ở Domain

Ví dụ:

```txt
Email hợp lệ
FullName hợp lệ
SalaryRange hợp lệ
Không publish job đã Deleted
Chỉ job Published mới close
Không đổi trạng thái application đã Accepted/Rejected/Withdrawn
```

---

# 11. Domain Event trong Application

Các UseCase có dispatch event:

```txt
AuthUseCase.RegisterAsync
EmployerProfileUseCase.CreateAsync
CandidateProfileUseCase.CreateAsync
JobUseCase.PublishAsync
JobUseCase.CloseAsync
JobApplicationUseCase.SubmitAsync
JobApplicationUseCase.ChangeStatusAsync
```

Các event tương ứng:

```txt
UserRegisteredEvent
EmployerProfileCreatedEvent
CandidateProfileCreatedEvent
JobPostedEvent
JobClosedEvent
JobApplicationSubmittedEvent
JobApplicationStatusChangedEvent
```

Hiện tại chưa cần handler thật.

Sau này Infrastructure/Application có thể xử lý:

```txt
Gửi email
Tạo notification
Ghi audit log
Đồng bộ search index
Thống kê
```

---

# 12. Các dependency của từng UseCase

## AuthUseCase

```txt
IUserRepository
IPasswordHasher
ITokenService
IDomainEventDispatcher
```

## UserUseCase

```txt
IUserRepository
ICurrentUserService
```

## EmployerProfileUseCase

```txt
IEmployerProfileRepository
ICurrentUserService
IDomainEventDispatcher
```

## CandidateProfileUseCase

```txt
ICandidateProfileRepository
ICurrentUserService
IDomainEventDispatcher
```

## JobUseCase

```txt
IJobRepository
IEmployerProfileRepository
ICurrentUserService
IDomainEventDispatcher
```

## JobApplicationUseCase

```txt
IJobApplicationRepository
IJobRepository
ICandidateProfileRepository
IEmployerProfileRepository
ICurrentUserService
IDomainEventDispatcher
```

## CatalogUseCase

```txt
IDisabilityTypeRepository
IAssistiveDeviceRepository
IJobCategoryRepository
ICurrentUserService
```

---

# 13. Các điểm cần kiểm tra compile

## 13.1. Phải có ITokenService

Nếu `AuthUseCase` dùng:

```txt
ITokenService
```

thì Ports phải có:

```txt
EnableVN.Ports/Outbound/Services/ITokenService.cs
```

## 13.2. Phải đổi ApplicationException thành UseCaseException

Không dùng tên:

```txt
ApplicationException
```

vì trùng với:

```txt
System.ApplicationException
```

Dùng:

```txt
UseCaseException
```

## 13.3. Các file UseCase cần using đúng

Ví dụ:

```csharp
using EnableVN.Application.Common;
using EnableVN.Application.Mappers;
using EnableVN.Ports.Inbound;
using EnableVN.Ports.Outbound.Repositories;
using EnableVN.Ports.Outbound.Services;
```

## 13.4. Các mapper cần Domain property đúng tên

Ví dụ:

```txt
CompanyName.Value
FullName.Value
JobTitle.Value
SalaryRange.MinSalary
SalaryRange.MaxSalary
JobAccessibilityInfo.SupportsWheelchairAccess
```

Nếu Domain property khác tên, cần sửa mapper tương ứng.

---

# 14. Các điểm có thể cải tiến sau

## 14.1. GetByIdAsync của JobApplicationUseCase

Hiện tại chỉ check:

```txt
User đã đăng nhập
```

Nên cải tiến:

```txt
Nếu Candidate:
    chỉ xem application của chính Candidate đó

Nếu Employer:
    chỉ xem application thuộc Job của Employer đó

Nếu Admin:
    có thể xem tất cả
```

## 14.2. Thêm Unit of Work

Hiện tại repository có `AddAsync/UpdateAsync`.

Sau này khi dùng EF Core có thể thêm:

```txt
IUnitOfWork
SaveChangesAsync
```

Để đảm bảo transaction rõ ràng.

## 14.3. DomainEvent dispatch sau transaction

Hiện tại Application dispatch event sau repository Add/Update.

Khi có EF Core, nên dispatch sau khi `SaveChangesAsync` thành công.

## 14.4. Validation input ở Presentation/Application

Domain có validation nghiệp vụ.

Nhưng Presentation/Application có thể bổ sung validation format/input:

```txt
Password length
Required fields
Max length request
File size
File content type
```

## 14.5. Thêm Result pattern thay cho exception

Hiện tại dùng exception.

Sau này có thể dùng:

```txt
Result<T>
ErrorCode
ValidationError
```

Nếu muốn API response chuẩn hơn.

---

# 15. Bước tiếp theo nên làm

Sau khi Application đã có code nền tảng, bước tiếp theo nên là:

```txt
EnableVN.InfrastructureInMemory
```

Lý do làm InMemory trước:

```txt
Dễ test UseCase ngay
Không cần SQLite/EF Core
Có thể demo flow đăng ký, tạo profile, tạo job, nộp hồ sơ
Giúp phát hiện lỗi logic trước khi làm database thật
```

InfrastructureInMemory sẽ implement:

```txt
IUserRepository
IEmployerProfileRepository
ICandidateProfileRepository
IJobRepository
IJobApplicationRepository
IDisabilityTypeRepository
IAssistiveDeviceRepository
IJobCategoryRepository
IPasswordHasher
ITokenService
IDomainEventDispatcher
ICurrentUserService dạng fake/test nếu cần
```

Sau InMemory mới nên làm:

```txt
EnableVN.InfrastructureSqlite
```

Rồi cuối cùng:

```txt
EnableVN.Presentation
```

---

# 16. Câu lệnh nên gửi trong phiên mới

Có thể gửi file này cho ChatGPT và nói:

```txt
Dựa trên file EnableVN_Application_Summary.md này, hãy tiếp tục làm EnableVN.InfrastructureInMemory.
Yêu cầu:
- Code C#
- Có comment giải thích trong code
- Implement các Outbound Port từ EnableVN.Ports
- Không phụ thuộc Presentation
- Không dùng database thật
- Lưu dữ liệu bằng collection trong RAM
- Phù hợp để test các UseCase trong EnableVN.Application
```

---

# 17. Tóm tắt ngắn gọn

Đã hoàn thành:

```txt
Domain: Entity, AggregateRoot, ValueObject, Policy, DomainEvent
Ports: Inbound, Outbound, Models
Application: Common, Mapper, UseCases
```

Application đã implement:

```txt
AuthUseCase
UserUseCase
EmployerProfileUseCase
CandidateProfileUseCase
JobUseCase
JobApplicationUseCase
CatalogUseCase
```

Đã sửa lỗi tên:

```txt
ApplicationException -> UseCaseException
```

Đã bổ sung cần thiết:

```txt
ITokenService
```

Bước tiếp theo:

```txt
Làm EnableVN.InfrastructureInMemory
```
