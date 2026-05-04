using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Ports.Inbound;

namespace Application;

/// <summary>
/// Đăng ký toàn bộ UseCase của tầng Application.
/// 
/// Đây là Inbound Adapter implementation:
/// - Presentation chỉ gọi interface trong Ports.Inbound
/// - Application implement các interface đó
/// - Không phụ thuộc trực tiếp vào Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddEnableVNApplication(this IServiceCollection services)
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