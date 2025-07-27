using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace DemoApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    public AuthService(IConfiguration config, ILoggerFactory loggerFactory)
    {
        _config = config;
        _logger = loggerFactory.CreateLogger<AuthService>();
    }

    public Task<string> GenerateTokenAsync(string username, string password)
    {
        // ⚠️ Simulation — ici on simule un login hardcodé
        if (username != "admin" || password != "pass")
            return Task.FromResult(string.Empty);

        var key = _config["Jwt:Secret"];
        var issuer = _config["Jwt:Issuer"];

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? throw new InvalidOperationException("JWT Secret is not configured")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer, issuer, claims,
            expires: DateTime.UtcNow.AddHours(1), signingCredentials: credentials);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
