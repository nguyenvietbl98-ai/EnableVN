# EnableVN Domain Summary

> **Đồng bộ code (bản cập nhật):** Cây thư mục §2 và các chỗ nhắc `JobPostingPolicy` đã chỉnh cho khớp repo hiện tại (thư mục project: `Domain/`). Chi tiết tổng thể: `EnableVN_Final_Project_Summary_8.md`.

## 1. Mục tiêu của Domain Layer

`EnableVN.Domain` là lõi nghiệp vụ của hệ thống EnableVN.

Domain Layer chịu trách nhiệm mô hình hóa các khái niệm nghiệp vụ chính như:

- Người dùng
- Nhà tuyển dụng
- Ứng viên
- Tin tuyển dụng
- Hồ sơ ứng tuyển
- Danh mục hệ thống
- Quy tắc nghiệp vụ
- Sự kiện nghiệp vụ

Domain **không phụ thuộc** vào database, API, UI, Entity Framework, SQLite, DTO hay bất kỳ công nghệ hạ tầng nào.

Điều này phù hợp với:

- Hexagonal Architecture
- Domain Driven Design
- SOLID
- Clean Architecture
- Separation of Concerns

---

## 2. Cấu trúc Domain hiện tại

```txt
EnableVN.Domain   (thư mục: Domain/)
│
├── Common
│   ├── Entity.cs
│   ├── AggregateRoot.cs
│   ├── ValueObject.cs
│   ├── IDomainEvent.cs
│   └── DomainException.cs
│
├── Users
│   ├── User.cs
│   ├── Email.cs
│   ├── UserRole.cs
│   ├── UserStatus.cs
│   └── Events
│       └── UserRegisteredEvent.cs
│
├── Employers
│   ├── EmployerProfile.cs
│   ├── CompanyName.cs
│   ├── InclusiveWorkplaceInfo.cs
│   └── Events
│       └── EmployerProfileCreatedEvent.cs
│
├── Candidates
│   ├── CandidateProfile.cs
│   ├── FullName.cs
│   ├── DisabilityInfo.cs
│   └── Events
│       └── CandidateProfileCreatedEvent.cs
│
├── Jobs
│   ├── JobPost.cs
│   ├── JobTitle.cs
│   ├── SalaryRange.cs
│   ├── JobAccessibilityInfo.cs
│   ├── WorkMode.cs
│   ├── JobStatus.cs
│   └── Events
│       ├── JobPostedEvent.cs
│       └── JobClosedEvent.cs
│
├── Applications
│   ├── JobApplication.cs
│   ├── ApplicationStatus.cs
│   ├── ApplicationStatusHistory.cs
│   ├── Policies
│   │   └── ApplicationStatusPolicy.cs
│   └── Events
│       ├── JobApplicationSubmittedEvent.cs
│       └── JobApplicationStatusChangedEvent.cs
│
├── Catalogs
│   ├── DisabilityType.cs
│   ├── AssistiveDevice.cs
│   ├── JobCategory.cs
│   └── CatalogStatus.cs
│
├── Notifications
│   ├── Notification.cs
│   ├── NotificationType.cs
│   └── NotificationStatus.cs
│
├── Reviews
│   └── CompanyReview.cs
│
└── Reports
    ├── ViolationReport.cs
    ├── ReportStatus.cs
    └── ReportTargetType.cs
```

**Ghi chú (đồng bộ code):** Trước đây tài liệu có `Jobs/Policies/JobPostingPolicy.cs` — **không còn** trong repo; quy tắc đăng tin nằm trong `JobPost` và tầng Application/UseCase.

---

# 3. Common

## 3.1. Entity

### Mục đích

`Entity<TId>` là lớp cơ sở cho các đối tượng có định danh riêng.

Ví dụ:

- User
- EmployerProfile
- CandidateProfile
- JobPost
- JobApplication
- DisabilityType
- AssistiveDevice

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Id | TId | Định danh duy nhất của Entity |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Entity Pattern | Đại diện cho đối tượng có danh tính riêng |
| Encapsulation | Cho phép `Id` được set trong nội bộ Domain |
| DDD Tactical Pattern | Là nền tảng để xây dựng Aggregate |

---

## 3.2. AggregateRoot

### Mục đích

`AggregateRoot<TId>` là lớp cơ sở cho các Aggregate Root.

Aggregate Root là điểm vào chính để thao tác với một cụm nghiệp vụ.

Ví dụ:

- User
- EmployerProfile
- CandidateProfile
- JobPost
- JobApplication

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| DomainEvents | IReadOnlyCollection<IDomainEvent> | Danh sách sự kiện nghiệp vụ phát sinh trong Aggregate |

### Hàm chính

| Hàm | Ý nghĩa |
|---|---|
| AddDomainEvent | Thêm một Domain Event |
| ClearDomainEvents | Xóa danh sách event sau khi Application xử lý |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Aggregate Root Pattern | Kiểm soát toàn bộ thay đổi nghiệp vụ qua một điểm |
| Domain Event Pattern | Ghi nhận các sự kiện quan trọng |
| Encapsulation | Danh sách event không cho bên ngoài sửa trực tiếp |

---

## 3.3. IDomainEvent

### Mục đích

`IDomainEvent` là interface chung cho tất cả sự kiện nghiệp vụ.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| OccurredOn | DateTime | Thời điểm sự kiện xảy ra |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Domain Event Pattern | Tách sự kiện khỏi logic xử lý phụ |
| Interface Segregation | Event chỉ cần hợp đồng tối thiểu |
| Open/Closed Principle | Có thể thêm event mới mà không sửa event cũ |

---

## 3.4. DomainException

### Mục đích

`DomainException` dùng để báo lỗi khi vi phạm quy tắc nghiệp vụ.

Ví dụ:

- Email không hợp lệ
- Job đã đóng nhưng ứng viên vẫn nộp hồ sơ
- Không thể đổi trạng thái hồ sơ đã bị từ chối
- Không thể publish tin đã bị xóa

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Fail Fast | Phát hiện lỗi nghiệp vụ ngay tại Domain |
| Domain Validation | Validation nghiệp vụ không bị đẩy xuống UI hoặc Database |
| Single Responsibility | Lỗi nghiệp vụ được biểu diễn bằng exception riêng |

---

# 4. Users Domain

## 4.1. User

### Mục đích

`User` đại diện cho tài khoản đăng nhập trong hệ thống.

Một User có thể thuộc một trong ba vai trò:

- Admin
- Employer
- Candidate

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Id | Guid | Định danh người dùng |
| Email | Email | Email đăng nhập, được kiểm tra bằng Value Object |
| PasswordHash | string | Mật khẩu đã mã hóa |
| Role | UserRole | Vai trò của người dùng |
| Status | UserStatus | Trạng thái tài khoản |

### Hàm chính

| Hàm | Ý nghĩa |
|---|---|
| Register | Tạo tài khoản mới |
| Lock | Khóa tài khoản |
| Activate | Kích hoạt lại tài khoản |
| Delete | Xóa mềm tài khoản |

### Domain Event phát sinh

| Event | Khi nào phát sinh |
|---|---|
| UserRegisteredEvent | Khi đăng ký user thành công |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Aggregate Root | User là gốc nghiệp vụ tài khoản |
| Factory Method | Dùng `Register()` để tạo User hợp lệ |
| Value Object | Email không dùng string thuần |
| Encapsulation | Không cho sửa Role, Status tùy tiện |
| Domain Event | Ghi nhận sự kiện user được tạo |

---

## 4.2. Email

### Mục đích

`Email` là Value Object dùng để đảm bảo email luôn hợp lệ.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Value | string | Giá trị email đã được chuẩn hóa |

### Quy tắc nghiệp vụ

- Không được rỗng
- Tự động trim
- Tự động lowercase
- Phải đúng định dạng email cơ bản

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Value Object Pattern | Gom rule email vào một object riêng |
| Factory Method | Dùng `Email.Create()` để tạo email hợp lệ |
| Encapsulation | Không cho tạo email sai từ constructor public |

---

## 4.3. UserRole

### Mục đích

Enum xác định vai trò người dùng.

### Giá trị

| Giá trị | Ý nghĩa |
|---|---|
| Admin | Quản trị hệ thống |
| Employer | Nhà tuyển dụng |
| Candidate | Ứng viên |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Type Safety | Tránh dùng string như `"Admin"`, `"Employer"` |
| Ubiquitous Language | Thể hiện đúng ngôn ngữ nghiệp vụ |

---

## 4.4. UserStatus

### Mục đích

Enum xác định trạng thái tài khoản.

### Giá trị

| Giá trị | Ý nghĩa |
|---|---|
| Active | Đang hoạt động |
| Locked | Bị khóa |
| Deleted | Đã xóa mềm |

---

## 4.5. UserRegisteredEvent

### Mục đích

Ghi nhận việc một User mới được đăng ký.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| UserId | Guid | Id của user vừa đăng ký |
| Role | UserRole | Vai trò của user |
| OccurredOn | DateTime | Thời điểm phát sinh event |

### Có cần xử lý ngay không?

Hiện tại MVP chưa bắt buộc xử lý event này.

Sau này có thể dùng để:

- Gửi email chào mừng
- Ghi audit log
- Tạo notification
- Đồng bộ sang hệ thống khác

---

# 5. Employers Domain

## 5.1. EmployerProfile

### Mục đích

`EmployerProfile` đại diện cho hồ sơ doanh nghiệp của nhà tuyển dụng.

Đây là phần phục vụ tính năng:

- Tạo hồ sơ doanh nghiệp
- Cập nhật thông tin doanh nghiệp
- Cập nhật thông tin môi trường làm việc bao trùm

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Id | Guid | Định danh hồ sơ doanh nghiệp |
| UserId | Guid | User sở hữu hồ sơ này |
| CompanyName | CompanyName | Tên doanh nghiệp |
| Description | string? | Mô tả doanh nghiệp |
| WebsiteUrl | string? | Website doanh nghiệp |
| WorkplaceInfo | InclusiveWorkplaceInfo | Thông tin môi trường làm việc tiếp cận |

### Hàm chính

| Hàm | Ý nghĩa |
|---|---|
| Create | Tạo hồ sơ doanh nghiệp |
| UpdateCompanyInfo | Cập nhật tên, mô tả, website |
| UpdateInclusiveWorkplaceInfo | Cập nhật thông tin tiếp cận nơi làm việc |

### Domain Event phát sinh

| Event | Khi nào phát sinh |
|---|---|
| EmployerProfileCreatedEvent | Khi tạo hồ sơ doanh nghiệp thành công |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Aggregate Root | EmployerProfile là gốc nghiệp vụ hồ sơ doanh nghiệp |
| Value Object | CompanyName, InclusiveWorkplaceInfo |
| Factory Method | Dùng `Create()` để tạo object hợp lệ |
| Encapsulation | Không cho sửa trực tiếp thuộc tính |
| Domain Event | Ghi nhận doanh nghiệp đã tạo profile |

---

## 5.2. CompanyName

### Mục đích

Value Object đại diện cho tên doanh nghiệp.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Value | string | Tên doanh nghiệp đã được chuẩn hóa |

### Quy tắc nghiệp vụ

- Không được rỗng
- Tối thiểu 2 ký tự
- Tối đa 200 ký tự
- Tự động trim

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Value Object | Bảo vệ rule tên doanh nghiệp |
| Factory Method | Tạo qua `CompanyName.Create()` |
| Fail Fast | Báo lỗi ngay nếu dữ liệu sai |

---

## 5.3. InclusiveWorkplaceInfo

### Mục đích

Mô tả mức độ hỗ trợ tiếp cận của doanh nghiệp.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| HasWheelchairAccess | bool | Có hỗ trợ xe lăn hay không |
| HasAccessibleRestroom | bool | Có nhà vệ sinh tiếp cận hay không |
| SupportsFlexibleWorkingTime | bool | Có hỗ trợ thời gian linh hoạt hay không |
| SupportsRemoteWork | bool | Có hỗ trợ làm việc từ xa hay không |
| ProvidesAssistiveDevices | bool | Có cung cấp thiết bị hỗ trợ hay không |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Value Object | Gom nhóm thông tin tiếp cận thành một object |
| Encapsulation | Thuộc tính chỉ được set trong object |
| Ubiquitous Language | Tên thuộc tính phản ánh đúng nghiệp vụ EnableVN |

---

## 5.4. EmployerProfileCreatedEvent

### Mục đích

Ghi nhận việc hồ sơ doanh nghiệp được tạo.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| EmployerProfileId | Guid | Id hồ sơ doanh nghiệp |
| UserId | Guid | Id user sở hữu hồ sơ |
| OccurredOn | DateTime | Thời điểm phát sinh event |

---

# 6. Candidates Domain

## 6.1. CandidateProfile

### Mục đích

`CandidateProfile` đại diện cho hồ sơ ứng viên.

Phục vụ các tính năng:

- Tạo hồ sơ ứng viên
- Cập nhật CV
- Ẩn hoặc hiện thông tin khuyết tật
- Public hoặc private profile

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Id | Guid | Định danh hồ sơ ứng viên |
| UserId | Guid | User sở hữu hồ sơ này |
| FullName | FullName | Họ tên ứng viên |
| Bio | string? | Giới thiệu bản thân |
| CvUrl | string? | Đường dẫn CV |
| DisabilityInfo | DisabilityInfo | Thông tin khuyết tật và quyền hiển thị |
| IsPublicProfile | bool | Cho phép nhà tuyển dụng tìm thấy profile hay không |

### Hàm chính

| Hàm | Ý nghĩa |
|---|---|
| Create | Tạo hồ sơ ứng viên |
| UpdateBasicInfo | Cập nhật họ tên, bio, CV |
| UpdateDisabilityInfo | Cập nhật thông tin khuyết tật |
| HideDisabilityInfo | Ẩn thông tin khuyết tật |
| ShowDisabilityInfoToEmployer | Cho phép Employer xem thông tin khuyết tật |
| MakeProfilePublic | Cho phép Employer tìm kiếm profile |
| MakeProfilePrivate | Ẩn profile khỏi danh sách tìm kiếm |

### Domain Event phát sinh

| Event | Khi nào phát sinh |
|---|---|
| CandidateProfileCreatedEvent | Khi tạo hồ sơ ứng viên thành công |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Aggregate Root | CandidateProfile là gốc nghiệp vụ hồ sơ ứng viên |
| Value Object | FullName, DisabilityInfo |
| Privacy by Design | Quyền riêng tư về khuyết tật nằm trong Domain |
| Factory Method | Tạo profile qua `Create()` |
| Encapsulation | Không cho UI tự sửa trạng thái riêng tư |
| Domain Event | Ghi nhận profile được tạo |

---

## 6.2. FullName

### Mục đích

Value Object đại diện cho họ tên ứng viên.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Value | string | Họ tên đã chuẩn hóa |

### Quy tắc nghiệp vụ

- Không được rỗng
- Tối thiểu 2 ký tự
- Tối đa 100 ký tự
- Tự động trim

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Value Object | Không dùng string thuần cho họ tên |
| Factory Method | Tạo qua `FullName.Create()` |
| Fail Fast | Dữ liệu sai thì báo lỗi ngay |

---

## 6.3. DisabilityInfo

### Mục đích

Quản lý thông tin khuyết tật và quyền hiển thị của ứng viên.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| DisabilityTypeId | Guid? | Loại khuyết tật, liên kết đến Catalog |
| Description | string? | Mô tả thêm nếu cần |
| IsVisibleToEmployer | bool | Employer có được xem thông tin này hay không |

### Hàm chính

| Hàm | Ý nghĩa |
|---|---|
| Hidden | Tạo trạng thái mặc định là ẩn |
| Create | Tạo thông tin khuyết tật |
| Hide | Ẩn thông tin khỏi Employer |
| Show | Cho phép Employer xem |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Value Object | Gom thông tin riêng tư thành object rõ nghĩa |
| Privacy by Design | Mặc định ẩn thông tin khuyết tật |
| Encapsulation | Việc ẩn/hiện được kiểm soát bằng hàm |
| Domain Rule | Quyền riêng tư không phụ thuộc UI |

---

## 6.4. CandidateProfileCreatedEvent

### Mục đích

Ghi nhận việc hồ sơ ứng viên được tạo.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| CandidateProfileId | Guid | Id hồ sơ ứng viên |
| UserId | Guid | Id user sở hữu hồ sơ |
| OccurredOn | DateTime | Thời điểm phát sinh event |

---

# 7. Jobs Domain

## 7.1. JobPost

### Mục đích

`JobPost` đại diện cho tin tuyển dụng.

Phục vụ các tính năng:

- Employer tạo tin tuyển dụng
- Employer chỉnh sửa tin
- Employer đăng tin
- Employer đóng tin
- Candidate lọc việc theo khả năng tiếp cận

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Id | Guid | Định danh tin tuyển dụng |
| EmployerId | Guid | Doanh nghiệp sở hữu tin |
| Title | JobTitle | Tiêu đề công việc |
| Description | string | Mô tả công việc |
| Requirement | string | Yêu cầu công việc |
| WorkMode | WorkMode | Hình thức làm việc |
| SalaryRange | SalaryRange | Khoảng lương |
| AccessibilityInfo | JobAccessibilityInfo | Thông tin hỗ trợ tiếp cận |
| Status | JobStatus | Trạng thái tin tuyển dụng |
| CreatedAt | DateTime | Thời điểm tạo tin |
| PublishedAt | DateTime? | Thời điểm đăng tin |
| ClosedAt | DateTime? | Thời điểm đóng tin |

### Hàm chính

| Hàm | Ý nghĩa |
|---|---|
| CreateDraft | Tạo tin ở trạng thái nháp |
| UpdateContent | Cập nhật nội dung tin |
| Publish | Đăng tin |
| Close | Đóng tin |
| Delete | Xóa mềm tin |
| CanReceiveApplication | Kiểm tra tin có nhận hồ sơ được không |

### Domain Event phát sinh

| Event | Khi nào phát sinh |
|---|---|
| JobPostedEvent | Khi tin được publish |
| JobClosedEvent | Khi tin được close |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Aggregate Root | JobPost là gốc nghiệp vụ của tin tuyển dụng |
| Factory Method | Tạo tin qua `CreateDraft()` |
| Value Object | JobTitle, SalaryRange, JobAccessibilityInfo |
| State Management | Quản lý trạng thái Draft, Published, Closed, Deleted |
| Domain Event | Ghi nhận đăng tin và đóng tin |
| Encapsulation | Không sửa trực tiếp trạng thái từ bên ngoài |
| Business Rule | Chỉ job Published mới được nhận hồ sơ |

---

## 7.2. JobTitle

### Mục đích

Value Object đại diện cho tiêu đề công việc.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Value | string | Tiêu đề công việc đã chuẩn hóa |

### Quy tắc nghiệp vụ

- Không được rỗng
- Tối thiểu 5 ký tự
- Tối đa 200 ký tự
- Tự động trim

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Value Object | Tránh dùng string thuần |
| Factory Method | Tạo qua `JobTitle.Create()` |
| Fail Fast | Tiêu đề sai thì báo lỗi ngay |

---

## 7.3. SalaryRange

### Mục đích

Value Object đại diện cho khoảng lương.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| MinSalary | decimal? | Lương tối thiểu |
| MaxSalary | decimal? | Lương tối đa |

### Quy tắc nghiệp vụ

- Lương tối thiểu không được âm
- Lương tối đa không được âm
- Lương tối thiểu không được lớn hơn lương tối đa
- Có thể để null nếu không công khai lương

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Value Object | Biểu diễn khoảng lương có rule riêng |
| Factory Method | Tạo qua `SalaryRange.Create()` |
| Invariant Protection | Luôn giữ khoảng lương hợp lệ |

---

## 7.4. JobAccessibilityInfo

### Mục đích

Mô tả khả năng hỗ trợ tiếp cận của một tin tuyển dụng cụ thể.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| SupportsWheelchairAccess | bool | Công việc/nơi làm có hỗ trợ xe lăn |
| SupportsRemoteWork | bool | Công việc có thể làm từ xa |
| SupportsFlexibleTime | bool | Có hỗ trợ thời gian linh hoạt |
| ProvidesAssistiveDevices | bool | Có cung cấp thiết bị hỗ trợ |
| AdditionalSupportDescription | string? | Mô tả thêm về hỗ trợ tiếp cận |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Value Object | Gom tiêu chí accessibility vào một object |
| Ubiquitous Language | Phản ánh đúng tiêu chí lọc việc của EnableVN |
| MVP Scope Control | Gộp các yêu cầu 5, 6, 7, 8 vào Job |

---

## 7.5. WorkMode

### Mục đích

Enum xác định hình thức làm việc.

### Giá trị

| Giá trị | Ý nghĩa |
|---|---|
| Onsite | Làm tại văn phòng |
| Remote | Làm từ xa |
| Hybrid | Kết hợp văn phòng và từ xa |

---

## 7.6. JobStatus

### Mục đích

Enum xác định trạng thái của tin tuyển dụng.

### Giá trị

| Giá trị | Ý nghĩa |
|---|---|
| Draft | Tin nháp |
| Published | Đã đăng |
| Closed | Đã đóng |
| Deleted | Đã xóa mềm |

---

## 7.7. JobPostingPolicy (lịch sử — không còn file riêng trong repo)

**Trạng thái code hiện tại:** Không còn class `JobPostingPolicy` trong `Domain/Jobs/Policies/`. Các quy tắc đăng tin / publish / đóng tin được gói trong **`JobPost`** (và use case `JobUseCase` ở Application).

Nội dung dưới đây giữ như **mô tả rule nghiệp vụ** (vẫn hữu ích khi đọc `JobPost`):

- Không được publish tin đã bị xóa
- Tin phải có mô tả
- Tin phải có yêu cầu công việc

---

## 7.8. JobPostedEvent

### Mục đích

Ghi nhận việc tin tuyển dụng được đăng.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| JobId | Guid | Id tin tuyển dụng |
| EmployerId | Guid | Id nhà tuyển dụng |
| OccurredOn | DateTime | Thời điểm phát sinh event |

### Có thể dùng sau này để

- Gửi notification cho candidate phù hợp
- Ghi audit log
- Index job vào search engine
- Kích hoạt matching job

---

## 7.9. JobClosedEvent

### Mục đích

Ghi nhận việc tin tuyển dụng được đóng.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| JobId | Guid | Id tin tuyển dụng |
| EmployerId | Guid | Id nhà tuyển dụng |
| OccurredOn | DateTime | Thời điểm phát sinh event |

---

# 8. Applications Domain

## 8.1. JobApplication

### Mục đích

`JobApplication` đại diện cho hồ sơ ứng tuyển của Candidate vào một Job.

Phục vụ tính năng:

- Candidate nộp hồ sơ
- Employer đổi trạng thái hồ sơ
- Lưu lịch sử trạng thái
- Candidate rút hồ sơ

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Id | Guid | Định danh hồ sơ ứng tuyển |
| JobId | Guid | Tin tuyển dụng được ứng tuyển |
| CandidateId | Guid | Ứng viên nộp hồ sơ |
| CoverLetter | string? | Thư ứng tuyển |
| CvUrl | string? | Đường dẫn CV dùng khi nộp |
| Status | ApplicationStatus | Trạng thái hiện tại của hồ sơ |
| SubmittedAt | DateTime | Thời điểm nộp hồ sơ |
| StatusHistories | IReadOnlyCollection<ApplicationStatusHistory> | Lịch sử thay đổi trạng thái |

### Hàm chính

| Hàm | Ý nghĩa |
|---|---|
| Submit | Tạo hồ sơ ứng tuyển mới |
| ChangeStatus | Employer đổi trạng thái hồ sơ |
| Withdraw | Candidate rút hồ sơ |

### Domain Event phát sinh

| Event | Khi nào phát sinh |
|---|---|
| JobApplicationSubmittedEvent | Khi ứng viên nộp hồ sơ |
| JobApplicationStatusChangedEvent | Khi trạng thái hồ sơ thay đổi |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Aggregate Root | JobApplication là gốc nghiệp vụ ứng tuyển |
| Factory Method | Tạo hồ sơ qua `Submit()` |
| Policy Pattern | Rule đổi trạng thái nằm trong `ApplicationStatusPolicy` |
| Domain Event | Ghi nhận submit và đổi trạng thái |
| Audit Trail | Lưu lịch sử trạng thái |
| Encapsulation | Không cho sửa status trực tiếp |
| Invariant Protection | Hồ sơ luôn có trạng thái hợp lệ |

---

## 8.2. ApplicationStatus

### Mục đích

Enum xác định trạng thái của hồ sơ ứng tuyển.

### Giá trị

| Giá trị | Ý nghĩa |
|---|---|
| Pending | Đang chờ xử lý |
| Reviewing | Đang xem xét |
| Interview | Được mời phỏng vấn |
| Rejected | Bị từ chối |
| Accepted | Được chấp nhận |
| Withdrawn | Ứng viên đã rút hồ sơ |

---

## 8.3. ApplicationStatusHistory

### Mục đích

Lưu lại lịch sử thay đổi trạng thái hồ sơ ứng tuyển.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Status | ApplicationStatus | Trạng thái tại thời điểm thay đổi |
| Note | string? | Ghi chú khi đổi trạng thái |
| ChangedAt | DateTime | Thời điểm đổi trạng thái |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Audit Trail | Lưu vết thay đổi trạng thái |
| Value Object-like Model | Không có định danh riêng, chỉ mô tả một mốc lịch sử |
| Encapsulation | Tạo lịch sử thông qua factory method |

---

## 8.4. ApplicationStatusPolicy

### Mục đích

Chứa rule chuyển trạng thái hồ sơ ứng tuyển.

### Rule hiện tại

- Không đổi trạng thái nếu hồ sơ đã Withdrawn
- Không đổi trạng thái nếu hồ sơ đã Rejected
- Không đổi trạng thái nếu hồ sơ đã Accepted
- Không đổi sang chính trạng thái hiện tại

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Policy Pattern | Tách rule trạng thái ra khỏi Entity |
| Single Responsibility | JobApplication không phải chứa toàn bộ logic điều kiện |
| Open/Closed Principle | Dễ mở rộng rule khi workflow phức tạp hơn |

---

## 8.5. JobApplicationSubmittedEvent

### Mục đích

Ghi nhận việc ứng viên nộp hồ sơ.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| ApplicationId | Guid | Id hồ sơ ứng tuyển |
| JobId | Guid | Id tin tuyển dụng |
| CandidateId | Guid | Id ứng viên |
| OccurredOn | DateTime | Thời điểm phát sinh event |

### Có thể dùng sau này để

- Gửi email cho Employer
- Tạo notification cho Employer
- Ghi audit log
- Cập nhật thống kê số lượt ứng tuyển

---

## 8.6. JobApplicationStatusChangedEvent

### Mục đích

Ghi nhận việc trạng thái hồ sơ ứng tuyển thay đổi.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| ApplicationId | Guid | Id hồ sơ ứng tuyển |
| JobId | Guid | Id tin tuyển dụng |
| CandidateId | Guid | Id ứng viên |
| OldStatus | ApplicationStatus | Trạng thái cũ |
| NewStatus | ApplicationStatus | Trạng thái mới |
| OccurredOn | DateTime | Thời điểm phát sinh event |

### Có thể dùng sau này để

- Gửi email cho Candidate
- Gửi notification khi được mời phỏng vấn
- Ghi lịch sử audit
- Tạo báo cáo tuyển dụng

---

# 9. Catalogs Domain

## 9.1. DisabilityType

### Mục đích

Danh mục loại khuyết tật do Admin quản lý.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Id | Guid | Định danh loại khuyết tật |
| Name | string | Tên loại khuyết tật |
| Description | string? | Mô tả |
| Status | CatalogStatus | Trạng thái danh mục |

### Hàm chính

| Hàm | Ý nghĩa |
|---|---|
| Create | Tạo loại khuyết tật |
| Update | Cập nhật tên, mô tả |
| Activate | Kích hoạt |
| Deactivate | Tạm ẩn/khóa |

### Kỹ thuật áp dụng

| Kỹ thuật | Ý nghĩa |
|---|---|
| Aggregate Root | Catalog item có vòng đời riêng |
| Factory Method | Tạo danh mục qua `Create()` |
| Soft State | Dùng Active/Inactive thay vì xóa cứng |
| Encapsulation | Không cho sửa trạng thái tùy tiện |

---

## 9.2. AssistiveDevice

### Mục đích

Danh mục thiết bị hỗ trợ do Admin quản lý.

### Thuộc tính

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Id | Guid | Định danh thiết bị hỗ trợ |
| Name | string | Tên thiết bị |
| Description | string? | Mô tả |
| Status | CatalogStatus | Trạng thái danh mục |

### Hàm chính

| Hàm | Ý nghĩa |
|---|---|
| Create | Tạo thiết bị hỗ trợ |
| Update | Cập nhật tên, mô tả |
| Activate | Kích hoạt |
| Deactivate | Tạm ẩn/khóa |

---

## 9.3. JobCategory

### Mục đích

Danh mục ngành nghề hoặc nhóm công việc.

### Thuộc tính đề xuất

| Thuộc tính | Kiểu dữ liệu | Ý nghĩa |
|---|---|---|
| Id | Guid | Định danh ngành nghề |
| Name | string | Tên ngành nghề |
| Description | string? | Mô tả |
| Status | CatalogStatus | Trạng thái danh mục |

---

## 9.4. CatalogStatus

### Mục đích

Enum xác định trạng thái của dữ liệu danh mục.

### Giá trị

| Giá trị | Ý nghĩa |
|---|---|
| Active | Đang sử dụng |
| Inactive | Tạm ngưng sử dụng |

---

# 10. Các kỹ thuật DDD đã áp dụng

## 10.1. Entity

Áp dụng cho:

- User
- EmployerProfile
- CandidateProfile
- JobPost
- JobApplication
- DisabilityType
- AssistiveDevice
- JobCategory

Ý nghĩa:

Entity là đối tượng có định danh riêng, có vòng đời và có thể thay đổi dữ liệu theo thời gian.

---

## 10.2. Value Object

Áp dụng cho:

- Email
- CompanyName
- FullName
- JobTitle
- SalaryRange
- JobAccessibilityInfo
- InclusiveWorkplaceInfo
- DisabilityInfo

Ý nghĩa:

Value Object giúp gom rule dữ liệu vào đúng chỗ, tránh dùng primitive type quá nhiều.

Ví dụ không nên dùng:

```csharp
public string Email { get; set; }
```

Nên dùng:

```csharp
public Email Email { get; private set; }
```

---

## 10.3. Aggregate Root

Áp dụng cho:

- User
- EmployerProfile
- CandidateProfile
- JobPost
- JobApplication

Ý nghĩa:

Aggregate Root là cổng kiểm soát thay đổi của nghiệp vụ.

Ví dụ:

- Muốn publish job thì gọi `JobPost.Publish()`
- Muốn đổi trạng thái hồ sơ thì gọi `JobApplication.ChangeStatus()`
- Muốn ẩn thông tin khuyết tật thì gọi `CandidateProfile.HideDisabilityInfo()`

---

## 10.4. Domain Event

Áp dụng cho:

- UserRegisteredEvent
- EmployerProfileCreatedEvent
- CandidateProfileCreatedEvent
- JobPostedEvent
- JobClosedEvent
- JobApplicationSubmittedEvent
- JobApplicationStatusChangedEvent

Ý nghĩa:

Domain Event ghi nhận sự kiện nghiệp vụ đã xảy ra.

Event giúp tách logic phụ khỏi Entity.

Ví dụ:

Khi đổi trạng thái hồ sơ, Domain chỉ tạo event:

```txt
JobApplicationStatusChangedEvent
```

Còn việc gửi email hoặc notification sẽ do Application Layer xử lý sau.

---

## 10.5. Policy Pattern

Áp dụng cho:

- ApplicationStatusPolicy
- (Đăng tin) rule nằm trong `JobPost` — không còn file `JobPostingPolicy.cs` riêng

Ý nghĩa:

Policy Pattern giúp tách các rule phức tạp ra khỏi Entity.

Ví dụ:

```txt
Không đổi trạng thái hồ sơ đã bị từ chối.
Không đổi trạng thái hồ sơ đã được chấp nhận.
Không publish tin đã bị xóa.
```

---

## 10.6. Factory Method

Áp dụng cho:

- User.Register()
- EmployerProfile.Create()
- CandidateProfile.Create()
- JobPost.CreateDraft()
- JobApplication.Submit()
- Email.Create()
- SalaryRange.Create()
- JobTitle.Create()
- FullName.Create()

Ý nghĩa:

Không cho tạo object bằng constructor public. Object phải được tạo qua hàm factory để đảm bảo luôn hợp lệ.

---

## 10.7. Encapsulation

Áp dụng gần như toàn bộ Domain.

Ví dụ:

```csharp
public ApplicationStatus Status { get; private set; }
```

Bên ngoài không thể tự ý sửa:

```csharp
application.Status = ApplicationStatus.Accepted;
```

Mà phải gọi:

```csharp
application.ChangeStatus(ApplicationStatus.Accepted, "Phù hợp với vị trí.");
```

Ý nghĩa:

Mọi thay đổi quan trọng đều đi qua rule nghiệp vụ.

---

# 11. Các nguyên lý SOLID đã áp dụng

## 11.1. Single Responsibility Principle

Mỗi class có một trách nhiệm rõ ràng.

Ví dụ:

| Class | Trách nhiệm |
|---|---|
| Email | Kiểm tra và biểu diễn email |
| SalaryRange | Kiểm tra và biểu diễn khoảng lương |
| JobPost | Quản lý nghiệp vụ tin tuyển dụng |
| JobApplication | Quản lý nghiệp vụ ứng tuyển |
| ApplicationStatusPolicy | Kiểm tra rule đổi trạng thái |

---

## 11.2. Open/Closed Principle

Có thể mở rộng mà ít phải sửa code cũ.

Ví dụ:

- Thêm event mới không cần sửa `IDomainEvent`
- Thêm policy mới không cần sửa Entity cũ quá nhiều
- Thêm status mới có thể mở rộng enum và policy

---

## 11.3. Liskov Substitution Principle

Các class kế thừa từ `Entity<TId>` và `AggregateRoot<TId>` vẫn giữ đúng hành vi cơ bản của Entity.

---

## 11.4. Interface Segregation Principle

`IDomainEvent` rất nhỏ, chỉ chứa `OccurredOn`.

Domain không bị ép phụ thuộc vào các interface lớn, phức tạp.

---

## 11.5. Dependency Inversion Principle

Domain không phụ thuộc vào Infrastructure.

Domain không biết:

- SQLite là gì
- EF Core là gì
- Email service là gì
- API controller là gì

Các phụ thuộc kỹ thuật sẽ nằm ở tầng Ports và Infrastructure.

---

# 12. Design Patterns đã áp dụng

| Design Pattern | Vị trí áp dụng | Mục đích |
|---|---|---|
| Entity Pattern | User, JobPost, JobApplication | Mô hình hóa đối tượng có định danh |
| Value Object Pattern | Email, SalaryRange, JobTitle | Bảo vệ rule dữ liệu |
| Aggregate Root Pattern | User, JobPost, JobApplication | Kiểm soát thay đổi nghiệp vụ |
| Factory Method Pattern | Create, Register, Submit | Tạo object hợp lệ |
| Policy Pattern | ApplicationStatusPolicy; rule job trong `JobPost` | Tách rule nghiệp vụ |
| Domain Event Pattern | Các file Event | Tách nghiệp vụ chính và nghiệp vụ phụ |
| State Pattern nhẹ | JobStatus, ApplicationStatus | Quản lý trạng thái vòng đời |
| Repository Pattern | Chưa nằm trong Domain, sẽ đặt ở Ports | Tách lưu trữ khỏi nghiệp vụ |

---

# 13. Mapping Domain với MVP Feature

| MVP Feature | Domain tương ứng |
|---|---|
| Phân quyền Admin, Employer, Candidate | User, UserRole |
| Quản lý danh mục hệ thống | DisabilityType, AssistiveDevice, JobCategory |
| CRUD cơ bản | Các Aggregate Root và method tương ứng |
| Tạo hồ sơ doanh nghiệp | EmployerProfile |
| Quản lý trạng thái hồ sơ ứng viên | JobApplication, ApplicationStatusPolicy |
| Ẩn thông tin khuyết tật | CandidateProfile, DisabilityInfo |
| Lọc việc theo hỗ trợ tiếp cận | JobAccessibilityInfo |
| Hỗ trợ xe lăn | JobAccessibilityInfo.SupportsWheelchairAccess |
| Làm việc từ xa | JobAccessibilityInfo.SupportsRemoteWork |
| Thời gian linh hoạt | JobAccessibilityInfo.SupportsFlexibleTime |
| Thiết bị chuyên biệt | JobAccessibilityInfo.ProvidesAssistiveDevices |
| Thông báo tự động giai đoạn 2 | Domain Event + **Notification** (entity `Notification`) |
| Employer tìm ứng viên giai đoạn 2 | CandidateProfile.IsPublicProfile |
| Đánh giá doanh nghiệp (Giai đoạn 2) | **CompanyReview** |
| Báo cáo vi phạm (Giai đoạn 2) | **ViolationReport**, `ReportStatus`, `ReportTargetType` |

---

# 14. Những phần cố ý chưa đưa vào Domain MVP

> **Đồng bộ code:** Domain hiện **đã có** thêm Notifications / Reviews / Reports so với MVP ban đầu; bảng dưới vẫn giữ ý nghĩa “không nhét infrastructure vào Domain”.

Các phần sau chưa nên đưa vào Domain giai đoạn đầu:

| Tính năng | Lý do |
|---|---|
| AI bóc tách tin tuyển dụng | **Đồng bộ code:** đã có tích hợp ở Presentation (`/api/ai`) — **không** đặt logic LLM trong Domain; dòng này giữ để nhắc ranh giới Domain. |
| Gợi ý việc bằng AI | Tương tự — orchestration ngoài Domain. |
| Điều khiển bằng giọng nói | Thuộc Presentation/Application |
| Chrome Extension | Là project riêng |
| NGO verification | Cần quy trình vận hành/phối hợp bên thứ ba |
| Email notification | Xử lý ở Application Layer bằng Domain Event |
| SQLite Repository | Thuộc InfrastructureSqlite |
| InMemory Repository | Thuộc InfrastructureInMemory |
| API Controller | Thuộc Presentation |
| DTO | Thuộc Application hoặc Presentation |

---

# 15. Những điểm cần lưu ý khi code tiếp

## 15.1. Không đưa Repository vào Domain

Không đặt các interface này trong Domain:

```csharp
IUserRepository
IJobRepository
IEmailService
IFileStorageService
```

Các interface này nên nằm ở Project `Ports`.

---

## 15.2. Không đưa DTO vào Domain

Không đặt:

```csharp
CreateJobRequest
JobResponse
CandidateProfileDto
```

DTO nên nằm ở tầng Application hoặc Presentation.

---

## 15.3. Không dùng Entity Framework Attribute trong Domain

Không nên viết:

```csharp
[Table("Users")]
[Key]
[Required]
```

Vì Domain không nên phụ thuộc EF Core.

Mapping database sẽ xử lý ở InfrastructureSqlite.

---

## 15.4. Aggregate chỉ tham chiếu nhau bằng Id

Nên viết:

```csharp
public Guid EmployerId { get; private set; }
public Guid CandidateId { get; private set; }
public Guid JobId { get; private set; }
```

Không nên viết:

```csharp
public EmployerProfile Employer { get; private set; }
public CandidateProfile Candidate { get; private set; }
public JobPost Job { get; private set; }
```

Lý do:

- Tránh object graph quá lớn
- Dễ kiểm soát transaction
- Đúng tinh thần DDD
- Dễ mapping database

---

# 16. Kết luận

Domain hiện tại đã hoàn thành phần nền tảng cho MVP của EnableVN.

Các phần đã làm gồm:

1. Common base cho Entity, AggregateRoot, DomainEvent, DomainException.
2. User Domain để quản lý tài khoản và phân quyền.
3. Employer Domain để quản lý hồ sơ doanh nghiệp.
4. Candidate Domain để quản lý hồ sơ ứng viên và quyền riêng tư khuyết tật.
5. Job Domain để quản lý tin tuyển dụng và tiêu chí accessibility.
6. Application Domain để quản lý hồ sơ ứng tuyển và trạng thái xử lý.
7. Catalog Domain để quản lý danh mục hệ thống.
8. Domain Event để chuẩn bị cho notification, email, audit log ở các phase sau.
9. Policy để tách rule nghiệp vụ phức tạp.
10. Value Object để bảo vệ dữ liệu đầu vào ngay trong Domain.

Thiết kế này phù hợp với:

- Hexagonal Architecture
- DDD
- SOLID
- MVP có khả năng mở rộng
- Dễ test
- Dễ triển khai tiếp sang Ports, Application và Infrastructure
