using DemoApp.Application.Services;
using DemoApp.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApp.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
