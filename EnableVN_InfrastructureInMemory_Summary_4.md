# EnableVN_InfrastructureInMemory_Summary.md

> **Đồng bộ code:** SQLite + Presentation **đã có**; InMemory **vẫn** dùng cho password/token/email và các phần chưa có Sqlite. Hai repo InMemory review/violation: file trong `Ports/Outbound/Repositories/`. **Cần** `AddSingleton<IViolationReportRepository, InMemoryViolationReportRepository>()` — xem Summary 8.

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

Hiện tại đã làm (đồng bộ code):

```txt
EnableVN.Domain                    ✅
EnableVN.Ports                   ✅
EnableVN.Application             ✅
EnableVN.InfrastructureInMemory  ✅
EnableVN.InfrastructureSqlite    ✅
EnableVN.Presentation (ENABLEVN/) ✅
```

**Lưu ý:** Dữ liệu SQLite ghi đè repository core; `ICompanyReviewRepository` vẫn dùng implementation InMemory (Singleton). `IViolationReportRepository` có class `InMemoryViolationReportRepository` nhưng **chưa đăng ký DI** trong `InfrastructureInMemory/DependencyInjection.cs` tại thời điểm đồng bộ tài liệu.

---

# 2. Vai trò của EnableVN.InfrastructureInMemory

`EnableVN.InfrastructureInMemory` là tầng hạ tầng tạm thời, dùng để implement các Outbound Port bằng bộ nhớ RAM.

Mục tiêu:

```txt
Chạy thử các UseCase mà chưa cần database thật
Test flow MVP nhanh
Không dùng SQLite
Không dùng Entity Framework
Không cần migration
Dễ debug logic Application
```

Dữ liệu trong InMemory:

```txt
Được lưu trong List<T>
Mất khi app restart
Không phù hợp production
Phù hợp test/demo/dev giai đoạn đầu
```

---

# 3. Vị trí trong Hexagonal Architecture

Luồng phụ thuộc:

```txt
Application
    ↓ gọi interface
Ports.Outbound
    ↑ implement bởi
InfrastructureInMemory
```

Ví dụ:

```txt
Application gọi IUserRepository
InfrastructureInMemory cung cấp InMemoryUserRepository
```

`InfrastructureInMemory` được phép reference:

```txt
EnableVN.Domain
EnableVN.Ports
```

`InfrastructureInMemory` không nên reference:

```txt
EnableVN.Application
EnableVN.InfrastructureSqlite
EnableVN.Presentation
```

Lưu ý:

```txt
Trong thực tế có thể Presentation gọi extension method đăng ký DI của Application và InMemory.
Nhưng InMemory không cần phụ thuộc Application.
```

---

# 4. Cấu trúc project đã thiết kế

```txt
InfrastructureInMemory/
│
├── Repositories/
│   ├── InMemoryUserRepository.cs
│   ├── InMemoryEmployerProfileRepository.cs
│   ├── InMemoryCandidateProfileRepository.cs
│   ├── InMemoryJobRepository.cs
│   ├── InMemoryJobApplicationRepository.cs
│   ├── InMemoryDisabilityTypeRepository.cs
│   ├── InMemoryAssistiveDeviceRepository.cs
│   ├── InMemoryJobCategoryRepository.cs
│   ├── InMemoryCurrentUserService.cs
│   ├── SimplePasswordHasher.cs
│   ├── SimpleTokenService.cs
│   └── InMemoryDomainEventDispatcher.cs
│
├── Services/
│   └── SmtpEmailService.cs
│
└── DependencyInjection.cs
```

**Đồng bộ code:** `InMemoryCompanyReviewRepository.cs` và `InMemoryViolationReportRepository.cs` nằm trong project **`Ports/`** (`Ports/Outbound/Repositories/`), namespace `InfrastructureInMemory.Repositories`, nhưng vẫn được đăng ký từ `InfrastructureInMemory/DependencyInjection.cs` (trừ `IViolationReportRepository` — chưa đăng ký).

---

# 5. Các Repository đã implement

Tất cả repository trong `InfrastructureInMemory` dùng:

```txt
private readonly List<T> _items = new();
```

hoặc tên tương ứng như:

```txt
_users
_profiles
_jobs
_applications
```

Các method đều trả về `Task` để khớp với interface async trong Ports.

---

# 6. InMemoryUserRepository

File:

```txt
EnableVN.InfrastructureInMemory/Repositories/InMemoryUserRepository.cs
```

Implement:

```txt
IUserRepository
```

Lưu dữ liệu:

```txt
List<User> _users
```

Method đã implement:

```txt
GetByIdAsync
GetByEmailAsync
ExistsByEmailAsync
AddAsync
UpdateAsync
```

Ý nghĩa:

```txt
Dùng cho đăng ký, đăng nhập, khóa/kích hoạt/xóa mềm user.
```

Chi tiết:

```txt
GetByEmailAsync:
    normalize email bằng Trim().ToLower()
    tìm user theo user.Email.Value

ExistsByEmailAsync:
    kiểm tra email đã tồn tại chưa

AddAsync:
    thêm User vào List

UpdateAsync:
    tìm theo Id
    nếu có thì thay object cũ bằng object mới
```

---

# 7. InMemoryEmployerProfileRepository

File:

```txt
EnableVN.InfrastructureInMemory/Repositories/InMemoryEmployerProfileRepository.cs
```

Implement:

```txt
IEmployerProfileRepository
```

Lưu dữ liệu:

```txt
List<EmployerProfile> _profiles
```

Method đã implement:

```txt
GetByIdAsync
GetByUserIdAsync
ExistsByUserIdAsync
AddAsync
UpdateAsync
```

Ý nghĩa:

```txt
Dùng cho Employer tạo/cập nhật/xem hồ sơ doanh nghiệp.
```

Rule hỗ trợ ở Application:

```txt
Mỗi Employer chỉ nên có một EmployerProfile.
```

Repository cung cấp:

```txt
ExistsByUserIdAsync
```

để Application kiểm tra trước khi tạo profile.

---

# 8. InMemoryCandidateProfileRepository

File:

```txt
EnableVN.InfrastructureInMemory/Repositories/InMemoryCandidateProfileRepository.cs
```

Implement:

```txt
ICandidateProfileRepository
```

Lưu dữ liệu:

```txt
List<CandidateProfile> _profiles
```

Method đã implement:

```txt
GetByIdAsync
GetByUserIdAsync
ExistsByUserIdAsync
GetPublicProfilesAsync
AddAsync
UpdateAsync
```

Ý nghĩa:

```txt
Dùng cho Candidate tạo/cập nhật profile.
Dùng cho Employer xem danh sách Candidate đã public profile ở phase 2.
```

Chi tiết:

```txt
GetPublicProfilesAsync:
    lọc profile có IsPublicProfile == true
```

Lưu ý:

```txt
Việc ẩn/hiện thông tin khuyết tật không xử lý ở repository.
Application mapper quyết định có map thông tin khuyết tật ra result hay không.
```

---

# 9. InMemoryJobRepository

File:

```txt
EnableVN.InfrastructureInMemory/Repositories/InMemoryJobRepository.cs
```

Implement:

```txt
IJobRepository
```

Lưu dữ liệu:

```txt
List<JobPost> _jobs
```

Method đã implement:

```txt
GetByIdAsync
GetByEmployerIdAsync
SearchPublishedJobsAsync
AddAsync
UpdateAsync
```

Ý nghĩa:

```txt
Dùng cho Employer tạo/sửa/publish/close/delete job.
Dùng cho Candidate hoặc guest tìm job đang published.
```

## SearchPublishedJobsAsync

Search này hỗ trợ:

```txt
keyword
workMode
supportsWheelchairAccess
supportsRemoteWork
supportsFlexibleTime
providesAssistiveDevices
```

Luồng lọc:

```txt
1. Chỉ lấy job có Status == JobStatus.Published
2. Nếu có keyword:
   - tìm trong Title
   - Description
   - Requirement
3. Nếu có WorkMode:
   - lọc theo WorkMode
4. Nếu có supportsWheelchairAccess:
   - lọc theo AccessibilityInfo.SupportsWheelchairAccess
5. Nếu có supportsRemoteWork:
   - lọc theo AccessibilityInfo.SupportsRemoteWork
6. Nếu có supportsFlexibleTime:
   - lọc theo AccessibilityInfo.SupportsFlexibleTime
7. Nếu có providesAssistiveDevices:
   - lọc theo AccessibilityInfo.ProvidesAssistiveDevices
8. Sort giảm dần theo PublishedAt hoặc CreatedAt
```

Lưu ý:

```txt
Filter bool? có nghĩa:
null = không lọc
true = chỉ lấy job có hỗ trợ tiêu chí đó
false = chỉ lấy job không hỗ trợ tiêu chí đó
```

---

# 10. InMemoryJobApplicationRepository

File:

```txt
EnableVN.InfrastructureInMemory/Repositories/InMemoryJobApplicationRepository.cs
```

Implement:

```txt
IJobApplicationRepository
```

Lưu dữ liệu:

```txt
List<JobApplication> _applications
```

Method đã implement:

```txt
GetByIdAsync
GetByJobIdAsync
GetByCandidateIdAsync
ExistsByJobIdAndCandidateIdAsync
AddAsync
UpdateAsync
```

Ý nghĩa:

```txt
Dùng cho Candidate nộp hồ sơ.
Dùng cho Candidate xem hồ sơ đã nộp.
Dùng cho Employer xem hồ sơ theo Job.
Dùng cho Employer đổi trạng thái hồ sơ.
```

Chi tiết:

```txt
ExistsByJobIdAndCandidateIdAsync:
    kiểm tra Candidate đã nộp vào Job này chưa
```

Rule này nằm ở Application vì cần kiểm tra dữ liệu đã lưu.

Repository chỉ cung cấp khả năng truy vấn.

---

# 11. InMemoryDisabilityTypeRepository

File:

```txt
EnableVN.InfrastructureInMemory/Repositories/InMemoryDisabilityTypeRepository.cs
```

Implement:

```txt
IDisabilityTypeRepository
```

Lưu dữ liệu:

```txt
List<DisabilityType> _items
```

Method đã implement:

```txt
GetByIdAsync
GetAllAsync
GetActiveAsync
AddAsync
UpdateAsync
```

Ý nghĩa:

```txt
Dùng cho Admin quản lý danh mục loại khuyết tật.
Dùng cho form Candidate chọn loại khuyết tật.
```

Chi tiết:

```txt
GetActiveAsync:
    lọc item có Status == CatalogStatus.Active
```

---

# 12. InMemoryAssistiveDeviceRepository

File:

```txt
EnableVN.InfrastructureInMemory/Repositories/InMemoryAssistiveDeviceRepository.cs
```

Implement:

```txt
IAssistiveDeviceRepository
```

Lưu dữ liệu:

```txt
List<AssistiveDevice> _items
```

Method đã implement:

```txt
GetByIdAsync
GetAllAsync
GetActiveAsync
AddAsync
UpdateAsync
```

Ý nghĩa:

```txt
Dùng cho Admin quản lý danh mục thiết bị hỗ trợ.
Dùng cho dropdown/form liên quan đến hỗ trợ tiếp cận.
```

---

# 13. InMemoryJobCategoryRepository

File:

```txt
EnableVN.InfrastructureInMemory/Repositories/InMemoryJobCategoryRepository.cs
```

Implement:

```txt
IJobCategoryRepository
```

Lưu dữ liệu:

```txt
List<JobCategory> _items
```

Method đã implement:

```txt
GetByIdAsync
GetAllAsync
GetActiveAsync
AddAsync
UpdateAsync
```

Ý nghĩa:

```txt
Dùng cho Admin quản lý danh mục ngành nghề / nhóm công việc.
Có thể dùng sau này để lọc job theo ngành nghề.
```

Lưu ý:

```txt
JobCategory đã được bổ sung vào Domain vì Ports có IJobCategoryRepository.
```

---

# 14. Các Service đã implement

---

# 15. InMemoryCurrentUserService

File:

```txt
EnableVN.InfrastructureInMemory/Services/InMemoryCurrentUserService.cs
```

Implement:

```txt
ICurrentUserService
```

Thuộc tính:

```txt
Guid? UserId
UserRole? Role
bool IsAuthenticated
```

Method bổ sung riêng cho InMemory:

```txt
SetCurrentUser(Guid userId, UserRole role)
Clear()
```

Ý nghĩa:

```txt
Dùng để giả lập user hiện tại khi test/demo.
Không cần JWT thật.
Không cần HttpContext.
```

Ví dụ dùng trong test:

```csharp
var currentUser = serviceProvider.GetRequiredService<ICurrentUserService>();
var inMemoryCurrentUser = (InMemoryCurrentUserService)currentUser;

inMemoryCurrentUser.SetCurrentUser(userId, UserRole.Employer);
```

Lưu ý:

```txt
ICurrentUserService interface không có SetCurrentUser.
SetCurrentUser chỉ có trong implementation InMemory để phục vụ test/demo.
```

---

# 16. SimplePasswordHasher

File:

```txt
EnableVN.InfrastructureInMemory/Services/SimplePasswordHasher.cs
```

Implement:

```txt
IPasswordHasher
```

Method:

```txt
Hash(string plainPassword)
Verify(string plainPassword, string passwordHash)
```

Cách hash hiện tại:

```txt
SHA256
Convert.ToBase64String
```

Lưu ý bảo mật:

```txt
SimplePasswordHasher chỉ dùng cho InMemory/demo/test.
Không dùng production.
Production nên dùng BCrypt, Argon2, PBKDF2 hoặc ASP.NET Identity PasswordHasher.
```

Ý nghĩa:

```txt
Giúp AuthUseCase chạy được Register/Login mà chưa cần thư viện password thật.
```

---

# 17. SimpleTokenService

File:

```txt
EnableVN.InfrastructureInMemory/Services/SimpleTokenService.cs
```

Implement:

```txt
ITokenService
```

Method:

```txt
GenerateToken(Guid userId, string email, UserRole role)
```

Cách tạo token hiện tại:

```txt
Ghép userId|email|role|timestamp
Encode Base64
```

Lưu ý bảo mật:

```txt
Đây không phải JWT thật.
Không dùng production.
Production sẽ implement ITokenService bằng JWT hoặc session token an toàn.
```

Ý nghĩa:

```txt
Giúp AuthUseCase trả AuthResult.Token để test flow đăng ký/đăng nhập.
```

---

# 18. InMemoryDomainEventDispatcher

File:

```txt
EnableVN.InfrastructureInMemory/Services/InMemoryDomainEventDispatcher.cs
```

Implement:

```txt
IDomainEventDispatcher
```

Lưu dữ liệu:

```txt
List<IDomainEvent> _dispatchedEvents
```

Thuộc tính public:

```txt
IReadOnlyList<IDomainEvent> DispatchedEvents
```

Method:

```txt
DispatchAsync(IDomainEvent domainEvent)
DispatchAsync(IEnumerable<IDomainEvent> domainEvents)
Clear()
```

Ý nghĩa:

```txt
Không xử lý event thật.
Chỉ lưu event đã dispatch vào RAM.
Dùng để debug/test xem event có được tạo và dispatch không.
```

Sau này Infrastructure thật có thể xử lý event bằng:

```txt
Email
Notification
Audit log
Message queue
Search indexing
```

---

# 19. DependencyInjection trong InfrastructureInMemory

File:

```txt
EnableVN.InfrastructureInMemory/DependencyInjection.cs
```

Có extension method:

```csharp
public static IServiceCollection AddEnableVNInMemoryInfrastructure(
    this IServiceCollection services
)
```

Method này đăng ký tất cả implementation InMemory vào DI container.

## Repository registrations

```csharp
services.AddSingleton<IUserRepository, InMemoryUserRepository>();
services.AddSingleton<IEmployerProfileRepository, InMemoryEmployerProfileRepository>();
services.AddSingleton<ICandidateProfileRepository, InMemoryCandidateProfileRepository>();
services.AddSingleton<IJobRepository, InMemoryJobRepository>();
services.AddSingleton<IJobApplicationRepository, InMemoryJobApplicationRepository>();
services.AddSingleton<IDisabilityTypeRepository, InMemoryDisabilityTypeRepository>();
services.AddSingleton<IAssistiveDeviceRepository, InMemoryAssistiveDeviceRepository>();
services.AddSingleton<IJobCategoryRepository, InMemoryJobCategoryRepository>();
```

## Service registrations

```csharp
services.AddSingleton<ICurrentUserService, InMemoryCurrentUserService>();
services.AddSingleton<IPasswordHasher, SimplePasswordHasher>();
services.AddSingleton<ITokenService, SimpleTokenService>();
services.AddSingleton<IDomainEventDispatcher, InMemoryDomainEventDispatcher>();
```

Lý do dùng Singleton:

```txt
Dữ liệu RAM được giữ trong suốt vòng đời app.
Nếu dùng Transient hoặc Scoped, dữ liệu có thể reset liên tục khi tạo instance mới.
```

---

# 20. Lưu ý quan trọng về Singleton và CurrentUser

Trong InMemory đã đăng ký:

```csharp
services.AddSingleton<ICurrentUserService, InMemoryCurrentUserService>();
```

Cách này phù hợp cho:

```txt
Console demo
Unit test đơn giản
Manual test một user tại một thời điểm
```

Không phù hợp cho Web API production vì:

```txt
Current user khác nhau theo từng request.
Singleton có thể làm rò rỉ user giữa request.
```

Khi làm Presentation thật, nên tạo:

```txt
HttpContextCurrentUserService
```

và đăng ký:

```csharp
services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();
```

Nếu vẫn dùng InMemory repository trong Web API demo, có thể vẫn dùng InMemory repository Singleton, nhưng `ICurrentUserService` nên thay bằng implementation đọc từ HTTP request.

---

# 21. Package cần thêm

Vì có dùng `IServiceCollection`, project `EnableVN.InfrastructureInMemory` cần package:

```txt
Microsoft.Extensions.DependencyInjection.Abstractions
```

Cài bằng CLI:

```bash
dotnet add EnableVN.InfrastructureInMemory package Microsoft.Extensions.DependencyInjection.Abstractions
```

Nếu Application có `DependencyInjection.cs`, project `EnableVN.Application` cũng cần package này:

```bash
dotnet add EnableVN.Application package Microsoft.Extensions.DependencyInjection.Abstractions
```

---

# 22. Application DependencyInjection đã đề xuất

Trong `EnableVN.Application` nên có file:

```txt
EnableVN.Application/DependencyInjection.cs
```

Có method:

```csharp
public static IServiceCollection AddEnableVNApplication(
    this IServiceCollection services
)
```

Đăng ký:

```csharp
services.AddScoped<IAuthUseCase, AuthUseCase>();
services.AddScoped<IUserUseCase, UserUseCase>();
services.AddScoped<IEmployerProfileUseCase, EmployerProfileUseCase>();
services.AddScoped<ICandidateProfileUseCase, CandidateProfileUseCase>();
services.AddScoped<IJobUseCase, JobUseCase>();
services.AddScoped<IJobApplicationUseCase, JobApplicationUseCase>();
services.AddScoped<ICatalogUseCase, CatalogUseCase>();
```

Mục đích:

```txt
Presentation chỉ cần gọi AddEnableVNApplication().
Không phải tự đăng ký từng UseCase.
```

---

# 23. Cách đăng ký trong Presentation hoặc Test

Ví dụ trong `Program.cs`:

```csharp
using EnableVN.Application;
using EnableVN.InfrastructureInMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEnableVNApplication();
builder.Services.AddEnableVNInMemoryInfrastructure();

var app = builder.Build();

app.Run();
```

Sau đó các controller/API có thể inject:

```txt
IAuthUseCase
IJobUseCase
ICandidateProfileUseCase
IEmployerProfileUseCase
IJobApplicationUseCase
ICatalogUseCase
```

---

# 24. Cách giả lập user hiện tại trong test/demo

Do `ICurrentUserService` chỉ expose thông tin user hiện tại, không có setter, muốn giả lập user cần cast về `InMemoryCurrentUserService`.

Ví dụ:

```csharp
using EnableVN.Domain.Users;
using EnableVN.InfrastructureInMemory.Services;
using EnableVN.Ports.Outbound.Services;

var currentUser = serviceProvider.GetRequiredService<ICurrentUserService>();

var inMemoryCurrentUser = (InMemoryCurrentUserService)currentUser;

inMemoryCurrentUser.SetCurrentUser(
    userId,
    UserRole.Employer
);
```

Clear user:

```csharp
inMemoryCurrentUser.Clear();
```

---

# 25. Flow test MVP có thể chạy bằng InMemory

Sau khi đã có:

```txt
Domain
Ports
Application
InfrastructureInMemory
DI
```

có thể test các flow sau:

## Flow 1: Employer đăng ký và tạo hồ sơ

```txt
1. Gọi IAuthUseCase.RegisterAsync với Role = Employer
2. Lấy UserId từ AuthResult
3. SetCurrentUser(UserId, Employer)
4. Gọi IEmployerProfileUseCase.CreateAsync
```

## Flow 2: Employer tạo và publish job

```txt
1. SetCurrentUser employer
2. Gọi IJobUseCase.CreateDraftAsync
3. Gọi IJobUseCase.PublishAsync
4. JobPostedEvent được dispatch vào InMemoryDomainEventDispatcher
```

## Flow 3: Candidate đăng ký và tạo hồ sơ

```txt
1. Gọi IAuthUseCase.RegisterAsync với Role = Candidate
2. SetCurrentUser(UserId, Candidate)
3. Gọi ICandidateProfileUseCase.CreateAsync
4. Có thể gọi UpdateMyDisabilityInfoAsync
5. Có thể Hide/Show disability info
```

## Flow 4: Candidate tìm job

```txt
1. Gọi IJobUseCase.SearchPublishedJobsAsync
2. Lọc theo keyword/workMode/accessibility
```

## Flow 5: Candidate nộp hồ sơ

```txt
1. SetCurrentUser candidate
2. Gọi IJobApplicationUseCase.SubmitAsync
3. JobApplicationSubmittedEvent được dispatch
```

## Flow 6: Employer đổi trạng thái hồ sơ

```txt
1. SetCurrentUser employer
2. Gọi IJobApplicationUseCase.GetByJobIdAsync
3. Gọi IJobApplicationUseCase.ChangeStatusAsync
4. JobApplicationStatusChangedEvent được dispatch
```

---

# 26. Những giới hạn của InfrastructureInMemory

InfrastructureInMemory không có:

```txt
Persistence thật
Transaction thật
Concurrency control
Migration
Index database
Foreign key
Query optimization
Security token thật
Password hashing an toàn production
Current user theo HTTP request thật
```

Do đó:

```txt
Chỉ dùng cho dev/test/demo.
Không dùng production.
```

---

# 27. Những điểm có thể cải tiến sau

## 27.1. Tạo base repository generic

Hiện tại mỗi repository tự có List riêng.

Có thể tạo base class:

```txt
InMemoryRepositoryBase<TEntity>
```

Nhưng hiện tại chưa cần vì mỗi repository có query riêng.

## 27.2. Seed data

Có thể thêm:

```txt
InMemoryDataSeeder
```

Để seed:

```txt
Admin user
DisabilityType mặc định
AssistiveDevice mặc định
JobCategory mặc định
```

## 27.3. Fake CurrentUser tốt hơn

Có thể tạo thêm interface riêng cho test:

```txt
ITestCurrentUserSetter
```

để tránh cast từ `ICurrentUserService` sang `InMemoryCurrentUserService`.

## 27.4. Thread-safety

List<T> hiện tại không thread-safe.

Nếu test multi-thread hoặc Web API nhiều request đồng thời, có thể đổi sang:

```txt
ConcurrentDictionary<Guid, T>
```

Nhưng MVP/dev hiện tại dùng List là đủ đơn giản.

---

# 28. Bước tiếp theo nên làm

Sau `InfrastructureInMemory`, có 2 hướng:

## Hướng khuyến nghị: làm Presentation trước

Làm:

```txt
EnableVN.Presentation
```

Dùng InMemory để test API flow MVP ngay.

Ưu điểm:

```txt
Test được toàn bộ luồng từ API -> Application -> InMemory
Phát hiện thiếu DTO/endpoint/usecase sớm
Chưa cần quan tâm EF Core/SQLite
```

## Hướng khác: làm InfrastructureSqlite trước

Làm:

```txt
EnableVN.InfrastructureSqlite
```

Ưu điểm:

```txt
Có database thật sớm
Chuẩn bị persistence production hơn
```

Nhược điểm:

```txt
Phức tạp hơn
Cần mapping Domain -> PersistenceModel
Cần DbContext/migration
Có thể làm chậm việc test flow API
```

Khuyến nghị hiện tại:

```txt
Làm Presentation tối thiểu với InMemory trước.
Sau khi flow MVP chạy được thì làm SQLite.
```

---

# 29. Câu lệnh nên gửi trong phiên mới

Có thể gửi file này cho ChatGPT kèm yêu cầu:

```txt
Dựa trên file EnableVN_InfrastructureInMemory_Summary.md này, hãy tiếp tục làm EnableVN.Presentation tối thiểu để test các flow MVP bằng InMemory.
Yêu cầu:
- Code C#
- ASP.NET Core Web API hoặc Minimal API
- Có comment giải thích trong code
- Inject các Inbound Port từ Ports
- Gọi AddEnableVNApplication()
- Gọi AddEnableVNInMemoryInfrastructure()
- Tạo endpoint cho Auth, EmployerProfile, CandidateProfile, Job, JobApplication, Catalog
- Chưa cần SQLite
```

Hoặc nếu muốn làm SQLite trước:

```txt
Dựa trên file EnableVN_InfrastructureInMemory_Summary.md này, hãy tiếp tục làm EnableVN.InfrastructureSqlite.
Yêu cầu:
- Code C#
- EF Core SQLite
- Implement các Outbound Repository từ Ports
- Có PersistenceModel nếu cần
- Không để Domain phụ thuộc EF Core
```

---

# 30. Tóm tắt ngắn gọn

`EnableVN.InfrastructureInMemory` đã làm:

```txt
Implement repository bằng List<T> trong RAM
Implement current user giả lập
Implement password hasher đơn giản
Implement token service đơn giản
Implement domain event dispatcher lưu event vào RAM
Tạo DependencyInjection để đăng ký toàn bộ InMemory implementation
```

Các repository đã có:

```txt
InMemoryUserRepository
InMemoryEmployerProfileRepository
InMemoryCandidateProfileRepository
InMemoryJobRepository
InMemoryJobApplicationRepository
InMemoryDisabilityTypeRepository
InMemoryAssistiveDeviceRepository
InMemoryJobCategoryRepository
```

Các service đã có:

```txt
InMemoryCurrentUserService
SimplePasswordHasher
SimpleTokenService
InMemoryDomainEventDispatcher
```

Bước tiếp theo khuyến nghị:

```txt
Làm EnableVN.Presentation tối thiểu với InMemory để test flow MVP.
```
