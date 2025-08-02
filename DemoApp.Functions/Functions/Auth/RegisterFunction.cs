using DemoApp.Application.DTO;
using DemoApp.Application.Services;
using DemoApp.Domain.Interfaces.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DemoApp.Functions.Functions.Auth;

public class RegisterFunction
{
    private readonly IUserService _userService;
    private readonly ILogger _logger;
    private readonly IAuthService _authService;

    public RegisterFunction(ILoggerFactory loggerFactory, IUserService userService, IAuthService authService)
    {
        _logger = loggerFactory.CreateLogger<RegisterFunction>();
        _userService = userService;
        _authService = authService;
    }

    [Function("Register")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "register")] HttpRequestData req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonSerializer.Deserialize<LoginRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (data == null || string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Password))
            return req.CreateResponse(HttpStatusCode.BadRequest);

        if (await _userService.CheckUserExistsAsync(data.Username))
            return req.CreateResponse(HttpStatusCode.Conflict);

        await _authService.RegisterAsync(data.Username, data.Password);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteStringAsync("User registered successfully");
        return response;
    }
}
