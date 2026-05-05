# Đặc tả mở rộng Hồ sơ Ứng viên/Doanh nghiệp + Quy trình duyệt Admin

## 1) Mục tiêu

Mở rộng chức năng hồ sơ trên toàn hệ thống để:

- Hồ sơ ứng viên đầy đủ các trường nghiệp vụ tuyển dụng thực tế.
- Hồ sơ doanh nghiệp đầy đủ các trường nhận diện/uy tín tuyển dụng.
- Bổ sung cơ chế **Admin kiểm duyệt hồ sơ doanh nghiệp** trước khi cho phép đăng tin.
- Tuân thủ kiến trúc hiện tại: **Hexagonal Architecture**, **DDD**, **SOLID**.

---

## 2) Phạm vi thay đổi nghiệp vụ

## 2.1 Hồ sơ ứng viên (Candidate Profile)

Trường dữ liệu yêu cầu:

- Họ tên
- Ảnh đại diện
- Ngày sinh
- Giới tính
- Số điện thoại
- Email liên hệ
- Địa chỉ / tỉnh thành
- Vị trí mong muốn
- Mức lương mong muốn
- Kinh nghiệm
- Kỹ năng
- Học vấn
- Chứng chỉ
- Portfolio / GitHub / LinkedIn
- CV upload
- Giới thiệu bản thân
- Trạng thái tìm việc
- Hình thức làm việc mong muốn
- Nhu cầu hỗ trợ tiếp cận

## 2.2 Hồ sơ doanh nghiệp (Employer Profile)

Trường dữ liệu yêu cầu:

- Tên doanh nghiệp
- Logo
- Website
- Email liên hệ
- Số điện thoại
- Địa chỉ
- Quy mô công ty
- Lĩnh vực hoạt động
- Mã số thuế
- Người liên hệ tuyển dụng
- Chức vụ người liên hệ
- Mô tả doanh nghiệp
- Phúc lợi
- Văn hóa công ty
- Môi trường tiếp cận

## 2.3 Rule bắt buộc duyệt hồ sơ doanh nghiệp

- Employer chỉ được đăng tin khi `VerificationStatus = Approved`.
- Khi hồ sơ doanh nghiệp được tạo mới hoặc cập nhật:
  - chuyển trạng thái về `Pending`.
  - yêu cầu Admin duyệt lại.

---

## 3) Thiết kế theo Hexagonal Architecture

## 3.1 Domain layer

### Candidate aggregate

`CandidateProfile` mở rộng thuộc tính cho toàn bộ trường mới của hồ sơ ứng viên.

### Employer aggregate

`EmployerProfile` mở rộng thuộc tính hồ sơ doanh nghiệp và thêm vòng đời duyệt:

- `VerificationStatus` (`Pending`, `Approved`, `Rejected`)
- `VerifiedAtUtc`
- `VerificationNote`
- Hành vi domain:
  - `ApproveByAdmin(note)`
  - `RejectByAdmin(note)`
  - cập nhật profile -> tự về `Pending`

### Value/Enum mới

- `EmployerVerificationStatus` (enum domain).

## 3.2 Ports layer

### Inbound Ports

Mở rộng command/result cho Candidate/Employer profile:

- `CreateCandidateProfileCommand`, `UpdateCandidateProfileCommand`, `CandidateProfileResult`
- `CreateEmployerProfileCommand`, `UpdateEmployerProfileCommand`, `EmployerProfileResult`

Mở rộng `IEmployerProfileUseCase`:

- `GetPendingProfilesAsync()`
- `ApproveProfileAsync(...)`
- `RejectProfileAsync(...)`

### Outbound Ports

Mở rộng `IEmployerProfileRepository`:

- `GetAllAsync()` phục vụ danh sách duyệt của Admin.

## 3.3 Application layer

### UseCase cập nhật

- `CandidateProfileUseCase`:
  - map và cập nhật đầy đủ các trường profile ứng viên.
- `EmployerProfileUseCase`:
  - map đầy đủ trường doanh nghiệp.
  - xử lý các action duyệt/từ chối cho admin.

### Rule đăng tin

Trong `JobUseCase.PublishAsync`:

- kiểm tra Employer profile.
- chặn publish nếu chưa `Approved`.

### Mapper cập nhật

- `CandidateProfileMapper`
- `EmployerProfileMapper`

đảm bảo mapping đầy đủ domain -> result model.

## 3.4 Infrastructure layer

### Persistence Models

Mở rộng record:

- `CandidateProfileRecord`
- `EmployerProfileRecord`

thêm cột tương ứng tất cả trường mới + cột duyệt doanh nghiệp.

### Persistence Mapper

Mở rộng mapper 2 chiều:

- `CandidateProfilePersistenceMapper`
- `EmployerProfilePersistenceMapper`

### Repository

- `SqliteEmployerProfileRepository` implement `GetAllAsync`.
- `InMemoryEmployerProfileRepository` implement `GetAllAsync`.

### DbContext

`EnableVnDbContext`:

- cấu hình độ dài cột/constraints hợp lý cho các trường mới.
- cấu hình cột trạng thái duyệt và ghi chú duyệt.

> Lưu ý triển khai: cần tạo migration EF mới để đồng bộ DB schema.

## 3.5 Presentation layer

### Controllers

- `CandidateProfileController`: nhận/upload avatar + CV, bind trường mới.
- `EmployerProfileController`: nhận/upload logo, bind trường mới.
- `AdminDashboardController`: hiển thị danh sách hồ sơ chờ duyệt + action duyệt/từ chối.

### Views

Cập nhật các trang:

- Candidate:
  - `Create`, `Edit`, `Index`
- Employer:
  - `Create`, `Edit`, `_EmployerProfileForm`, `Index`, `Details`
- Admin:
  - dashboard section duyệt hồ sơ doanh nghiệp.

---

## 4) Tuân thủ DDD & SOLID

## 4.1 DDD

- Business rule quan trọng ở aggregate (đặc biệt vòng đời duyệt employer).
- Application use case orchestration, không chứa persistence chi tiết.
- Repository interface ở Ports, implementation ở Infrastructure.
- Controller chỉ là adapter nhận/trả HTTP.

## 4.2 SOLID

- **S**: Domain model xử lý quy tắc nghiệp vụ cốt lõi; controller chỉ điều phối request.
- **O**: Mở rộng profile bằng field mới và trạng thái duyệt mà không phá vỡ luồng cũ.
- **L**: Interface repositories/usecases thay thế được bởi bản InMemory/Sqlite.
- **I**: Ports chia nhỏ theo bounded context (candidate/employer/job).
- **D**: Application phụ thuộc abstraction (`I*Repository`, `I*UseCase`), không phụ thuộc implementation.

---

## 5) Quy trình duyệt hồ sơ doanh nghiệp (end-to-end)

1. Employer tạo/cập nhật hồ sơ.
2. Domain tự set `VerificationStatus = Pending`.
3. Admin vào dashboard, xem danh sách pending.
4. Admin chọn:
   - Duyệt -> `Approved`
   - Từ chối -> `Rejected` + note
5. Employer chỉ publish job khi profile là `Approved`.

---

## 6) Validation & dữ liệu

Đề xuất chuẩn hóa thêm (nếu chưa áp dụng hết):

- Email/phone format validation ở Presentation/Application.
- Max length theo cột DB và domain constraints.
- Chuẩn hóa enum/string controlled fields:
  - giới tính
  - trạng thái tìm việc
  - hình thức làm việc mong muốn
- Upload policy:
  - avatar/logo: image types + size limit.
  - CV: pdf/doc/docx + size limit.

---

## 7) Danh sách file liên quan cần đồng bộ

Nhóm chính:

- Domain:
  - `Domain/Candidates/CandidateProfile.cs`
  - `Domain/Employers/EmployerProfile.cs`
  - `Domain/Employers/EmployerVerificationStatus.cs`
- Ports models/inbound/outbound:
  - `Ports/Models/Candidates/*`
  - `Ports/Models/Employers/*`
  - `Ports/Inbound/IEmployerProfileUseCase.cs`
  - `Ports/Outbound/Repositories/IEmployerProfileRepository.cs`
- Application:
  - `Application/UseCases/CandidateProfileUseCase.cs`
  - `Application/UseCases/EmployerProfileUseCase.cs`
  - `Application/UseCases/JobUseCase.cs`
  - `Application/Mappers/*ProfileMapper.cs`
- Infrastructure:
  - `InfrastructureSqlite/PersistenceModels/*ProfileRecord.cs`
  - `InfrastructureSqlite/Mappers/*ProfilePersistenceMapper.cs`
  - `InfrastructureSqlite/Repositories/SqliteEmployerProfileRepository.cs`
  - `InfrastructureInMemory/Repositories/InMemoryEmployerProfileRepository.cs`
  - `InfrastructureSqlite/Persistence/EnableVnDbContext.cs`
  - `InfrastructureSqlite/Migrations/*` (migration mới)
- Presentation:
  - `ENABLEVN/Controllers/CandidateProfileController.cs`
  - `ENABLEVN/Controllers/EmployerProfileController.cs`
  - `ENABLEVN/Controllers/AdminDashboardController.cs`
  - `ENABLEVN/ViewModels/Dashboard/AdminDashboardViewModel.cs`
  - `ENABLEVN/Views/CandidateProfile/*`
  - `ENABLEVN/Views/EmployerProfile/*`
  - `ENABLEVN/Views/AdminDashboard/Index.cshtml`

---

## 8) Checklist kiểm thử

- Candidate tạo/sửa profile với toàn bộ trường mới.
- Employer tạo/sửa profile với toàn bộ trường mới.
- Upload avatar/logo/CV đúng/không đúng định dạng.
- Sau khi employer update profile -> trạng thái về pending.
- Admin duyệt/từ chối và ghi chú.
- Publish job:
  - profile chưa duyệt -> bị chặn.
  - profile đã duyệt -> publish thành công.
- View hiển thị đủ thông tin ở trang index/details.
- Regression:
  - chat, apply, dashboard, tìm ứng viên không bị ảnh hưởng.

---

## 9) Migration & rollout

1. Tạo migration mới cho các cột profile và trạng thái duyệt.
2. Apply migration trên môi trường dev/staging.
3. Seed hoặc xử lý dữ liệu cũ:
   - set mặc định verification cho hồ sơ doanh nghiệp hiện hữu (`Pending` hoặc policy mong muốn).
4. Smoke test publish job và admin review flow.

---

## 10) Kết luận

Thiết kế này mở rộng đầy đủ dữ liệu hồ sơ theo yêu cầu, giữ nguyên cấu trúc Hexa hiện tại, đặt business rule đúng layer theo DDD, và kiểm soát quyền/luồng duyệt rõ ràng theo SOLID.  
Đây là baseline phù hợp để tiếp tục mở rộng các tính năng trust/compliance (KYC, audit log, notification duyệt hồ sơ) trong các phase tiếp theo.

