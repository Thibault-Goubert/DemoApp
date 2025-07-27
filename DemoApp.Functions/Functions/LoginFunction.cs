using DemoApp.Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace DemoApp.Functions.Functions;

public class LoginFunction
{
    private readonly IAuthService _authService;

    public LoginFunction(IAuthService authService)
    {
        _authService = authService;
    }

    [Function("Login")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")] HttpRequestData req)
    {
        var data = await req.ReadFromJsonAsync<LoginRequest>();

        var token = await _authService.GenerateTokenAsync(data.Username, data.Password);

        var response = req.CreateResponse(token != null ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
        if (token != null)
        {
            await response.WriteAsJsonAsync(new { token });
        }

        return response;
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
