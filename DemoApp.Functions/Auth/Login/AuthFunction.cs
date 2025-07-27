using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class AuthFunction
{
    private readonly IAuthService _authService;

    public AuthFunction(IAuthService authService)
    {
        _authService = authService;
    }

    [Function("Login")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")] HttpRequestData req)
    {
        var body = await JsonSerializer.DeserializeAsync<LoginRequest>(req.Body);

        var token = await _authService.AuthenticateAsync(body.Username, body.Password);

        var response = req.CreateResponse(token is null ? HttpStatusCode.Unauthorized : HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { token });

        return response;
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
