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
       // Solution : comparaison insensible à la casse
        if (!string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase) || 
            !string.Equals(password, "pass", StringComparison.Ordinal))
        {
            _logger.LogWarning($"Invalid login attempt for user: {username}");
            return Task.FromResult(string.Empty);
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

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
