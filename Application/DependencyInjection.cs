using Application.UseCases;
using Ports.Inbound;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    /// <summary>
    /// Đăng ký các UseCase của tầng Application.
    /// 
    /// Presentation sẽ gọi method này để đăng ký các Inbound Port implementation.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddEnableVNApplication(
            this IServiceCollection services
        )
        {
            services.AddScoped<IAuthUseCase, AuthUseCase>();

            services.AddScoped<IUserUseCase, UserUseCase>();

            services.AddScoped<IEmployerProfileUseCase, EmployerProfileUseCase>();

            services.AddScoped<ICandidateProfileUseCase, CandidateProfileUseCase>();

            services.AddScoped<IJobUseCase, JobUseCase>();

            services.AddScoped<IJobApplicationUseCase, JobApplicationUseCase>();

            services.AddScoped<ICatalogUseCase, CatalogUseCase>();

            return services;
        }
    }
}
