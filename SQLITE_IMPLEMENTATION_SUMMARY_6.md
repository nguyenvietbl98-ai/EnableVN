# EnableVN SQLite Implementation Summary

**Ngày:** 05/05/2026  
**Status:** ✅ Hoàn thành  
**Build:** ✅ Success  
**Database:** ✅ Created (enablevn.db)

---

## 📋 Tổng Quan

Hoàn thành triển khai **InfrastructureSqlite** để thay thế InfrastructureInMemory, lưu dữ liệu thực bằng SQLite đồng thời giữ đúng **Hexagonal Architecture + Domain-Driven Design**.

---

## 📁 Cấu Trúc Files Tạo Mới

### 1. PersistenceModels (8 files)
Các model EF Core đại diện cho bảng database, tách riêng khỏi Domain Entities.

```
InfrastructureSqlite/PersistenceModels/
├── UserRecord.cs                           ✅ Đã có
├── EmployerProfileRecord.cs                🆕 Mới tạo
├── CandidateProfileRecord.cs               🆕 Mới tạo
├── JobPostRecord.cs                        🆕 Mới tạo
├── JobApplicationRecord.cs                 🆕 Mới tạo
├── ApplicationStatusHistoryRecord.cs       🆕 Mới tạo
├── DisabilityTypeRecord.cs                 🆕 Mới tạo
├── AssistiveDeviceRecord.cs                🆕 Mới tạo
└── JobCategoryRecord.cs                    🆕 Mới tạo
```

**Đặc điểm:**
- Enum lưu dạng string (WorkMode, Status, Role)
- ValueObjects flatten thành primitive types (Email → string, FullName → string)
- Complex types split thành multiple columns (InclusiveWorkplaceInfo → 5 bool, DisabilityInfo → 3 columns)

### 2. Mappers (6 files)
Chuyển đổi hai chiều giữa Domain Entities ↔ PersistenceModels.

```
InfrastructureSqlite/Mappers/
├── UserPersistenceMapper.cs                ✅ Đã có
├── EmployerProfilePersistenceMapper.cs     🆕 Mới tạo
├── CandidateProfilePersistenceMapper.cs    🆕 Mới tạo
├── JobPostPersistenceMapper.cs             🆕 Mới tạo
├── JobApplicationPersistenceMapper.cs      🆕 Mới tạo
├── ApplicationStatusHistoryPersistenceMapper.cs 🆕 Mới tạo
└── CatalogPersistenceMapper.cs             🆕 Mới tạo
```

**Pattern cho mỗi Mapper:**
```csharp
ToRecord(domain)           // Domain → Record
ToDomain(record)           // Record → Domain
UpdateRecord(record, domain) // Update existing record
```

### 3. Repositories (7 files)
Implement các Outbound Port interfaces từ Ports/, sử dụng EF Core DbContext.

```
InfrastructureSqlite/Repositories/
├── SqliteUserRepository.cs                 ✅ Đã có
├── SqliteEmployerProfileRepository.cs      🆕 Mới tạo
├── SqliteCandidateProfileRepository.cs     🆕 Mới tạo
├── SqliteJobRepository.cs                  🆕 Mới tạo
├── SqliteJobApplicationRepository.cs       🆕 Mới tạo
├── SqliteDisabilityTypeRepository.cs       🆕 Mới tạo
├── SqliteAssistiveDeviceRepository.cs      🆕 Mới tạo
└── SqliteJobCategoryRepository.cs          🆕 Mới tạo
```

**Lifecycle:**
- Scoped (not Singleton) - đi kèm DbContext Scoped lifecycle
- AsNoTracking() cho queries chỉ-đọc
- SaveChangesAsync() sau mỗi insert/update

### 4. Persistence (2 files)
DbContext + Factory cho EF Core migrations.

```
InfrastructureSqlite/Persistence/
├── EnableVnDbContext.cs                    🔄 Sửa (thêm 8 DbSets + configurations)
└── EnableVnDbContextFactory.cs             🔄 Sửa (implement IDesignTimeDbContextFactory)
```

**DbSets (9 bảng):**
1. Users
2. EmployerProfiles
3. CandidateProfiles
4. JobPosts
5. JobApplications
6. ApplicationStatusHistories
7. DisabilityTypes
8. AssistiveDevices
9. JobCategories

### 5. SeedData (2 files)
Automatic seed dữ liệu mặc định khi start Development environment.

```
InfrastructureSqlite/SeedData/
├── SqliteAdminSeeder.cs       🆕 Mới tạo
└── SqliteCatalogSeeder.cs     🆕 Mới tạo
```

**Tạo:**
- Admin: admin@enablevn.local / Admin@123
- DisabilityTypes: 7 loại
- AssistiveDevices: 7 thiết bị
- JobCategories: 8 ngành

### 6. DependencyInjection (1 file)
Extension method đăng ký tất cả SQLite repositories + DbContext vào DI.

```csharp
services.AddEnableVNSqliteInfrastructure(configuration)
```

---

## 🔧 Domain Extensions (Thêm Restore Methods)

Thêm `static Restore()` methods vào 5 Domain Entities để dựng lại aggregate từ database:

| Entity | File | Method |
|--------|------|--------|
| User | Domain/Users/User.cs | ✅ Đã có |
| EmployerProfile | Domain/Employers/EmployerProfile.cs | 🆕 Thêm |
| CandidateProfile | Domain/Candidates/CandidateProfile.cs | 🆕 Thêm |
| JobPost | Domain/Jobs/JobPost.cs | 🆕 Thêm |
| JobApplication | Domain/Applications/JobApplication.cs | 🆕 Thêm |
| ApplicationStatusHistory | Domain/Applications/ApplicationStatusHistory.cs | 🆕 Thêm |
| DisabilityType | Domain/Catalogs/DisabilityType.cs | ✅ Đã có |
| AssistiveDevice | Domain/Catalogs/AssistiveDevice.cs | ✅ Đã có |
| JobCategory | Domain/Catalogs/JobCategory.cs | ✅ Đã có |

**Đặc tính của Restore():**
- Không raise DomainEvents (chỉ dùng để restore từ DB)
- Validate Id, required fields
- Private constructor + public static factory pattern

---

## 📊 Database Schema

### Unique Constraints
```
Users.Email                                    UNIQUE
EmployerProfiles.UserId                        UNIQUE
CandidateProfiles.UserId                       UNIQUE
JobApplications.(JobId, CandidateId)           UNIQUE
```

### Indexes
```
Users.Email                                    
JobPosts.Status                                
JobPosts.EmployerId                            
JobPosts.(Status, EmployerId)                  
JobApplications.JobId                          
JobApplications.CandidateId                    
ApplicationStatusHistories.JobApplicationId    
DisabilityTypes.Status                         
AssistiveDevices.Status                        
JobCategories.Status                           
```

### Column Mappings

#### Users
- Id: Guid (PK)
- Email: string [255] (Unique)
- PasswordHash: string [500]
- Role: string [50] (Enum as string)
- Status: string [50] (Enum as string)

#### EmployerProfiles
- Id: Guid (PK)
- UserId: Guid (FK, Unique)
- CompanyName: string [200]
- Description: string [2000] nullable
- WebsiteUrl: string [500] nullable
- HasWheelchairAccess: bool
- HasAccessibleRestroom: bool
- SupportsFlexibleWorkingTime: bool
- SupportsRemoteWork: bool
- ProvidesAssistiveDevices: bool

#### CandidateProfiles
- Id: Guid (PK)
- UserId: Guid (FK, Unique)
- FullName: string [100]
- Bio: string [2000] nullable
- CvUrl: string [500] nullable
- DisabilityTypeId: Guid nullable
- DisabilityDescription: string [1000] nullable
- IsDisabilityVisibleToEmployer: bool
- IsPublicProfile: bool

#### JobPosts
- Id: Guid (PK)
- EmployerId: Guid
- Title: string [200]
- Description: string [5000]
- Requirement: string [5000]
- WorkMode: string [50] (Enum as string)
- MinSalary: decimal nullable
- MaxSalary: decimal nullable
- SupportsWheelchairAccess: bool
- SupportsRemoteWork: bool
- SupportsFlexibleTime: bool
- ProvidesAssistiveDevices: bool
- AccessibilityAdditionalInfo: string [1000] nullable
- Status: string [50] (Enum as string)
- CreatedAt: DateTime
- PublishedAt: DateTime nullable
- ClosedAt: DateTime nullable

#### JobApplications
- Id: Guid (PK)
- JobId: Guid
- CandidateId: Guid
- CoverLetter: string [2000] nullable
- CvUrl: string [500] nullable
- Status: string [50] (Enum as string)
- SubmittedAt: DateTime
- Unique: (JobId, CandidateId)

#### ApplicationStatusHistories
- Id: Guid (PK)
- JobApplicationId: Guid (FK)
- Status: string [50] (Enum as string)
- Note: string [500] nullable
- ChangedAt: DateTime

#### DisabilityTypes, AssistiveDevices, JobCategories
- Id: Guid (PK)
- Name: string [100]
- Description: string [1000] nullable
- Status: string [50] (Enum as string, Indexed)

---

## 🚀 DI Configuration

### Program.cs Setup

```csharp
// 1. Đăng ký Application UseCases
builder.Services.AddEnableVNApplication();

// 2. Đăng ký InMemory services (mà chưa có SQLite version)
builder.Services.AddEnableVNInMemoryInfrastructure();

// 3. Ghi đè repositories bằng SQLite
builder.Services.AddEnableVNSqliteInfrastructure(builder.Configuration);

// 4. Ghi đè ICurrentUserService cho MVC Session
builder.Services.AddScoped<ICurrentUserService, SessionCurrentUserService>();

// 5. Seed data khi Development
if (app.Environment.IsDevelopment())
{
    await SqliteAdminSeeder.SeedAsync(app.Services);
    await SqliteCatalogSeeder.SeedAsync(app.Services);
}
```

### Service Lifecycle

| Service | Lifecycle | Tùy Chọn |
|---------|-----------|---------|
| DbContext | Scoped | ✓ SQLite |
| Repositories | Scoped | ✓ SQLite |
| PasswordHasher | Singleton | InMemory (SimplePasswordHasher) |
| TokenService | Singleton | InMemory (SimpleTokenService) |
| DomainEventDispatcher | Singleton | InMemory (tạm thời) |
| CurrentUserService | Scoped | Presentation (SessionCurrentUserService) |

---

## 📦 EF Core Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "EnableVnSqlite": "Data Source=enablevn.db"
  }
}
```

### DbContextFactory

```csharp
public sealed class EnableVnDbContextFactory : 
    IDesignTimeDbContextFactory<EnableVnDbContext>
{
    public EnableVnDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EnableVnDbContext>();
        optionsBuilder.UseSqlite("Data Source=enablevn.db");
        return new EnableVnDbContext(optionsBuilder.Options);
    }
}
```

**Dùng cho:** `dotnet ef migrations add/remove/update`

---

## 🗄️ Migrations

### Created

| Migration ID | Name | Status |
|--------------|------|--------|
| 20260504170824 | InitialSqliteCreate | ✅ Applied |

### Location
```
InfrastructureSqlite/Migrations/
├── 20260504170824_InitialSqliteCreate.cs
├── 20260504170824_InitialSqliteCreate.Designer.cs
└── EnableVnDbContextModelSnapshot.cs
```

### Database File
```
Location: ENABLEVN/enablevn.db
Size: ~151 KB
Status: ✅ Created
```

---

## ✅ Architecture Compliance Checklist

| Rule | Status | Ghi chú |
|------|--------|---------|
| Domain không phụ thuộc EF | ✅ | Không có [Table], [Key], [Column] attributes |
| Domain không phụ thuộc Database | ✅ | Không có DbContext references |
| Application không gọi DbContext | ✅ | Chỉ dùng Repository interfaces |
| Presentation không gọi Repository | ✅ | Chỉ gọi Application UseCases |
| Tất cả interfaces ở Ports | ✅ | 8 Repositories, 6 Services |
| InfrastructureSqlite implement Outbound Ports | ✅ | 8 Repository implementations |
| PersistenceModels tách riêng | ✅ | Không contaminate Domain |
| Mapper bidirectional | ✅ | ToRecord/ToDomain/UpdateRecord |
| Restore() không trigger events | ✅ | Chỉ dùng để rebuild aggregate |
| ValueObjects xử lý đúng | ✅ | Email→string, WorkMode→enum string |

---

## 🌱 Seed Data

### Admin Mặc Định

| Field | Value |
|-------|-------|
| Email | admin@enablevn.local |
| Password | Admin@123 |
| Role | Admin |
| Status | Active |

**Khi nào seed:** Development environment, lần đầu chạy  
**Logic:** `ExistsByEmailAsync()` → nếu có rồi thì skip

### Catalogs

#### DisabilityTypes (7)
1. Khiếm thị
2. Khiếm thính
3. Khiếm động
4. Khiếm khôn
5. Tự kỷ
6. Rối loạn tâm lý
7. Bệnh mãn tính

#### AssistiveDevices (7)
1. Màn hình nói
2. Bàn phím đặc biệt
3. Chuột đặc biệt
4. Thiết bị trợ thính
5. Bảng chữ cái mở rộng
6. Tựa gối
7. Xe lăn

#### JobCategories (8)
1. Công nghệ thông tin
2. Bán hàng
3. Kế toán
4. Nhân sự
5. Chăm sóc khách hàng
6. Thiết kế
7. Quản lý dự án
8. Giáo dục

---

## 🔍 Key Implementation Details

### ValueObject Mapping Strategy

| ValueObject | Storage | Reason |
|-------------|---------|--------|
| Email | string [255] | Email.Value → string |
| FullName | string [100] | FullName.Value → string |
| JobTitle | string [200] | JobTitle.Value → string |
| CompanyName | string [200] | CompanyName.Value → string |
| SalaryRange | decimal? min/max | 2 nullable columns |
| WorkMode | string [50] enum | Onsite/Remote/Hybrid → string |
| InclusiveWorkplaceInfo | 5 bool columns | Flatten 5 properties |
| JobAccessibilityInfo | 4 bool + string | Flatten 4 bools + AdditionalInfo |
| DisabilityInfo | 3 columns | DisabilityTypeId, Description, IsVisible |

### Collection Handling

| Collection | Storage | Why |
|-----------|---------|-----|
| JobApplication.StatusHistories | ApplicationStatusHistories table | 1-many relationship, easier filtering |

### Search Implementation

#### SqliteJobRepository.SearchPublishedJobsAsync

```csharp
// Filters
- keyword: case-insensitive search Title/Description/Requirement
- workMode: exact enum match
- supportsWheelchairAccess: bool exact match
- supportsRemoteWork: bool exact match
- supportsFlexibleTime: bool exact match
- providesAssistiveDevices: bool exact match

// null = không filter (optional search)
// true = bắt buộc có support
```

#### Unique Application Check

```csharp
ExistsByJobIdAndCandidateIdAsync(jobId, candidateId)

// Prevent: Candidate submit duplicate ứng tuyển cho cùng 1 job
// Storage: Unique index (JobId, CandidateId)
```

---

## 🧪 Build & Test Status

### Build
```
✅ dotnet build
   - No errors
   - 4 package warnings (System.Security.Cryptography.Xml)
   - Build time: ~7.78s
```

### Migration
```
✅ dotnet ef migrations add InitialSqliteCreate
   - Done. To undo: ef migrations remove
   
✅ dotnet ef database update
   - Applying migration '20260504170824_InitialSqliteCreate'
   - Done.
```

### Database
```
✅ enablevn.db created
   - Location: ENABLEVN/enablevn.db
   - Size: 151,552 bytes
   - Schema: 9 tables
```

---

## 🔄 Phiên Bản So Sánh: InMemory vs SQLite

| Aspect | InMemory | SQLite |
|--------|----------|--------|
| Storage | RAM (List<T>) | Disk file |
| Lifecycle | Singleton | Scoped (DbContext) |
| Persistence | Session/Lost on restart | ✅ Persistent |
| Data Reset | Manual clear | Manual delete .db file |
| Query Performance | O(n) LINQ-to-Objects | O(1) indexed |
| Concurrent Access | No locking | SQLite handles |
| Schema Migration | No | ✅ EF Migrations |
| Test Environment | ✅ Good | ✅ Good |
| Production | ❌ Not suitable | ✅ Suitable for MVP |

---

## 📝 Notes & Known Issues

### None
✅ Không có issue nào. Build thành công, DB tạo ok, kiến trúc sạch.

### Future Improvements (ngoài scope MVP)
- [ ] Add logging to repositories
- [ ] Add transaction support (SaveChangesAsync context.Database.BeginTransactionAsync)
- [ ] Add soft delete (ShadowProperty IsDeleted)
- [ ] Async enumeration optimization
- [ ] Connection pool tuning for SQLite
- [ ] Backup automation

---

## 📋 Checklist Hoàn Thành

- [x] PersistenceModels (8 files)
- [x] Mappers (6 files)
- [x] Repositories (7 files)
- [x] DbContext + Factory
- [x] Seeders (2 files)
- [x] DependencyInjection registration
- [x] Domain Restore() methods (5 entities)
- [x] Program.cs configuration
- [x] Migrations created & applied
- [x] Database file generated
- [x] Build succeeded
- [x] Architecture compliance verified
- [x] Documentation written

---

## 🚀 Ready for Next Steps

1. **Test chạy web app** - `dotnet run --project ENABLEVN`
2. **Test login admin** - admin@enablevn.local / Admin@123
3. **Test Employer flow** - Register → Profile → Job post → Publish
4. **Test Candidate flow** - Register → Profile → Search jobs → Apply
5. **Test persistence** - Restart app → Data still there
6. **Test search filters** - Filter accessibility requirements

---

**Ngày hoàn thành:** 05/05/2026  
**Tạo bởi:** GitHub Copilot  
**Status:** ✅ DONE - Ready for manual testing
