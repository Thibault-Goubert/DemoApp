using DemoApp.Application.Services;
using DemoApp.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApp.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IGreetingService, GreetingService>();
        return services;
    }
}
