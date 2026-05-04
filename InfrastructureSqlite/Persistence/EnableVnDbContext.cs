using InfrastructureSqlite.PersistenceModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureSqlite.Persistence
{
    public sealed class EnableVnDbContext : DbContext
    {
        public EnableVnDbContext(DbContextOptions<EnableVnDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserRecord> Users => Set<UserRecord>();

        public DbSet<EmployerProfileRecord> EmployerProfiles => Set<EmployerProfileRecord>();

        public DbSet<CandidateProfileRecord> CandidateProfiles => Set<CandidateProfileRecord>();

        public DbSet<JobPostRecord> JobPosts => Set<JobPostRecord>();

        public DbSet<JobApplicationRecord> JobApplications => Set<JobApplicationRecord>();

        public DbSet<ApplicationStatusHistoryRecord> ApplicationStatusHistories => Set<ApplicationStatusHistoryRecord>();

        public DbSet<DisabilityTypeRecord> DisabilityTypes => Set<DisabilityTypeRecord>();

        public DbSet<AssistiveDeviceRecord> AssistiveDevices => Set<AssistiveDeviceRecord>();

        public DbSet<JobCategoryRecord> JobCategories => Set<JobCategoryRecord>();
        public DbSet<NotificationRecord> Notifications => Set<NotificationRecord>();

        public DbSet<ApplicationChatMessageRecord> ApplicationChatMessages => Set<ApplicationChatMessageRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUsers(modelBuilder);
            ConfigureEmployerProfiles(modelBuilder);
            ConfigureCandidateProfiles(modelBuilder);
            ConfigureJobPosts(modelBuilder);
            ConfigureJobApplications(modelBuilder);
            ConfigureApplicationStatusHistories(modelBuilder);
            ConfigureDisabilityTypes(modelBuilder);
            ConfigureAssistiveDevices(modelBuilder);
            ConfigureJobCategories(modelBuilder);
            modelBuilder.Entity<NotificationRecord>(entity =>
            {
                entity.ToTable("Notifications"); // Tên bảng trong SQLite.

                entity.HasKey(x => x.Id); // Khóa chính.

                entity.Property(x => x.UserId)
                    .IsRequired(); // Bắt buộc có người nhận.

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200); // Tiêu đề không nên quá dài.

                entity.Property(x => x.Message)
                    .IsRequired()
                    .HasMaxLength(1000); // Nội dung giới hạn để tránh quá tải UI.

                entity.Property(x => x.Type)
                    .IsRequired()
                    .HasMaxLength(50); // Enum lưu string.

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(50); // Unread/Read.

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.HasIndex(x => x.UserId);
                // Tối ưu truy vấn danh sách thông báo theo user.

                entity.HasIndex(x => new { x.UserId, x.Status });
                // Tối ưu đếm thông báo chưa đọc.
            });

            modelBuilder.Entity<ApplicationChatMessageRecord>(entity =>
            {
                entity.ToTable("ApplicationChatMessages");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.JobApplicationId).IsRequired();

                entity.Property(x => x.SenderUserId).IsRequired();

                entity.Property(x => x.Body)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(x => x.ModerationOutcome)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.ModerationReasonVi).HasMaxLength(500);

                entity.Property(x => x.SentAtUtc).IsRequired();

                entity.HasIndex(x => x.JobApplicationId);
            });
        }

        private static void ConfigureUsers(ModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<UserRecord>();

            user.ToTable("Users");

            user.HasKey(x => x.Id);

            user.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            user.HasIndex(x => x.Email)
                .IsUnique();

            user.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            user.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(50);

            user.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);
        }

        private static void ConfigureEmployerProfiles(ModelBuilder modelBuilder)
        {
            var employer = modelBuilder.Entity<EmployerProfileRecord>();

            employer.ToTable("EmployerProfiles");

            employer.HasKey(x => x.Id);

            employer.Property(x => x.UserId).IsRequired();

            employer.HasIndex(x => x.UserId).IsUnique();

            employer.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            employer.Property(x => x.Description).HasMaxLength(2000);

            employer.Property(x => x.WebsiteUrl).HasMaxLength(500);
        }

        private static void ConfigureCandidateProfiles(ModelBuilder modelBuilder)
        {
            var candidate = modelBuilder.Entity<CandidateProfileRecord>();

            candidate.ToTable("CandidateProfiles");

            candidate.HasKey(x => x.Id);

            candidate.Property(x => x.UserId).IsRequired();

            candidate.HasIndex(x => x.UserId).IsUnique();

            candidate.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            candidate.Property(x => x.Bio).HasMaxLength(2000);

            candidate.Property(x => x.CvUrl).HasMaxLength(500);

            candidate.Property(x => x.DisabilityDescription).HasMaxLength(1000);
        }

        private static void ConfigureJobPosts(ModelBuilder modelBuilder)
        {
            var job = modelBuilder.Entity<JobPostRecord>();

            job.ToTable("JobPosts");

            job.HasKey(x => x.Id);

            job.Property(x => x.EmployerId).IsRequired();

            job.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            job.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(5000);

            job.Property(x => x.Requirement)
                .IsRequired()
                .HasMaxLength(5000);

            job.Property(x => x.WorkMode)
                .IsRequired()
                .HasMaxLength(50);

            job.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            job.Property(x => x.AccessibilityAdditionalInfo).HasMaxLength(1000);

            job.HasIndex(x => x.EmployerId);

            job.HasIndex(x => x.Status);

            job.HasIndex(x => new { x.Status, x.EmployerId });
        }

        private static void ConfigureJobApplications(ModelBuilder modelBuilder)
        {
            var app = modelBuilder.Entity<JobApplicationRecord>();

            app.ToTable("JobApplications");

            app.HasKey(x => x.Id);

            app.Property(x => x.JobId).IsRequired();

            app.Property(x => x.CandidateId).IsRequired();

            app.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            app.Property(x => x.CoverLetter).HasMaxLength(2000);

            app.Property(x => x.CvUrl).HasMaxLength(500);

            // Unique constraint: Một candidate chỉ có thể nộp một lần cho một job
            app.HasIndex(x => new { x.JobId, x.CandidateId }).IsUnique();

            app.HasIndex(x => x.JobId);

            app.HasIndex(x => x.CandidateId);
        }

        private static void ConfigureApplicationStatusHistories(ModelBuilder modelBuilder)
        {
            var history = modelBuilder.Entity<ApplicationStatusHistoryRecord>();

            history.ToTable("ApplicationStatusHistories");

            history.HasKey(x => x.Id);

            history.Property(x => x.JobApplicationId).IsRequired();

            history.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            history.Property(x => x.Note).HasMaxLength(500);

            history.HasIndex(x => x.JobApplicationId);
        }

        private static void ConfigureDisabilityTypes(ModelBuilder modelBuilder)
        {
            var disability = modelBuilder.Entity<DisabilityTypeRecord>();

            disability.ToTable("DisabilityTypes");

            disability.HasKey(x => x.Id);

            disability.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            disability.Property(x => x.Description).HasMaxLength(1000);

            disability.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            disability.HasIndex(x => x.Status);
        }

        private static void ConfigureAssistiveDevices(ModelBuilder modelBuilder)
        {
            var device = modelBuilder.Entity<AssistiveDeviceRecord>();

            device.ToTable("AssistiveDevices");

            device.HasKey(x => x.Id);

            device.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            device.Property(x => x.Description).HasMaxLength(1000);

            device.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            device.HasIndex(x => x.Status);
        }

        private static void ConfigureJobCategories(ModelBuilder modelBuilder)
        {
            var category = modelBuilder.Entity<JobCategoryRecord>();

            category.ToTable("JobCategories");

            category.HasKey(x => x.Id);

            category.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            category.Property(x => x.Description).HasMaxLength(1000);

            category.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50);

            category.HasIndex(x => x.Status);
        }

    }
}
