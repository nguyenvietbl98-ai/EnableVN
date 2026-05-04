# ENABLEVN_PRODUCTION_UI_AGENT_GUIDE.md

## 0. Vai trò của Agent

Bạn là Coding Agent hỗ trợ phát triển giao diện Production cho dự án **EnableVN**.

EnableVN là nền tảng tìm việc dành cho **người khuyết tật**, được xây dựng bằng **C# ASP.NET Core MVC**, theo định hướng:

- Hexagonal Architecture
- DDD
- SOLID
- MVC đúng chuẩn
- Accessibility-first
- Không đưa business logic vào Controller/View
- UI hiện đại, rõ ràng, dễ dùng, thân thiện với người khuyết tật

Repo public:

```txt
https://github.com/nguyenvietbl98-ai/EnableVN
```

Khi làm việc với repo, phải ưu tiên:

```txt
Code hiện tại > Summary 7 > Summary 6 > Summary 5 > Summary 4 > Summary 3 > Summary 2 > Summary 1
```

Nếu repo public chưa thấy Summary 6/7 hoặc SQLite code, **không được kết luận là chưa làm**. Hãy yêu cầu người dùng upload/push lại.

---

# 1. Mục tiêu chính

Tạo một UI có thể dùng như sản phẩm thật cho **Giai đoạn 1 + Giai đoạn 2** của EnableVN.

UI phải đạt các mục tiêu sau:

1. Đẹp, hiện đại, chuyên nghiệp.
2. Phù hợp với một nền tảng việc làm xã hội dành cho người khuyết tật.
3. Dễ dùng cho cả:
  - Candidate
  - Employer
  - Admin
4. Tuân thủ MVC.
5. Không phá kiến trúc Hexagonal/DDD.
6. Ưu tiên Accessibility/WCAG.
7. Có giao diện rõ ràng cho các luồng core:
  - Đăng nhập/đăng ký
  - Employer đăng việc
  - Candidate tìm việc/lọc việc/nộp hồ sơ
  - Employer quản lý ứng viên
  - Admin quản lý danh mục/báo cáo
  - Notification
  - Review doanh nghiệp
  - Report vi phạm

---

# 2. Nguyên tắc bắt buộc

## 2.1. Không phá kiến trúc

Không được viết business logic trong:

```txt
Controller
Razor View
JavaScript phía client
```

Controller chỉ được:

- Nhận request
- Validate ViewModel ở mức input cơ bản
- Gọi UseCase qua Ports.Inbound
- Map kết quả sang ViewModel
- Trả View/Redirect

Business logic phải nằm ở:

```txt
Application/UseCases
Domain
Policies
Services
```

Repository và database chỉ nằm ở Infrastructure.

---

## 2.2. Không tự ý đổi Domain nếu không cần

Khi làm UI, chỉ sửa Domain/Application/Ports nếu UI thật sự cần DTO/ViewModel/UseCase còn thiếu.

Không được tự ý đổi:

- Entity core
- ValueObject
- Enum
- Repository contract
- UseCase contract

trừ khi có lỗi compile hoặc thiếu dữ liệu bắt buộc cho UI.

---

## 2.3. UI phải accessibility-first

EnableVN không phải job board bình thường. Đây là sản phẩm cho người khuyết tật, vì vậy UI phải đặt Accessibility lên hàng đầu.

Bắt buộc:

- Điều hướng được bằng bàn phím 100%.
- Có `:focus-visible` rõ ràng.
- Có skip link: “Bỏ qua đến nội dung chính”.
- Không dùng drag/drop làm thao tác bắt buộc.
- Không chỉ dùng màu sắc để truyền đạt trạng thái.
- Mọi button/link/input phải có accessible name.
- Form phải có label thật, không chỉ placeholder.
- Mọi thông báo lỗi phải đọc được bởi screen reader.
- Tương phản màu đạt WCAG 2.1/2.2 tối thiểu AA.
- Có chế độ tương phản cao.
- HTML semantic rõ ràng: `header`, `nav`, `main`, `section`, `footer`.

---

# 3. Phạm vi UI cần làm

## 3.1. Giai đoạn 1 - MVP

### 3.1.1. Phân quyền người dùng

Vai trò:

```txt
Admin
Employer
Candidate
```

UI cần có:

- Trang đăng nhập
- Trang đăng ký
- Chọn vai trò khi đăng ký nếu hệ thống hiện tại hỗ trợ
- Điều hướng khác nhau theo role
- Header hiển thị trạng thái đăng nhập
- Menu riêng cho từng role

Gợi ý navigation:

Candidate:

```txt
Trang chủ
Tìm việc
Hồ sơ của tôi
Đơn ứng tuyển
Thông báo
```

Employer:

```txt
Bảng điều khiển
Hồ sơ doanh nghiệp
Tin tuyển dụng
Ứng viên
Thông báo
```

Admin:

```txt
Dashboard
Danh mục hệ thống
Người dùng
Tin tuyển dụng
Báo cáo vi phạm
```

---

### 3.1.2. Quản lý danh mục hệ thống

Admin cần quản lý các danh mục:

- Loại khuyết tật
- Thiết bị hỗ trợ
- Danh mục kỹ năng nếu đã có
- Danh mục ngành nghề nếu đã có

UI yêu cầu:

- Trang danh sách dạng table responsive
- Có tìm kiếm
- Có nút thêm mới rõ ràng
- Có sửa/xóa
- Có trạng thái empty state
- Có cảnh báo trước khi xóa
- Không dùng modal bắt buộc nếu gây khó cho screen reader; ưu tiên trang riêng hoặc modal có focus trap chuẩn

---

### 3.1.3. CRUD/Search core

Các thực thể chính cần UI CRUD/search tùy role:

- Job
- Employer Profile
- Candidate Profile
- Application
- System Catalog

Yêu cầu:

- Form rõ ràng
- Có validation message
- Có breadcrumb
- Có trạng thái loading nếu có thao tác dài
- Có empty state thân thiện

---

### 3.1.4. Trải nghiệm tiếp cận

Bắt buộc làm:

- High contrast mode
- Focus ring rõ
- Button đủ lớn
- Font dễ đọc
- Spacing thoáng
- Không dùng icon mà không có text/aria-label
- Không làm giao diện quá nhiều hiệu ứng gây rối
- Không autoplay animation

Nên có một nút trên header:

```txt
Tương phản cao
```

hoặc:

```txt
Chế độ dễ đọc
```

Có thể lưu lựa chọn bằng localStorage.

---

### 3.1.5. Employer Profile

Trang hồ sơ doanh nghiệp cần có:

- Tên doanh nghiệp
- Logo nếu có
- Mô tả
- Địa chỉ
- Website
- Quy mô
- Lĩnh vực
- Cam kết môi trường làm việc hòa nhập
- Các hỗ trợ tiếp cận nếu hệ thống có dữ liệu

UI cần hiển thị đẹp ở:

- Trang public company profile
- Trang employer edit profile

---

### 3.1.6. Quản lý trạng thái hồ sơ ứng viên

Employer cần xem danh sách ứng viên đã nộp vào job của mình.

Trạng thái tối thiểu:

```txt
Đang chờ
Phỏng vấn
Từ chối
Đã nhận / Chấp nhận nếu hệ thống có
```

UI cần có:

- Danh sách application theo job
- Bộ lọc theo trạng thái
- Chi tiết ứng viên
- Nút đổi trạng thái
- Badge trạng thái rõ ràng
- Timeline trạng thái nếu có dữ liệu

Lưu ý bảo mật:

- Employer chỉ được xem application thuộc job của mình.
- Không truyền `UserId` từ form nếu có thể lấy từ current user.
- Không cho sửa application của employer khác.

---

### 3.1.7. Ẩn thông tin khuyết tật

Candidate phải kiểm soát quyền riêng tư.

UI cần có:

- Checkbox/toggle rõ ràng:

```txt
Cho phép nhà tuyển dụng xem thông tin hỗ trợ/khuyết tật của tôi
```

- Giải thích ngắn gọn:

```txt
Bạn có thể ẩn thông tin này. Nhà tuyển dụng chỉ thấy khi bạn cho phép hoặc khi bạn chủ động chia sẻ trong hồ sơ ứng tuyển.
```

Candidate profile public không được tự động hiển thị thông tin nhạy cảm nếu domain/usecase không cho phép.

---

### 3.1.8. Job accessibility filters

Trong Job cần có các thuộc tính đặc thù:

- Hỗ trợ xe lăn
- Làm việc từ xa
- Thời gian linh hoạt
- Thiết bị chuyên biệt
- Môi trường phù hợp với người khuyết tật nếu có

UI Candidate search cần có filter rõ ràng:

```txt
[ ] Làm việc từ xa
[ ] Hỗ trợ xe lăn
[ ] Thời gian linh hoạt
[ ] Có thiết bị chuyên biệt
```

Job card nên hiển thị badge:

```txt
Remote
Xe lăn
Linh hoạt
Thiết bị hỗ trợ
```

---

# 4. Giai đoạn 2 - UI nâng cao

## 4.1. Notification system

Nếu Phase 2 Notification đã có code, tạo UI cho:

Candidate/Employer/Admin:

- Danh sách thông báo
- Badge số thông báo chưa đọc
- Trang chi tiết thông báo
- Mark as read
- Archive nếu usecase đã hỗ trợ

Notification card cần hiển thị:

- Tiêu đề
- Nội dung ngắn
- Loại thông báo
- Thời gian
- Trạng thái đọc/chưa đọc

Không để người dùng đọc/sửa thông báo của người khác.

Rule bảo mật quan trọng:

```csharp
if (notification.UserId != userId)
{
    throw new UseCaseException("Bạn không có quyền đọc thông báo này.");
}
```

---

## 4.2. Employer Candidate Search

Employer có thể tìm candidate đã public profile.

UI cần:

- Trang danh sách candidate public
- Search theo kỹ năng
- Filter theo kinh nghiệm nếu có
- Filter theo nhu cầu hỗ trợ nếu candidate cho phép hiển thị
- Candidate card
- Trang chi tiết candidate public

Tuyệt đối không hiển thị thông tin khuyết tật/nhạy cảm nếu Candidate không public.

---

## 4.3. Company Review

Candidate review Employer theo rule:

Candidate chỉ được review Employer nếu:

- Đã đăng nhập với role Candidate
- Có CandidateProfile
- Đã ứng tuyển vào job thuộc Employer đó
- Chưa review Employer đó trước đây

Có thể cân nhắc chỉ cho review khi application status là:

```txt
Interview
Accepted
```

UI cần:

- Form review
- Rating 1-5
- Nội dung nhận xét
- Danh sách review trên company profile
- Empty state nếu chưa có review
- Không hiển thị review đang bị ẩn nếu admin chưa duyệt, nếu hệ thống có moderation

---

## 4.4. Violation Report

Bất kỳ user đăng nhập nào có thể report:

- Tin đăng ảo
- Hành vi phân biệt đối xử
- Nội dung không phù hợp
- Công ty đáng ngờ

Admin có thể:

- Xem pending reports
- Xem chi tiết report
- Resolve
- Reject

UI cần:

User side:

- Nút “Báo cáo vi phạm” trên Job detail hoặc Company profile
- Form report rõ ràng
- Chọn lý do
- Nhập mô tả

Admin side:

- Table danh sách report
- Filter theo trạng thái
- Chi tiết report
- Nút xử lý
- Ghi chú xử lý nếu usecase hỗ trợ

Rule:

```txt
Any logged-in user can submit report.
Only Admin can view pending reports.
Only Admin can resolve/reject reports.
```

---

# 5. Định hướng giao diện Production

## 5.1. Phong cách tổng thể

Phong cách mong muốn:

```txt
Hiện đại
Sạch
Rõ ràng
Tin cậy
Thân thiện
Có cảm giác social impact
Không màu mè quá mức
```

Không làm UI kiểu demo sơ sài.

Không dùng quá nhiều gradient/animation.

Ưu tiên bố cục giống các nền tảng SaaS/job platform hiện đại:

- Header cố định hoặc sticky nhẹ
- Hero section rõ ràng
- Card-based layout
- Dashboard layout
- Table responsive
- Filter sidebar
- Empty state đẹp
- Alert/toast rõ nghĩa
- CTA nổi bật

---

## 5.2. Bảng màu đề xuất

Có thể dùng bảng màu này nếu project chưa có design system:

```css
:root {
  --evn-primary: #2563eb;
  --evn-primary-dark: #1d4ed8;
  --evn-secondary: #0f766e;
  --evn-accent: #f59e0b;

  --evn-bg: #f8fafc;
  --evn-surface: #ffffff;
  --evn-surface-soft: #f1f5f9;

  --evn-text: #0f172a;
  --evn-text-muted: #475569;

  --evn-border: #cbd5e1;
  --evn-danger: #dc2626;
  --evn-success: #16a34a;
  --evn-warning: #d97706;

  --evn-focus: #7c3aed;
}
```

High contrast mode:

```css
:root[data-theme="contrast"] {
  --evn-bg: #000000;
  --evn-surface: #0f0f0f;
  --evn-surface-soft: #1a1a1a;

  --evn-text: #ffffff;
  --evn-text-muted: #f5f5f5;

  --evn-primary: #ffff00;
  --evn-primary-dark: #ffff00;
  --evn-secondary: #00ffff;
  --evn-accent: #ffcc00;

  --evn-border: #ffffff;
  --evn-focus: #ff00ff;
}
```

---

## 5.3. Typography

Ưu tiên font hệ thống, dễ đọc, không phụ thuộc CDN nếu không cần:

```css
font-family: system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
```

Kích thước gợi ý:

```css
body: 16px
small text: 14px
heading h1: 40-48px desktop, 32px mobile
heading h2: 28-36px
heading h3: 22-24px
```

Line-height:

```css
1.5 - 1.7
```

---

## 5.4. Component bắt buộc

Tạo hoặc chuẩn hóa các component CSS/Razor partial sau nếu phù hợp:

```txt
_Layout.cshtml
_Header.cshtml
_Footer.cshtml
_Sidebar.cshtml
_ValidationSummary.cshtml
_NotificationBell.cshtml
_JobCard.cshtml
_CompanyCard.cshtml
_CandidateCard.cshtml
_ApplicationStatusBadge.cshtml
_AccessibilityBadges.cshtml
_EmptyState.cshtml
_Pagination.cshtml
_Breadcrumb.cshtml
```

Không nhất thiết phải tạo tất cả nếu project hiện tại nhỏ, nhưng nên ưu tiên reusable partial.

---

# 6. Các trang cần có

## 6.1. Public pages

### Home

Trang chủ cần có:

- Hero section
- CTA cho Candidate:

```txt
Tìm việc phù hợp
```

- CTA cho Employer:

```txt
Đăng tuyển dụng hòa nhập
```

- Search box nhanh
- Giới thiệu giá trị EnableVN
- Các accessibility feature nổi bật
- Danh sách job mới nhất nếu có
- Danh sách công ty nổi bật nếu có

Thông điệp gợi ý:

```txt
Kết nối cơ hội việc làm hòa nhập cho mọi người.
```

---

### Job list/search

Cần có:

- Search keyword
- Location
- Filter accessibility
- Job cards
- Sorting nếu có
- Empty state
- Pagination

Job card hiển thị:

- Title
- Company
- Location/Remote
- Salary nếu có
- Accessibility badges
- Ngày đăng
- CTA “Xem chi tiết”

---

### Job detail

Cần có:

- Title
- Company info
- Job description
- Requirements
- Benefits
- Accessibility support
- Apply button
- Report button nếu logged in

Candidate đã apply rồi thì hiển thị trạng thái thay vì nút apply.

---

### Company profile public

Cần có:

- Logo
- Company name
- Description
- Inclusive workplace statement
- Job openings
- Reviews nếu Phase 2 có
- Report company button nếu Phase 2 có

---

## 6.2. Candidate pages

### Candidate dashboard

Hiển thị:

- Lời chào
- Số đơn đã nộp
- Số đơn đang phỏng vấn
- Job gợi ý đơn giản nếu có
- Notification mới
- CTA hoàn thiện hồ sơ

---

### Candidate profile

Gồm:

- Thông tin cá nhân
- Kỹ năng
- Kinh nghiệm
- Học vấn nếu có
- Nhu cầu hỗ trợ
- Toggle ẩn/hiện thông tin khuyết tật
- CV hoặc mô tả năng lực nếu có

---

### My applications

Gồm:

- Danh sách job đã apply
- Trạng thái
- Ngày apply
- Link job detail
- Link company profile

---

### Notifications

Gồm:

- Danh sách notification
- Đọc/chưa đọc
- Mark as read
- Archive nếu có

---

## 6.3. Employer pages

### Employer dashboard

Hiển thị:

- Tổng số job
- Job đang mở
- Tổng số application
- Application mới
- Notification mới
- CTA đăng việc

---

### Employer profile edit

Form chỉnh sửa profile công ty.

---

### Manage jobs

Gồm:

- Danh sách job của employer
- Tạo job
- Sửa job
- Đóng/mở job nếu usecase hỗ trợ
- Số application từng job

---

### Create/Edit job

Form cần có:

- Title
- Description
- Requirements
- Benefits
- Location
- Remote
- Flexible time
- Wheelchair support
- Assistive devices
- Job status nếu có

Accessibility fields không được bị giấu sâu. Chúng là linh hồn của EnableVN.

---

### Applications management

Gồm:

- Danh sách application theo job
- Filter status
- Candidate summary
- Detail
- Change status

---

### Candidate search

Gồm:

- Search box
- Filter skills
- Candidate card
- Candidate detail
- Privacy-safe rendering

---

## 6.4. Admin pages

### Admin dashboard

Hiển thị:

- Số user
- Số employer
- Số candidate
- Số job
- Số report pending
- Link nhanh tới catalog/report

---

### Catalog management

Quản lý danh mục hệ thống.

---

### Report management

Gồm:

- Pending reports
- Resolved reports
- Rejected reports
- Detail
- Resolve/Reject action

---

# 7. ViewModel định hướng

Nếu chưa có ViewModel phù hợp, tạo trong Presentation.

Không dùng trực tiếp Domain Entity trong Razor View nếu có thể tránh.

Gợi ý folder:

```txt
ENABLEVN/ViewModels/
  Home/
  Jobs/
  Candidates/
  Employers/
  Applications/
  Notifications/
  Reviews/
  Reports/
  Admin/
```

Ví dụ:

```csharp
public sealed class JobCardViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public bool IsRemote { get; init; }
    public bool SupportsWheelchair { get; init; }
    public bool HasFlexibleTime { get; init; }
    public bool HasAssistiveDevices { get; init; }
    public DateTime CreatedAt { get; init; }
}
```

---

# 8. CSS/JS tổ chức

Ưu tiên tạo file:

```txt
wwwroot/css/enablevn.css
wwwroot/js/enablevn-accessibility.js
```

Không viết CSS inline trong Razor trừ trường hợp rất nhỏ.

Không viết JS phức tạp nếu không cần.

JS chỉ nên dùng cho:

- Toggle high contrast
- Toggle mobile nav
- Confirm action
- Small enhancement

Không dùng JS để xử lý rule nghiệp vụ.

---

# 9. Layout chuẩn

## 9.1. Layout yêu cầu

`_Layout.cshtml` cần có:

- Skip link
- Header
- Role-based nav
- Main content có `id="main-content"`
- Footer
- RenderSection Scripts
- Antiforgery nếu form cần
- CSS chính
- JS accessibility

Ví dụ cấu trúc:

```html
<a class="skip-link" href="#main-content">Bỏ qua đến nội dung chính</a>

<header class="evn-header">
  ...
</header>

<main id="main-content" tabindex="-1">
  @RenderBody()
</main>

<footer class="evn-footer">
  ...
</footer>
```

---

# 10. Accessibility checklist trước khi hoàn thành

Agent phải tự kiểm tra:

- Tab qua toàn bộ trang được.
- Focus ring nhìn rõ.
- Không có button/link rỗng text.
- Input có label.
- Error message gắn với input nếu có thể.
- Có skip link.
- Không dùng màu là tín hiệu duy nhất.
- Badge trạng thái có text.
- Table có header.
- Modal nếu có phải có focus management.
- High contrast mode hoạt động.
- Mobile nav dùng được bằng keyboard.
- Không có console error.
- Không có Razor compile error.

---

# 11. Production quality checklist

Trước khi kết thúc, Agent phải đảm bảo:

- `dotnet build` thành công.
- Không phá migration/database hiện tại.
- Không đổi namespace tùy tiện.
- Không tạo duplicate class trùng tên.
- Không hard-code UserId trong form.
- Không bypass UseCase.
- Không truy cập repository trực tiếp từ Controller.
- Không hiển thị disability info nếu Candidate ẩn.
- Không để Employer xem application không thuộc job của họ.
- Không để user đọc notification của người khác.
- Không để non-admin xử lý report.
- UI responsive mobile/tablet/desktop.
- Form có validation.
- Empty state có nội dung thân thiện.
- Error state rõ ràng.

---

# 12. Thứ tự làm việc đề xuất cho Agent

Không làm lung tung. Làm theo thứ tự sau.

## Bước 1. Audit repo

Đọc các phần:

```txt
ENABLEVN/
Application/
Domain/
Ports/
InfrastructureSqlite/
InfrastructureInMemory/
```

Xác định:

- Controller hiện có
- View hiện có
- Layout hiện có
- CSS hiện có
- UseCase hiện có
- DTO/Result hiện có

Không đoán bừa.

---

## Bước 2. Chuẩn hóa layout và design system

Làm trước:

```txt
_Layout.cshtml
enablevn.css
enablevn-accessibility.js
```

Sau đó mới làm từng trang.

---

## Bước 3. Làm public pages

Ưu tiên:

```txt
Home
Job list
Job detail
Company profile
```

---

## Bước 4. Làm Candidate UI

Ưu tiên:

```txt
Candidate dashboard
Candidate profile
My applications
Notifications
```

---

## Bước 5. Làm Employer UI

Ưu tiên:

```txt
Employer dashboard
Employer profile
Manage jobs
Application management
Candidate search
```

---

## Bước 6. Làm Admin UI

Ưu tiên:

```txt
Admin dashboard
Catalog management
Report management
```

---

## Bước 7. Polish accessibility

Sau khi đủ trang, quay lại polish:

```txt
Focus
Contrast
Keyboard
Responsive
Empty states
Validation
```

---

# 13. Nội dung text/UX nên dùng

Một số câu chữ nên dùng để UI có cảm giác sản phẩm thật:

```txt
Tìm công việc phù hợp với năng lực và nhu cầu tiếp cận của bạn.
```

```txt
Đăng tuyển dụng hòa nhập để tiếp cận nhiều ứng viên tài năng hơn.
```

```txt
EnableVN giúp kết nối doanh nghiệp với người lao động khuyết tật trong một môi trường minh bạch, tôn trọng và dễ tiếp cận.
```

```txt
Bạn có toàn quyền kiểm soát việc hiển thị thông tin hỗ trợ cá nhân.
```

```txt
Thông tin này chỉ được chia sẻ khi bạn cho phép.
```

```txt
Báo cáo này sẽ được gửi tới quản trị viên để xem xét.
```

---

# 14. Những điều tuyệt đối tránh

Không được:

- Làm UI màu mè nhưng khó đọc.
- Bỏ qua accessibility.
- Dùng drag/drop làm thao tác chính.
- Dùng icon-only button không có aria-label.
- Dùng placeholder thay label.
- Cho candidate disability info hiển thị mặc định.
- Cho Employer xem dữ liệu không thuộc quyền.
- Cho Admin page lộ với user thường.
- Viết business logic trong Razor.
- Gọi DbContext trực tiếp từ Controller.
- Tạo UI nhưng không nối vào UseCase.
- Xóa code cũ khi chưa hiểu.
- Đổi namespace hàng loạt không cần thiết.
- Đưa AI/Voice của Phase 3 vào UI Phase 1/2.

---

# 15. Definition of Done

Agent chỉ được xem là hoàn thành khi:

1. UI có giao diện production-level cho Phase 1 và Phase 2.
2. Các role có navigation riêng.
3. Các luồng chính Candidate/Employer/Admin dùng được.
4. Accessibility được xử lý rõ ràng.
5. High contrast mode có thể bật/tắt.
6. Responsive tốt.
7. Không phá architecture.
8. `dotnet build` pass.
9. Không có lỗi Razor compile.
10. Người dùng có thể chạy:

```powershell
dotnet run --project .\ENABLEVN\Presentation.csproj
```

và kiểm tra UI.

---

# 16. Ghi chú quan trọng theo dự án EnableVN

Hiện dự án đang đi theo các giai đoạn:

## Giai đoạn 1

Lõi hệ thống và trải nghiệm thiết yếu:

- Role Admin/Employer/Candidate
- Catalog admin
- CRUD/Search
- WCAG/accessibility UI
- High contrast/no drag-drop
- Employer profile
- Application status management
- Hidden disability info/privacy
- Job accessibility filters

## Giai đoạn 2

Nâng cao tương tác và tự động hóa:

- Notification system
- Employer candidate search
- Company review
- Violation report

Không đưa Phase 3 vào lúc này.

Phase 3 gồm AI/Machine Learning, voice, third-party integration. Chỉ chuẩn bị UI sao cho sau này có thể mở rộng, nhưng không triển khai trong task này.

---

## # 17. Prompt khởi chạy Agent (Dành cho UI/UX Production)

Bạn là một Expert Frontend Developer & C# [ASP.NET](http://ASP.NET) Core Engineer. Hãy đọc toàn bộ repo EnableVN hiện tại (ưu tiên code thật hơn file markdown). 

Nhiệm vụ của bạn là XÂY DỰNG TOÀN BỘ HỆ THỐNG GIAO DIỆN (UI) TỪ A-Z cho Giai đoạn 1 và Giai đoạn 2 dựa theo định hướng của file ENABLEVN_PRODUCTION_UI_AGENT_[GUIDE.md](http://GUIDE.md). Bạn KHÔNG ĐƯỢC làm hời hợt, KHÔNG ĐƯỢC chỉ làm trang mẫu (mockup), mà phải code hoàn thiện TẤT CẢ các View/Controller cho cả 3 Role (Candidate, Employer, Admin).

MỤC TIÊU TỐI THƯỢNG: Giao diện toàn hệ thống phải CỰC KỲ ĐẸP, HIỆN ĐẠI, ĐỒNG BỘ và TUÂN THỦ NGHIÊM NGẶT ACCESSIBILITY (WCAG).

📍 Lộ trình thực hiện (Yêu cầu làm tuần tự đến khi xong TOÀN BỘ WEB):

1. Nền tảng UI: Setup `_Layout.cshtml`, `_Header.cshtml`, `_Footer.cshtml`, `enablevn.css`, `enablevn-accessibility.js` và hệ thống Component (Cards, Buttons, Badges).

2. Public UI: Trang chủ (Home), Danh sách việc làm, Chi tiết việc làm, Public Profile Doanh nghiệp.

3. Candidate UI: Dashboard, Quản lý Profile (tích hợp toggle ẩn/hiện khuyết tật), Danh sách việc đã ứng tuyển, Thông báo, Form Review Công ty.

4. Employer UI: Dashboard, Quản lý Profile, CRUD Tin tuyển dụng, Quản lý luồng trạng thái ứng viên, Tìm kiếm ứng viên (public).

5. Admin UI: Dashboard, Quản lý danh mục (Catalog), Quản lý/Xét duyệt Báo cáo vi phạm (Violation Report).

📍 Yêu cầu thiết kế UI/UX & Kỹ thuật:

- Thiết kế theo phong cách Modern SaaS/Job Portal (Card-based layout, whitespace thoáng đãng, đổ bóng tinh tế, hover effects mượt mà). Không dùng giao diện mặc định nhàm chán.

- Giữ đúng [ASP.NET](http://ASP.NET) Core MVC. KHÔNG viết business logic vào Controller/Razor View.

- Tôn trọng kiến trúc Hexagonal (Domain, Ports, Application, Infrastructure).

- Accessibility-first: Hỗ trợ Keyboard navigation 100%, có High Contrast Mode, form input báo lỗi chuẩn xác.

- Bỏ qua Phase 3 (AI, Voice, External Auth) trong task này.

⚠️ LƯU Ý QUAN TRỌNG CHO AGENT: 

Mọi thay đổi code phải đảm bảo `dotnet build` pass 100%. Nếu quá trình sinh code đạt tới giới hạn độ dài của một lần phản hồi, hãy dừng lại đúng ngữ pháp và lập tức yêu cầu tôi gõ "Tiếp tục" để bạn hoàn thiện nốt các trang còn lại. Tuyệt đối không được bỏ sót bất kỳ trang nào!