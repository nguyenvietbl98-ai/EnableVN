using InfrastructureInMemory.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Ports.Outbound.Repositories;
using Ports.Outbound.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureInMemory
{
    /// <summary>
    /// Đăng ký toàn bộ implementation InMemory vào DI Container.
    /// 
    /// Presentation hoặc Test project có thể gọi:
    /// services.AddEnableVNInMemoryInfrastructure();
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddEnableVNInMemoryInfrastructure(
            this IServiceCollection services
        )
        {
            // =========================
            // Repositories
            // =========================
            //
            // Dùng Singleton để dữ liệu RAM được giữ lại trong suốt vòng đời app.
            // Nếu dùng Scoped/Transient thì dữ liệu sẽ dễ bị reset theo request/object.

            services.AddSingleton<IUserRepository, InMemoryUserRepository>();

            services.AddSingleton<IEmployerProfileRepository, InMemoryEmployerProfileRepository>();

            services.AddSingleton<ICandidateProfileRepository, InMemoryCandidateProfileRepository>();

            services.AddSingleton<IJobRepository, InMemoryJobRepository>();

            services.AddSingleton<IJobApplicationRepository, InMemoryJobApplicationRepository>();

            services.AddSingleton<IDisabilityTypeRepository, InMemoryDisabilityTypeRepository>();

            services.AddSingleton<IAssistiveDeviceRepository, InMemoryAssistiveDeviceRepository>();

            services.AddSingleton<IJobCategoryRepository, InMemoryJobCategoryRepository>();

            // =========================
            // Services
            // =========================

            services.AddSingleton<ICurrentUserService, InMemoryCurrentUserService>();

            services.AddSingleton<IPasswordHasher, SimplePasswordHasher>();

            services.AddSingleton<ITokenService, SimpleTokenService>();

            services.AddSingleton<IDomainEventDispatcher, InMemoryDomainEventDispatcher>();

            return services;
        }
    }
}
