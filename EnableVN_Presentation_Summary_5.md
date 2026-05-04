# EnableVN_Presentation_Summary.md

Tài liệu này dùng để gửi lại cho ChatGPT ở phiên làm việc mới nhằm tiếp tục phát triển dự án EnableVN mà không bị mất ngữ cảnh.

---

# 1. Trạng thái hiện tại của dự án

Dự án EnableVN đang được xây dựng theo:

```txt
Hexagonal Architecture
Domain Driven Design
SOLID
Design Pattern nếu phù hợp
ASP.NET Core MVC cho Presentation
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
EnableVN.Domain                 ✅
EnableVN.Ports                  ✅
EnableVN.Application            ✅
EnableVN.InfrastructureInMemory ✅
EnableVN.Presentation           🟡 Đang làm, đã có MVC nền tảng và các flow chính
```

Chưa làm:

```txt
EnableVN.InfrastructureSqlite   ❌
Catalog Admin UI                ❌
Seed Admin                      ❌
```

---

# 2. Yêu cầu riêng của Presentation

Người dùng yêu cầu:

```txt
Presentation phải tuân thủ MVC
Không làm API thuần ở bước này
Giao diện đẹp, hiện đại, sạch
Responsive
Phù hợp với web hiện đại
Dùng Razor View
Dùng InMemory để test trước
```

Presentation dùng:

```txt
ASP.NET Core MVC
Controller
Razor View
Session
Bootstrap 5
CSS custom trong wwwroot/css/site.css
```

---

# 3. Vai trò của EnableVN.Presentation

`EnableVN.Presentation` là lớp giao diện MVC.

Nhiệm vụ:

```txt
Nhận HTTP request từ người dùng
Render Razor View
Nhận form submit
Gọi Inbound Port từ EnableVN.Ports
Hiển thị kết quả hoặc lỗi
Lưu thông tin đăng nhập tạm thời vào Session
```

Presentation không được:

```txt
Gọi Repository trực tiếp
Chứa business logic nặng
Tự xử lý rule nghiệp vụ đã có trong Domain/Application
Truy cập database trực tiếp
```

Presentation được phép:

```txt
Reference EnableVN.Ports
Reference EnableVN.Application để gọi AddEnableVNApplication()
Reference EnableVN.InfrastructureInMemory để gọi AddEnableVNInMemoryInfrastructure()
Implement ICurrentUserService bằng Session
```

---

# 4. Chiều gọi hiện tại

```txt
Browser
   ↓
MVC Controller
   ↓ gọi
Ports.Inbound UseCase Interface
   ↓ implement bởi
Application UseCase
   ↓ gọi
Ports.Outbound Repository/Service
   ↓ implement bởi
InfrastructureInMemory
```

Ví dụ:

```txt
JobsController
    -> IJobUseCase
        -> JobUseCase
            -> IJobRepository
                -> InMemoryJobRepository
```

---

# 5. Cấu trúc Presentation đã thiết kế

```txt
EnableVN.Presentation
│
├── Controllers
│   ├── HomeController.cs
│   ├── AuthController.cs
│   ├── JobsController.cs
│   ├── EmployerJobsController.cs
│   ├── EmployerProfileController.cs
│   ├── CandidateProfileController.cs
│   └── JobApplicationsController.cs
│
├── Models
│   └── ErrorViewModel.cs
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
│   ├── EmployerJobs
│   │   ├── Index.cshtml
│   │   └── Create.cshtml
│   │
│   ├── EmployerProfile
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── _EmployerProfileForm.cshtml
│   │
│   ├── CandidateProfile
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Disability.cshtml
│   │
│   └── JobApplications
│       ├── MyApplications.cshtml
│       └── ByJob.cshtml
│
├── wwwroot
│   └── css
│       └── site.css
│
├── Program.cs
├── _ViewImports.cshtml
└── _ViewStart.cshtml
```

---

# 6. Program.cs đã thiết kế

`Program.cs` cần có:

```txt
AddControllersWithViews()
AddSession()
AddHttpContextAccessor()
AddEnableVNApplication()
AddEnableVNInMemoryInfrastructure()
Override ICurrentUserService bằng SessionCurrentUserService
UseStaticFiles()
UseRouting()
UseSession()
MapControllerRoute()
```

Mẫu code quan trọng:

```csharp
using EnableVN.Application;
using EnableVN.InfrastructureInMemory;
using EnableVN.Ports.Outbound.Services;
using EnableVN.Presentation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddEnableVNApplication();
builder.Services.AddEnableVNInMemoryInfrastructure();

// Ghi đè ICurrentUserService của InMemory.
// Trong MVC, current user nên đọc từ Session.
builder.Services.AddScoped<ICurrentUserService, SessionCurrentUserService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
```

Lưu ý:

```txt
AddEnableVNInMemoryInfrastructure() đăng ký ICurrentUserService InMemory dạng Singleton.
Sau đó Presentation ghi đè bằng SessionCurrentUserService dạng Scoped.
```

---

# 7. SessionCurrentUserService

File:

```txt
EnableVN.Presentation/Services/SessionCurrentUserService.cs
```

Implement:

```txt
ICurrentUserService
```

Vai trò:

```txt
Đọc user hiện tại từ HttpContext.Session
Application chỉ biết ICurrentUserService
Application không biết HttpContext/Session
```

Session keys đang dùng:

```txt
UserId
UserEmail
UserRole
AccessToken
```

Thuộc tính:

```txt
Guid? UserId
UserRole? Role
bool IsAuthenticated
```

Lỗi đã gặp:

```txt
CS0246: SessionCurrentUserService could not be found
```

Cách fix:

```txt
Tạo file Services/SessionCurrentUserService.cs
Thêm using EnableVN.Presentation.Services trong Program.cs
Đảm bảo đã gọi builder.Services.AddHttpContextAccessor()
```

---

# 8. ErrorViewModel

File:

```txt
EnableVN.Presentation/Models/ErrorViewModel.cs
```

Vai trò:

```txt
ViewModel cho Views/Shared/Error.cshtml
```

Code đã đề xuất:

```csharp
namespace EnableVN.Presentation.Models;

public sealed class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
```

Lỗi đã gặp:

```txt
CS0246: ErrorViewModel could not be found
```

Cách fix:

```txt
Tạo Models/ErrorViewModel.cs
Sửa Views/Shared/Error.cshtml dùng đúng namespace/model
Sửa HomeController.Error() truyền ErrorViewModel
```

---

# 9. Layout và giao diện hiện đại

## 9.1. _Layout.cshtml

File:

```txt
Views/Shared/_Layout.cshtml
```

Vai trò:

```txt
Layout chung cho toàn bộ website
Navbar
Menu theo role
Hiển thị user hiện tại
Đăng nhập/Đăng ký/Đăng xuất
Render TempData Success/Error
Footer
Load Bootstrap và site.css
```

Menu theo role:

```txt
Guest:
- Việc làm
- Đăng nhập
- Đăng ký

Employer:
- Việc làm
- Doanh nghiệp
- Tin của tôi
- Đăng xuất

Candidate:
- Việc làm
- Hồ sơ của tôi
- Đã ứng tuyển
- Đăng xuất
```

Session đọc trong layout:

```txt
UserId
UserEmail
UserRole
```

---

## 9.2. site.css

File:

```txt
wwwroot/css/site.css
```

Style đã thiết kế:

```txt
Nền sáng hiện đại
Navbar blur/sticky
Card bo góc lớn
Shadow mềm
Button primary xanh
Hero section lớn
Badge mềm
Form control bo góc
Job card hover
Focus-visible accessibility
Responsive
```

Các class chính:

```txt
evn-navbar
evn-brand
evn-brand-mark
evn-user-chip
evn-btn-primary
evn-hero
evn-hero-title
evn-hero-text
evn-card
evn-card-soft
evn-section
evn-muted
evn-badge
evn-form
evn-job-card
evn-alert
evn-footer
```

Lưu ý accessibility:

```txt
Có :focus-visible rõ ràng
Không dùng drag/drop
Form dùng label
Button/link rõ ràng
```

---

# 10. View imports và View start

## _ViewImports.cshtml

Cần có:

```cshtml
@using EnableVN.Domain.Users
@using EnableVN.Domain.Jobs
@using EnableVN.Domain.Applications

@using EnableVN.Ports.Models.Auth
@using EnableVN.Ports.Models.Jobs
@using EnableVN.Ports.Models.Employers
@using EnableVN.Ports.Models.Candidates
@using EnableVN.Ports.Models.Applications
@using EnableVN.Ports.Models.Catalogs

@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

## _ViewStart.cshtml

```cshtml
@{
    Layout = "_Layout";
}
```

---

# 11. HomeController và Home View

## HomeController

File:

```txt
Controllers/HomeController.cs
```

Actions:

```txt
Index()
Error()
```

`Error()` nên trả `ErrorViewModel`.

## Views/Home/Index.cshtml

Trang chủ hiện đại với:

```txt
Hero section lớn
CTA Tìm việc ngay
CTA Tham gia EnableVN
Card giới thiệu:
- Hỗ trợ tiếp cận
- Quyền riêng tư
- Quản lý hồ sơ
```

---

# 12. AuthController

File:

```txt
Controllers/AuthController.cs
```

Dependencies:

```txt
IAuthUseCase
```

Actions:

```txt
GET  Register
POST Register
GET  Login
POST Login
POST Logout
```

## Register

Input model:

```txt
RegisterCommand
```

Luồng:

```txt
1. Nhận form đăng ký
2. Gọi IAuthUseCase.RegisterAsync
3. Nếu thành công:
   - lưu UserId, UserEmail, UserRole, AccessToken vào Session
   - TempData Success
   - Redirect Home
4. Nếu lỗi UseCaseException/DomainException:
   - TempData Error
   - return View(command)
```

Lưu ý:

```txt
AuthUseCase chặn public register Admin.
Register chỉ nên cho Candidate/Employer.
```

## Login

Input model:

```txt
LoginCommand
```

Luồng:

```txt
1. Gọi IAuthUseCase.LoginAsync
2. Nếu thành công lưu Session
3. Redirect Home
4. Nếu lỗi hiển thị Error
```

## Logout

Luồng:

```txt
HttpContext.Session.Clear()
Redirect Home
```

Views đã có:

```txt
Views/Auth/Register.cshtml
Views/Auth/Login.cshtml
```

---

# 13. JobsController

File:

```txt
Controllers/JobsController.cs
```

Dependencies:

```txt
IJobUseCase
IJobApplicationUseCase
```

Actions:

```txt
GET  Index
GET  Details
POST Apply
```

## Index

Input:

```txt
SearchJobQuery từ query string
```

Gọi:

```txt
IJobUseCase.SearchPublishedJobsAsync(query)
```

View:

```txt
Views/Jobs/Index.cshtml
```

Chức năng UI:

```txt
Tìm theo keyword
Lọc theo WorkMode
Lọc remote
Lọc hỗ trợ xe lăn
Lọc thời gian linh hoạt
Lọc thiết bị hỗ trợ
Hiển thị job card
```

## Details

Input:

```txt
Guid id
```

Gọi:

```txt
IJobUseCase.GetByIdAsync(id)
```

View:

```txt
Views/Jobs/Details.cshtml
```

Hiển thị:

```txt
Title
WorkMode
PublishedAt
Description
Requirement
AccessibilityInfo
Button nộp hồ sơ
```

## Apply

Input:

```txt
jobId
```

Gọi:

```txt
IJobApplicationUseCase.SubmitAsync(new SubmitJobApplicationCommand { JobId = jobId })
```

Lưu ý:

```txt
Candidate phải đăng nhập
Candidate phải có profile
Candidate phải có CV hoặc command.CvUrl
Job phải Published
Candidate chưa nộp job này
```

Nếu lỗi:

```txt
TempData["Error"] = ex.Message
Redirect Details
```

---

# 14. EmployerJobsController

File:

```txt
Controllers/EmployerJobsController.cs
```

Dependencies:

```txt
IJobUseCase
```

Actions:

```txt
GET  Index
GET  Create
POST Create
POST Publish
POST Close
```

## Index

Gọi:

```txt
IJobUseCase.GetMyJobsAsync()
```

View:

```txt
Views/EmployerJobs/Index.cshtml
```

Hiển thị:

```txt
Danh sách job của Employer hiện tại
Status Draft/Published/Closed/Deleted
Button tạo tin mới
Button publish nếu Draft
Button close nếu Published
Button xem chi tiết
Button hồ sơ ứng tuyển
```

Đã cập nhật thêm link:

```cshtml
<a asp-controller="JobApplications"
   asp-action="ByJob"
   asp-route-jobId="@job.Id">
    Hồ sơ ứng tuyển
</a>
```

## Create

Input:

```txt
CreateJobCommand
```

View:

```txt
Views/EmployerJobs/Create.cshtml
```

Form gồm:

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

## Publish

Gọi:

```txt
IJobUseCase.PublishAsync(id)
```

## Close

Gọi:

```txt
IJobUseCase.CloseAsync(id)
```

---

# 15. EmployerProfileController

File:

```txt
Controllers/EmployerProfileController.cs
```

Dependencies:

```txt
IEmployerProfileUseCase
```

Actions:

```txt
GET  Index
GET  Create
POST Create
GET  Edit
POST Edit
```

## Index

Gọi:

```txt
IEmployerProfileUseCase.GetMyProfileAsync()
```

Nếu chưa có profile:

```txt
Redirect Create
```

View:

```txt
Views/EmployerProfile/Index.cshtml
```

Hiển thị:

```txt
CompanyName
WebsiteUrl
Description
HasWheelchairAccess
HasAccessibleRestroom
SupportsFlexibleWorkingTime
SupportsRemoteWork
ProvidesAssistiveDevices
```

## Create

Input:

```txt
CreateEmployerProfileCommand
```

View:

```txt
Views/EmployerProfile/Create.cshtml
```

Dùng partial:

```txt
Views/EmployerProfile/_EmployerProfileForm.cshtml
```

## Edit

Input:

```txt
UpdateEmployerProfileCommand
```

View:

```txt
Views/EmployerProfile/Edit.cshtml
```

Dùng cùng partial form.

## _EmployerProfileForm.cshtml

Partial dùng dynamic model để tái sử dụng cho cả Create và Edit.

Fields:

```txt
CompanyName
WebsiteUrl
Description
HasWheelchairAccess
HasAccessibleRestroom
SupportsFlexibleWorkingTime
SupportsRemoteWork
ProvidesAssistiveDevices
```

Lưu ý:

```txt
Checkbox có hidden input value=false để model binding nhận false khi không check.
```

---

# 16. CandidateProfileController

File:

```txt
Controllers/CandidateProfileController.cs
```

Dependencies:

```txt
ICandidateProfileUseCase
```

Actions:

```txt
GET  Index
GET  Create
POST Create
GET  Edit
POST Edit
GET  Disability
POST Disability
POST HideDisabilityInfo
POST ShowDisabilityInfo
POST MakePublic
POST MakePrivate
```

## Index

Gọi:

```txt
ICandidateProfileUseCase.GetMyProfileAsync()
```

Nếu chưa có profile:

```txt
Redirect Create
```

View:

```txt
Views/CandidateProfile/Index.cshtml
```

Hiển thị:

```txt
FullName
Bio
CvUrl
IsPublicProfile
IsDisabilityInfoVisibleToEmployer
DisabilityDescription
```

Có action buttons:

```txt
Cập nhật hồ sơ
Thông tin khuyết tật
Public hồ sơ / Chuyển về riêng tư
Cho phép hiển thị / Ẩn khỏi nhà tuyển dụng
```

## Create

Input:

```txt
CreateCandidateProfileCommand
```

View:

```txt
Views/CandidateProfile/Create.cshtml
```

Fields:

```txt
FullName
CvUrl
Bio
```

Lưu ý:

```txt
Giai đoạn này dùng URL CV.
Upload file sẽ làm sau.
```

## Edit

Input:

```txt
UpdateCandidateProfileCommand
```

View:

```txt
Views/CandidateProfile/Edit.cshtml
```

Fields:

```txt
FullName
CvUrl
Bio
```

## Disability

Input:

```txt
UpdateDisabilityInfoCommand
```

View:

```txt
Views/CandidateProfile/Disability.cshtml
```

Fields:

```txt
DisabilityTypeId
Description
IsVisibleToEmployer
```

Lưu ý:

```txt
Hiện DisabilityTypeId tạm nhập Guid.
Sau khi làm Catalog UI thì nên đổi thành dropdown.
```

## Hide/Show Disability Info

Gọi:

```txt
HideMyDisabilityInfoAsync()
ShowMyDisabilityInfoAsync()
```

## Public/Private Profile

Gọi:

```txt
MakeMyProfilePublicAsync()
MakeMyProfilePrivateAsync()
```

---

# 17. JobApplicationsController

File:

```txt
Controllers/JobApplicationsController.cs
```

Dependencies:

```txt
IJobApplicationUseCase
```

Actions:

```txt
GET  MyApplications
GET  ByJob
POST ChangeStatus
POST Withdraw
```

## MyApplications

Candidate xem hồ sơ đã nộp.

Gọi:

```txt
IJobApplicationUseCase.GetMyApplicationsAsync()
```

View:

```txt
Views/JobApplications/MyApplications.cshtml
```

Hiển thị:

```txt
ApplicationId
JobId
Status
SubmittedAt
Button rút hồ sơ nếu còn được rút
```

Không cho rút nếu status:

```txt
Withdrawn
Accepted
Rejected
```

## ByJob

Employer xem hồ sơ ứng tuyển theo Job.

Input:

```txt
jobId
```

Gọi:

```txt
IJobApplicationUseCase.GetByJobIdAsync(jobId)
```

View:

```txt
Views/JobApplications/ByJob.cshtml
```

Hiển thị:

```txt
CandidateId
SubmittedAt
Status
CoverLetter
CvUrl
Form đổi trạng thái
```

## ChangeStatus

Input:

```txt
applicationId
jobId
newStatus
note
```

Gọi:

```txt
IJobApplicationUseCase.ChangeStatusAsync(
    new ChangeApplicationStatusCommand
    {
        ApplicationId = applicationId,
        NewStatus = newStatus,
        Note = note
    }
)
```

Các trạng thái cho Employer chọn:

```txt
Reviewing
Interview
Rejected
Accepted
```

Sau khi đổi:

```txt
Redirect ByJob(jobId)
```

## Withdraw

Candidate rút hồ sơ.

Input:

```txt
applicationId
```

Gọi:

```txt
IJobApplicationUseCase.WithdrawAsync(applicationId)
```

---

# 18. Error handling trong Controller

Các controller hiện bắt lỗi kiểu:

```csharp
catch (Exception ex) when (ex is UseCaseException or DomainException)
{
    TempData["Error"] = ex.Message;
    return View(command);
}
```

Hoặc redirect:

```csharp
TempData["Error"] = ex.Message;
return RedirectToAction(...);
```

Ý nghĩa:

```txt
UseCaseException = lỗi Application như chưa đăng nhập, sai quyền, không tìm thấy dữ liệu
DomainException = lỗi rule nghiệp vụ thuần trong Domain
```

---

# 19. Flow MVP hiện tại có thể test được

Sau các controller/view hiện tại, có thể test flow:

## Flow Employer

```txt
1. Đăng ký tài khoản Employer
2. Tạo hồ sơ doanh nghiệp
3. Vào Tin của tôi
4. Tạo tin tuyển dụng dạng Draft
5. Publish tin tuyển dụng
6. Xem danh sách hồ sơ ứng tuyển theo job
7. Đổi trạng thái hồ sơ ứng tuyển
```

## Flow Candidate

```txt
1. Đăng ký tài khoản Candidate
2. Tạo hồ sơ ứng viên
3. Cập nhật CV URL
4. Cập nhật thông tin khuyết tật
5. Ẩn/hiện thông tin khuyết tật
6. Public/private profile
7. Tìm job
8. Nộp hồ sơ
9. Xem hồ sơ đã nộp
10. Rút hồ sơ nếu còn được rút
```

## Flow Guest

```txt
1. Vào trang chủ
2. Xem danh sách việc làm Published
3. Xem chi tiết job
```

---

# 20. Những lỗi đã gặp và đã biết cách sửa

## 20.1. SessionCurrentUserService could not be found

Nguyên nhân:

```txt
Program.cs đăng ký SessionCurrentUserService nhưng chưa tạo class hoặc thiếu using.
```

Fix:

```txt
Tạo Services/SessionCurrentUserService.cs
Thêm using EnableVN.Presentation.Services;
Đảm bảo AddHttpContextAccessor()
```

## 20.2. ErrorViewModel could not be found

Nguyên nhân:

```txt
Views/Shared/Error.cshtml dùng ErrorViewModel nhưng chưa tạo model.
```

Fix:

```txt
Tạo Models/ErrorViewModel.cs
Sửa Error.cshtml dùng đúng model
Sửa HomeController.Error() truyền ErrorViewModel
```

## 20.3. ApplicationException ambiguous

Lỗi này ở Application trước đó.

Nguyên nhân:

```txt
EnableVN.Application.Common.ApplicationException trùng System.ApplicationException
```

Fix:

```txt
Đổi ApplicationException thành UseCaseException
Replace throw new ApplicationException(...) thành throw new UseCaseException(...)
```

---

# 21. Những phần còn thiếu trong Presentation

## 21.1. CatalogController cho Admin

Chưa làm UI cho Admin quản lý:

```txt
DisabilityType
AssistiveDevice
JobCategory
```

Cần thêm:

```txt
CatalogController
Views/Catalog/DisabilityTypes.cshtml
Views/Catalog/AssistiveDevices.cshtml
Views/Catalog/JobCategories.cshtml
Create/Edit form cho catalog
```

Hoặc làm chung một controller và view đơn giản.

## 21.2. Chưa có cách tạo Admin

Vì `AuthUseCase.RegisterAsync` chặn public register Admin.

Cần chọn một hướng:

```txt
Hướng A: Seed Admin trong InfrastructureInMemory
Hướng B: Tạo AdminBootstrapController chỉ dùng Development
Hướng C: Tạm cho phép tạo Admin bằng seed thủ công trong Program.cs
```

Khuyến nghị:

```txt
Seed Admin trong InfrastructureInMemory hoặc Presentation khi app start ở Development.
```

## 21.3. Candidate DisabilityTypeId còn nhập Guid thủ công

Hiện view:

```txt
Views/CandidateProfile/Disability.cshtml
```

đang cho nhập:

```txt
DisabilityTypeId
```

bằng textbox.

Sau khi có Catalog UI/seed, nên đổi thành dropdown:

```txt
<select asp-for="DisabilityTypeId">
    options từ ICatalogUseCase.GetActiveDisabilityTypesAsync()
</select>
```

## 21.4. Chưa có upload CV thật

Hiện Candidate dùng:

```txt
CvUrl
```

là URL text.

Sau này có thể dùng:

```txt
IFileStorageService
Upload file
Validate file size/type
```

## 21.5. Chưa có UI Update Job

Employer hiện có:

```txt
Create job
Publish job
Close job
```

Nhưng chưa có:

```txt
Edit job
Delete job
```

UseCase đã có:

```txt
IJobUseCase.UpdateAsync
IJobUseCase.DeleteAsync
```

Cần thêm UI sau.

## 21.6. Chưa có authentication/authorization middleware thật

Hiện tại dùng Session tự quản lý.

Chưa dùng:

```txt
ASP.NET Core Identity
Cookie Authentication
Authorize attribute
JWT
ClaimsPrincipal
```

MVP demo với Session là chấp nhận được.

---

# 22. Bước tiếp theo khuyến nghị

Nên làm tiếp theo thứ tự:

```txt
1. Seed Admin cho InMemory
2. CatalogController + Views cho Admin
3. Dropdown DisabilityType trong CandidateProfile/Disability
4. Edit/Delete Job UI
5. Tạo file markdown tóm tắt Presentation
6. Sau đó mới làm InfrastructureSqlite
```

Hiện tại bước 5 của danh sách này chính là file này.

Sau file này, bước tiếp theo nên là:

```txt
Seed Admin + CatalogController
```

---

# 23. Câu lệnh nên gửi trong phiên mới

Có thể gửi file này cho ChatGPT và nói:

```txt
Dựa trên file EnableVN_Presentation_Summary.md này, hãy tiếp tục làm phần Admin cho Presentation:
- Seed Admin trong InMemory hoặc Development
- CatalogController theo MVC
- Views quản lý DisabilityType, AssistiveDevice, JobCategory
- Giao diện đồng bộ với site.css hiện tại
- Controller chỉ gọi ICatalogUseCase
- Có comment giải thích trong code
```

Hoặc nếu muốn làm SQLite:

```txt
Dựa trên file EnableVN_Presentation_Summary.md này, hãy tiếp tục làm EnableVN.InfrastructureSqlite:
- EF Core SQLite
- Không để Domain phụ thuộc EF Core
- Implement các repository từ Ports
- Có PersistenceModel nếu cần
```

---

# 24. Tóm tắt ngắn gọn

`EnableVN.Presentation` hiện đã có:

```txt
MVC setup
Session current user
Layout hiện đại
CSS hiện đại
Home page
Register/Login/Logout
Job search
Job detail
Candidate apply job
Employer create/publish/close job
Employer profile create/edit/view
Candidate profile create/edit/view
Candidate disability privacy controls
Candidate my applications
Employer applications by job
Employer change application status
```

Còn thiếu chính:

```txt
Admin seed
Catalog admin UI
Job edit/delete UI
Dropdown catalog cho disability type
SQLite infrastructure
```

Trạng thái tổng thể:

```txt
Domain                 ✅
Ports                  ✅
Application            ✅
InfrastructureInMemory ✅
Presentation MVC       🟡 Gần đủ MVP, còn thiếu Admin/Catalog
InfrastructureSqlite   ❌
```
