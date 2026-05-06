using InfrastructureSqlite.Persistence;
using InfrastructureSqlite.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ports.Outbound.Repositories;
using System;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureSqlite
{
    /// <summary>
    /// Đăng ký tất cả SQLite implementation vào DI Container.
    /// 
    /// Presentation có thể gọi:
    /// services.AddEnableVNSqliteInfrastructure(configuration);
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddEnableVNSqliteInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("EnableVnSqlite");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "Data Source=enablevn.db";
            }

            // Đăng ký DbContext
            services.AddDbContext<EnableVnDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            // =========================
            // Repositories
            // =========================
            // Dùng Scoped vì EF Core DbContext là Scoped.

            services.AddScoped<IUserRepository, SqliteUserRepository>();

            services.AddScoped<IEmployerProfileRepository, SqliteEmployerProfileRepository>();

            services.AddScoped<ICandidateProfileRepository, SqliteCandidateProfileRepository>();

            services.AddScoped<IJobRepository, SqliteJobRepository>();

            services.AddScoped<IJobApplicationRepository, SqliteJobApplicationRepository>();

            services.AddScoped<IDisabilityTypeRepository, SqliteDisabilityTypeRepository>();

            services.AddScoped<IAssistiveDeviceRepository, SqliteAssistiveDeviceRepository>();

            services.AddScoped<IJobCategoryRepository, SqliteJobCategoryRepository>();
            services.AddScoped<INotificationRepository, SqliteNotificationRepository>();

            services.AddScoped<IApplicationChatRepository, SqliteApplicationChatRepository>();
            services.AddScoped<IInterviewScheduleRepository, SqliteInterviewScheduleRepository>();

            return services;
        }
    }
}
