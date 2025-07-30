using DemoApp.Domain.Entities;
using DemoApp.Domain.Interfaces;
using DemoApp.Infrastructure.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace DemoApp.Functions.Functions.Auth;

public class RegisterFunction
{
    private readonly IUserRepository _repo;
    private readonly PasswordService _passwordService;

    public RegisterFunction(IUserRepository repo, PasswordService passwordService)
    {
        _repo = repo;
        _passwordService = passwordService;
    }

    [Function("Register")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "register")] HttpRequestData req)
    {
        var data = await req.ReadFromJsonAsync<UserRegisterRequest>();
        if (data is null || string.IsNullOrWhiteSpace(data.Username) || string.IsNullOrWhiteSpace(data.Password))
            return req.CreateResponse(HttpStatusCode.BadRequest);

        // Vérifier si user existe déjà
        var existing = await _repo.GetByUsernameAsync(data.Username);
        if (existing != null)
        {
            var conflict = req.CreateResponse(HttpStatusCode.Conflict);
            await conflict.WriteStringAsync("Username already exists");
            return conflict;
        }

        var user = new User
        {
            Username = data.Username,
            PasswordHash = _passwordService.HashPassword(data.Username, data.Password)
        };

        await _repo.AddAsync(user);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteStringAsync("User created");
        return response;
    }
}

public record UserRegisterRequest(string Username, string Password);
