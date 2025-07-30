using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using DemoApp.Domain.Interfaces;

namespace DemoApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
    private readonly PasswordService _passwordService;
    private readonly IUserRepository _repo;

    public AuthService(IUserRepository repo, PasswordService passwordService, IConfiguration config, ILoggerFactory loggerFactory)
    {
        _repo = repo;
        _passwordService = passwordService;
        _config = config;
        _logger = loggerFactory.CreateLogger<AuthService>();
    }

    public async Task<string> GenerateTokenAsync(string username, string password)
    {
        var user = await _repo.GetByUsernameAsync(username);
        if (user is null || !_passwordService.VerifyPassword(username, password, user.PasswordHash))
        {
            _logger.LogWarning($"Invalid login attempt for {username}");
            return string.Empty;
        }

        var key = _config["Jwt:Secret"];
        var issuer = _config["Jwt:Issuer"];

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username), // ID sujet
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID unique du token
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? throw new InvalidOperationException("JWT Secret is not configured")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer, issuer, claims,
            expires: DateTime.UtcNow.AddHours(1), signingCredentials: credentials);

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
