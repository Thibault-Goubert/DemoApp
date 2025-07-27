using DemoApp.Application;
using DemoApp.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationServices();
        services.AddInfrastructure();
        services.AddLogging(logging => {
            logging.AddConsole();
            logging.AddDebug();
        });
    })
    .Build();

host.Run();
