# EnableVN — Tóm tắt phiên làm việc: Giai đoạn 1 → bắt đầu Giai đoạn 2

**Mục đích file:** Ghi lại những gì đã thống nhất, đã phân tích và đã hướng dẫn trong phiên làm việc này để các phiên sau hoặc agent khác có thể tiếp tục đúng trạng thái dự án.

---

## 1. Nguyên tắc đọc dự án đã thống nhất

Khi đọc repo EnableVN, không được lấy riêng một file `.md` cũ làm sự thật cuối cùng.

Thứ tự ưu tiên đúng:

```text
Code hiện tại > Summary 6 > Summary 5 > Summary 4 > Summary 3 > Summary 2 > Summary 1
```

Lý do:

- Các file summary được tạo theo từng giai đoạn phát triển.
- `EnableVN_Application_Summary_3.md` chỉ phản ánh trạng thái ở lần tạo thứ 3.
- Những phần bản 3 ghi “chưa làm” có thể đã được bù ở bản 4, bản 5, bản 6 hoặc code hiện tại.
- Bản 6 là bản cập nhật mới nhất liên quan đến SQLite.

---

## 2. Bản thứ 6 đã được bổ sung

Người dùng đã cung cấp file:

```text
SQLITE_IMPLEMENTATION_SUMMARY_6.md
```

Nội dung chính của bản 6:

- `InfrastructureSqlite` đã được triển khai.
- Đã có `PersistenceModels`.
- Đã có `Mappers`.
- Đã có `Repositories`.
- Đã có `EnableVnDbContext`.
- Đã có `EnableVnDbContextFactory`.
- Đã có seed admin/catalog.
- Đã có migration `InitialSqliteCreate`.
- Đã tạo database `enablevn.db`.
- Build thành công theo nội dung summary.

Kết luận mới sau bản 6:

```text
Domain + Ports + Application + InfrastructureInMemory + InfrastructureSqlite + Presentation đều đã có nền tảng.
```

Không còn coi `InfrastructureSqlite` là chưa làm.

---

## 3. Đánh giá Giai đoạn 1 MVP

Đã chấm dự án khoảng:

```text
84% Giai đoạn 1 MVP
```

Lý do chấm như vậy:

### Đã đạt tốt

- Kiến trúc Hexagonal tương đối rõ.
- Có Domain, Ports, Application, InfrastructureInMemory, InfrastructureSqlite, Presentation.
- Có phân quyền Admin / Employer / Candidate.
- Có Auth cơ bản.
- Có Employer Profile.
- Có Candidate Profile.
- Có ẩn thông tin khuyết tật.
- Có Job Post.
- Có Job Search / Filter theo accessibility.
- Có Candidate Apply.
- Có Employer quản lý trạng thái ứng tuyển.
- Có SQLite persistence.
- Có seed admin/catalog.

### Chưa nên chấm 100%

- Chưa có bằng chứng audit WCAG 2.1/2.2 đầy đủ.
- Chưa kiểm thử thủ công toàn bộ flow sau khi chuyển SQLite.
- Auth/Token/PasswordHasher còn đơn giản, phù hợp MVP nhưng chưa production.
- Chưa thấy test tự động cho Domain/Application/Repository.
- Accessibility UI cần nâng cấp thêm.

---

## 4. Hướng triển khai Giai đoạn 2 đã thống nhất

Giai đoạn 2 gồm 4 nhóm tính năng:

```text
1. Notification System
2. Employer tìm ứng viên public profile
3. Đánh giá doanh nghiệp
4. Báo cáo vi phạm
```

Thứ tự nên làm:

```text
Bước 1: Notification
Bước 2: Employer Search Candidate
Bước 3: Company Review
Bước 4: Report System
Bước 5: Migration + test toàn luồng
```

Lý do chọn Notification trước:

- Ít đụng UI nhất.
- Nối trực tiếp vào flow đã có:
  - Candidate nộp hồ sơ.
  - Employer đổi trạng thái hồ sơ.
- Tăng giá trị sử dụng ngay cho MVP.

---

## 5. Notification System — Những phần đã hướng dẫn tạo

### 5.1. Domain

Tạo thư mục:

```text
Domain/Notifications/
```

Các file:

```text
Notification.cs
NotificationType.cs
NotificationStatus.cs
```

Trong đó (khớp code hiện tại):

```csharp
public enum NotificationType
{
    Unknown = 0,
    System = 1,
    JobApplication = 2,
    ApplicationSubmitted = 3,
    ApplicationStatusChanged = 4,
    Message = 5
}
```

```csharp
public enum NotificationStatus
{
    Unread = 0,
    Read = 1,
    Archived = 2
}
```

`Notification` là Domain Entity (khớp SQLite persistence), kế thừa đúng kiểu generic:

```csharp
public sealed class Notification : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public static Notification Create(Guid userId, string title, string message, NotificationType type);
    public static Notification Restore(Guid id, Guid userId, string title, string message, NotificationType type, NotificationStatus status, DateTime createdAt, DateTime? readAt);
    public void MarkAsRead();
}
```

Lý do: trong project hiện tại, `Entity` là generic `Entity<TId>`, nên nếu viết `Entity` sẽ lỗi:

```text
CS0305: Using the generic type 'Entity<TId>' requires 1 type arguments
```

---

### 5.2. Ports Models

Tạo thư mục:

```text
Ports/Models/Notifications/
```

Tạo file:

```text
NotificationResult.cs
```

Yêu cầu class phải là `public` (khớp code hiện tại dùng init-only):

```csharp
public sealed class NotificationResult
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public NotificationType Type { get; init; }
    public NotificationStatus Status { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ReadAt { get; init; }
}
```

---

### 5.3. Outbound Port

Tạo file:

```text
Ports/Outbound/Repositories/INotificationRepository.cs
```

Interface gồm các method chính:

```csharp
Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
Task<int> CountUnreadAsync(Guid userId, CancellationToken cancellationToken = default);
Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
```

---

### 5.4. Inbound Port

Tạo file:

```text
Ports/Inbound/INotificationUseCase.cs
```

Interface gồm:

```csharp
Task<IReadOnlyList<NotificationResult>> GetMyNotificationsAsync(CancellationToken cancellationToken = default);
Task<int> CountMyUnreadAsync(CancellationToken cancellationToken = default);
Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
```

---

### 5.5. Application Mapper

Tạo file:

```text
Application/Mappers/NotificationMapper.cs
```

Namespace nên thống nhất:

```csharp
namespace Application.Mappers;
```

Cần có using:

```csharp
using Domain.Notifications;
using Ports.Models.Notifications;
```

Mapper chuyển từ Domain Entity sang DTO:

```csharp
public static NotificationResult ToResult(Notification notification)
```

---

### 5.6. Application UseCase

Tạo file:

```text
Application/UseCases/NotificationUseCase.cs
```

UseCase chịu trách nhiệm:

- Lấy thông báo của user hiện tại.
- Đếm thông báo chưa đọc.
- Đánh dấu thông báo đã đọc.
- Kiểm tra notification có thuộc user hiện tại hay không.

Rule bảo mật quan trọng trong `MarkAsReadAsync`:

```csharp
if (notification.UserId != userId)
    throw new UseCaseException("Bạn không có quyền đọc thông báo này.");
```

---

### 5.7. Đăng ký DI trong Application

Mở:

```text
Application/DependencyInjection.cs
```

Thêm:

```csharp
services.AddScoped<INotificationUseCase, NotificationUseCase>();
```

---

## 6. SQLite cho Notification — Những phần cần làm

### 6.1. Persistence Model

Tạo file:

```text
InfrastructureSqlite/PersistenceModels/NotificationRecord.cs
```

Các cột chính:

```csharp
public Guid Id { get; set; }
public Guid UserId { get; set; }
public string Title { get; set; } = string.Empty;
public string Message { get; set; } = string.Empty;
public string Type { get; set; } = string.Empty;
public string Status { get; set; } = string.Empty;
public DateTime CreatedAt { get; set; }
public DateTime? ReadAt { get; set; }
```

---

### 6.2. Persistence Mapper

Tạo file:

```text
InfrastructureSqlite/Mappers/NotificationPersistenceMapper.cs
```

Gồm 3 method:

```csharp
ToRecord(Notification notification)
ToDomain(NotificationRecord record)
UpdateRecord(NotificationRecord record, Notification notification)
```

Enum lưu dạng string:

```csharp
Type = notification.Type.ToString()
Status = notification.Status.ToString()
```

Khi restore:

```csharp
Enum.Parse<NotificationType>(record.Type)
Enum.Parse<NotificationStatus>(record.Status)
```

Ghi chú quan trọng (khớp code hiện tại):

- `NotificationRecord.CreatedAt/ReadAt` dùng `DateTime/DateTime?`.
- Domain `Notification.CreatedAt/ReadAt` cũng dùng `DateTime/DateTime?`.
- `NotificationResult` dùng `DateTimeOffset/DateTimeOffset?` để Presentation hiển thị timezone dễ hơn; mapper hiện map trực tiếp từ `DateTime` sang `DateTimeOffset` (implicit conversion).

---

### 6.3. DbContext

Mở:

```text
InfrastructureSqlite/Persistence/EnableVnDbContext.cs
```

Thêm DbSet:

```csharp
public DbSet<NotificationRecord> Notifications => Set<NotificationRecord>();
```

Thêm config trong `OnModelCreating`:

```csharp
modelBuilder.Entity<NotificationRecord>(entity =>
{
    entity.ToTable("Notifications");

    entity.HasKey(x => x.Id);

    entity.Property(x => x.UserId).IsRequired();

    entity.Property(x => x.Title)
        .IsRequired()
        .HasMaxLength(200);

    entity.Property(x => x.Message)
        .IsRequired()
        .HasMaxLength(1000);

    entity.Property(x => x.Type)
        .IsRequired()
        .HasMaxLength(50);

    entity.Property(x => x.Status)
        .IsRequired()
        .HasMaxLength(50);

    entity.Property(x => x.CreatedAt).IsRequired();

    entity.HasIndex(x => x.UserId);

    entity.HasIndex(x => new { x.UserId, x.Status });
});
```

---

### 6.4. SQLite Repository

Tạo file:

```text
InfrastructureSqlite/Repositories/SqliteNotificationRepository.cs
```

Repository implement:

```csharp
INotificationRepository
```

Các method chính:

```csharp
AddAsync
GetByIdAsync
GetByUserIdAsync
CountUnreadAsync
UpdateAsync
```

---

### 6.5. Đăng ký DI trong InfrastructureSqlite

Mở:

```text
InfrastructureSqlite/DependencyInjection.cs
```

Thêm:

```csharp
services.AddScoped<INotificationRepository, SqliteNotificationRepository>();
```

---

## 7. Gắn Notification vào JobApplicationUseCase

Trong:

```text
Application/UseCases/JobApplicationUseCase.cs
```

Thêm dependency:

```csharp
private readonly INotificationRepository _notificationRepository;
```

Thêm vào constructor:

```csharp
INotificationRepository notificationRepository
```

Gán:

```csharp
_notificationRepository = notificationRepository;
```

---

### 7.1. Candidate nộp hồ sơ → thông báo cho Employer

Trong `SubmitAsync`, sau khi lưu application:

```csharp
await _jobApplicationRepository.AddAsync(application, cancellationToken);
```

tạo notification cho Employer:

```csharp
var notification = Notification.Create(
    employerProfile.UserId,
    "Có hồ sơ ứng tuyển mới",
    $"Một ứng viên vừa nộp hồ sơ vào tin: {job.Title.Value}.",
    NotificationType.ApplicationSubmitted
);

await _notificationRepository.AddAsync(notification, cancellationToken);
```

---

### 7.2. Employer đổi trạng thái → thông báo cho Candidate

Trong `ChangeStatusAsync`, sau khi update application:

```csharp
await _jobApplicationRepository.UpdateAsync(application, cancellationToken);
```

tạo notification cho Candidate:

```csharp
var notification = Notification.Create(
    candidateProfile.UserId,
    "Trạng thái hồ sơ đã thay đổi",
    $"Hồ sơ của bạn đã được cập nhật sang trạng thái: {application.Status}.",
    NotificationType.ApplicationStatusChanged
);

await _notificationRepository.AddAsync(notification, cancellationToken);
```

---

## 8. Presentation cho Notification

Tạo controller:

```text
ENABLEVN/Controllers/NotificationsController.cs
```

Action chính:

```csharp
Index()
MarkAsRead(Guid id)
```

Tạo view:

```text
ENABLEVN/Views/Notifications/Index.cshtml
```

View hiển thị:

- Danh sách thông báo.
- Badge “Chưa đọc”.
- Ngày tạo.
- Button “Đánh dấu đã đọc”.

Form chỉ gửi `id`, không gửi `UserId`.

```html
<input type="hidden" name="id" value="@item.Id" />
```

---

## 9. Các lỗi đã gặp và cách xử lý

### 9.1. Lỗi `Entity<TId>`

Lỗi:

```text
CS0305: Using the generic type 'Entity<TId>' requires 1 type arguments
```

Nguyên nhân (đã gặp với `Notification`, `CompanyReview`, `ViolationReport`):

```csharp
public sealed class Notification : Entity
```

Sai vì `Entity` trong project là generic.

Cách sửa:

```csharp
public sealed class Notification : Entity<Guid>
```

Constructor private cần gọi:

```csharp
) : base(id)
```

---

### 9.2. Lỗi protection level

Lỗi:

```text
CS0122: 'Notification' is inaccessible due to its protection level
```

Nguyên nhân:

- `Notification` để `internal`.
- `NotificationMapper` nằm ở project `Application`.
- `Notification` nằm ở project `Domain`.
- `internal` chỉ dùng được trong cùng assembly.

Cách sửa:

```csharp
public sealed class Notification : Entity<Guid>
public enum NotificationType
public enum NotificationStatus
```

---

### 9.3. Lỗi không tìm thấy NotificationResult

Lỗi:

```text
CS0246: The type or namespace name 'NotificationResult' could not be found
```

Nguyên nhân có thể gồm:

- `NotificationResult` để `internal`.
- Chưa có `using Ports.Models.Notifications;`.
- Class DTO chưa khai báo đủ property.

Cách sửa:

```csharp
public sealed class NotificationResult
```

và thêm:

```csharp
using Ports.Models.Notifications;
```

trong `NotificationMapper`.

---

## 10. Vấn đề bảo mật khi đổi internal sang public

Đổi các type sau sang `public` không phải vấn đề bảo mật lớn:

```csharp
Notification
NotificationType
NotificationStatus
NotificationResult
```

Vì `public` chỉ có nghĩa là project khác trong solution có thể nhìn thấy type.

Điểm cần giữ để an toàn:

```text
1. Domain property dùng private set.
2. Constructor Domain để private.
3. Tạo object qua Create/Restore.
4. UseCase kiểm tra current user.
5. Controller không nhận UserId từ form.
6. Không trả Domain Entity trực tiếp ra View/API.
```

Đặc biệt:

```csharp
if (notification.UserId != userId)
    throw new UseCaseException("Bạn không có quyền đọc thông báo này.");
```

là rule bảo mật quan trọng.

---

## 11. Windows migration — Chọn Cách 1

Người dùng dùng Windows.

Cách 1 đã thống nhất:

```text
Không đổi tên Presentation.csproj.
Chỉ dùng đúng đường dẫn startup project là .\ENABLEVN\Presentation.csproj.
```

Lệnh migration đúng trong PowerShell:

```powershell
dotnet ef migrations add AddNotifications --project .\InfrastructureSqlite\InfrastructureSqlite.csproj --startup-project .\ENABLEVN\Presentation.csproj --context EnableVnDbContext
```

Lệnh update database:

```powershell
dotnet ef database update --project .\InfrastructureSqlite\InfrastructureSqlite.csproj --startup-project .\ENABLEVN\Presentation.csproj --context EnableVnDbContext
```

Chạy app:

```powershell
dotnet run --project .\ENABLEVN\Presentation.csproj
```

---

## 12. Lỗi migration đã gặp

Người dùng chạy lệnh với:

```powershell
--startup-project .\ENABLEVN\ENABLEVN.csproj
```

và bị lỗi:

```text
Unable to retrieve project metadata. Ensure it's an SDK-style project.
```

Nguyên nhân khả năng cao:

- Tên startup project không đúng.
- Project trong thư mục `ENABLEVN` thực tế là `Presentation.csproj`, không phải `ENABLEVN.csproj`.

Cách kiểm tra:

```powershell
dir .\ENABLEVN\*.csproj
```

Nếu thấy:

```text
Presentation.csproj
```

thì dùng `Presentation.csproj`.

---

## 13. Lưu ý package warning

Có warning:

```text
NU1903: Package 'System.Security.Cryptography.Xml' 9.0.0 has a known high severity vulnerability
```

Đây là warning bảo mật package, chưa phải nguyên nhân migration fail.

Nên xử lý sau bằng cách:

```powershell
dotnet list package --vulnerable
```

và cập nhật package phù hợp.

---

## 14. Giải thích phần 6.3 và 7.3 trước đó

### 14.1. Company Review UseCase Rule

Rule:

```text
Candidate chỉ được đánh giá Employer nếu:
1. User đang đăng nhập.
2. User có role Candidate.
3. User đã tạo CandidateProfile.
4. Candidate từng ứng tuyển vào ít nhất một job của Employer đó.
5. Candidate chưa từng đánh giá Employer đó trước đây.
```

Phần này sẽ nằm trong:

```text
Application/UseCases/CompanyReviewUseCase.cs
```

---

### 14.2. Violation Report UseCase Rule

Rule:

```text
User đăng nhập nào cũng có thể gửi report.
Chỉ Admin được xem danh sách report pending.
Chỉ Admin được resolve/reject report.
```

Phần này sẽ nằm trong:

```text
Application/UseCases/ViolationReportUseCase.cs
```

Nghĩa là:

| Hành động | Ai được làm |
|---|---|
| Gửi report | User đã đăng nhập |
| Xem pending reports | Admin |
| Resolve report | Admin |
| Reject report | Admin |

---

## 15. Trạng thái hiện tại của phiên

Hiện đang ở giai đoạn:

```text
Đang triển khai Notification System của Giai đoạn 2.
```

Việc đã xử lý/hướng dẫn:

```text
1. Hiểu lại dự án với đủ 6 bản summary.
2. Chấm Giai đoạn 1 khoảng 84%.
3. Chọn thứ tự Giai đoạn 2.
4. Bắt đầu Notification System.
5. Fix hướng kế thừa Entity<Guid>.
6. Fix lỗi public/internal cho Notification và DTO.
7. Giải thích bảo mật khi đổi public.
8. Chọn cách chạy migration trên Windows bằng Presentation.csproj.
```

Việc nên làm ngay tiếp theo:

```text
1. Hoàn tất SQLite Notification nếu chưa xong.
2. Chạy dotnet build.
3. Chạy migration AddNotifications bằng Presentation.csproj.
4. Chạy database update.
5. Test Candidate apply → Employer nhận notification.
6. Test Employer đổi trạng thái → Candidate nhận notification.
7. Sau đó mới sang Employer Search Candidate.
```
