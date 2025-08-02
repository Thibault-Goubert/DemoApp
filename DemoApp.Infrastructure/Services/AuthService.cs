using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using DemoApp.Application.Services;
using DemoApp.Domain.Interfaces.Repositories;
using DemoApp.Domain.Entities;

namespace DemoApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IPasswordHasher passwordHasher, IConfiguration config, ILoggerFactory loggerFactory, IUserService userService, IUserRepository userRepository)
    {
        _passwordHasher = passwordHasher;
        _config = config;
        _logger = loggerFactory.CreateLogger<AuthService>();
        _userRepository = userRepository;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        if (await _userRepository.GetByUsernameAsync(username) != null)
        {
            _logger.LogWarning("Tentative d'enregistrement avec un nom d'utilisateur déjà pris : {Username}", username);
            return false;
        }

        await _userRepository.AddAsync(new User(username, _passwordHasher.Hash(password)));
        return true;
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        { 
            return false;
        }
        
        return _passwordHasher.Verify(password, user.PasswordHash);
    }

    public async Task<string> GenerateTokenAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
        { 
            _logger.LogWarning($"Invalid login attempt for {username}");
            return string.Empty;
        }

        var key = _config["Jwt:Secret"];
        var issuer = _config["Jwt:Issuer"];

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? throw new InvalidOperationException("JWT Secret is not configured")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer, issuer, claims,
            expires: DateTime.UtcNow.AddHours(1), signingCredentials: credentials);

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
