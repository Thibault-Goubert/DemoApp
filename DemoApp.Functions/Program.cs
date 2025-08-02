using DemoApp.Application.Extensions;
using DemoApp.Domain.Interfaces.Repositories;
using DemoApp.Functions.Functions.Middleware;
using DemoApp.Infrastructure;
using DemoApp.Infrastructure.Extensions;
using DemoApp.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.UseMiddleware<JwtMiddleware>();
    })
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationServices();
        services.AddApplicationMappers();
        services.AddInfrastructure(context.Configuration);
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });
        services.AddScoped<IUserRepository, UserRepository>();
    })
    .Build();

host.Run();