using System.Net;
using DemoApp.Domain.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DemoApp.Functions.Functions;

public class HelloWorldFunction
{
    private readonly IGreetingService _greetingService;
    private readonly ILogger _logger;

    public HelloWorldFunction(IGreetingService greetingService, ILoggerFactory loggerFactory)
    {
        _greetingService = greetingService;
        _logger = loggerFactory.CreateLogger<HelloWorldFunction>();
    }

    [Function("HelloWorld")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("HelloWorld function triggered.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        var message = _greetingService.GetGreeting("Thibault");
        response.WriteString(message);

        return response;
    }
}
