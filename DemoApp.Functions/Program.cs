using DemoApp.Application;
using DemoApp.Domain.Interfaces;
using DemoApp.Infrastructure;
using DemoApp.Infrastructure.Data;
using DemoApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationServices();
        services.AddInfrastructure();
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });
        services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnection")));
        services.AddScoped<IUserRepository, UserRepository>();
    })
    .Build();

host.Run();