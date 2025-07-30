using DemoApp.Application;
using DemoApp.Domain.Interfaces;
using DemoApp.Infrastructure;
using DemoApp.Infrastructure.Data;
using DemoApp.Infrastructure.Repositories;
using DemoApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationServices();
        services.AddInfrastructure(context.Configuration);
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<PasswordService>();  
    })
    .Build();

host.Run();