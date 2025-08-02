using System.Text;
using AutoMapper;
using DemoApp.Application.DTO;
using DemoApp.Application.Services;
using DemoApp.Domain.Entities;
using DemoApp.Domain.Interfaces.Repositories;
using DemoApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace DemoApp.Infrastructure.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig = new();
    private readonly Mock<ILoggerFactory> _mockLoggerFactory = new();
    private readonly Mock<IPasswordHasher> _mockPasswordHasher = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // Configuration de base pour les tests
        var configValues = new Dictionary<string, string>
        {
            ["Jwt:Secret"] = "test_secret_32_characters_long_123",
            ["Jwt:Issuer"] = "test_issuer",
            ["Jwt:Audience"] = "test_audience"
        };

        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns((User user) => new UserDto(user.Id, user.Username, user.Role.ToString()));

        _mockConfig.Setup(c => c[It.IsAny<string>()])
            .Returns<string>(key => configValues.TryGetValue(key, out var value) ? value : null);

        _mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());

        _mockPasswordHasher.Setup(p => p.Hash(It.IsAny<string>()))
            .Returns((string password) => Convert.ToBase64String(Encoding.UTF8.GetBytes(password)));

        _mockUserService.Setup(s => s.GetUserAsync(It.IsAny<string>()))
            .ReturnsAsync(_mockMapper.Object.Map<UserDto>(new User(It.IsAny<string>(), Convert.ToBase64String(Encoding.UTF8.GetBytes("pass")))));
        
        _mockUserRepository.Setup(r => r.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync((string username) => username == "admin" ? new User(username, Convert.ToBase64String(Encoding.UTF8.GetBytes("pass"))) : null);

        _authService = new AuthService(_mockPasswordHasher.Object, _mockConfig.Object, _mockLoggerFactory.Object,  _mockUserService.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task GenerateTokenAsync_ValidCredentials_ReturnsToken()
    {
        // Act
        var token = await _authService.GenerateTokenAsync("admin", "pass");
        
        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.True(token.Split('.').Length == 3); // Format JWT valide
    }

    [Fact]
    public async Task GenerateTokenAsync_InvalidUsername_ReturnsEmpty()
    {
        // Act
        var token = await _authService.GenerateTokenAsync("wronguser", "pass");
        
        // Assert
        Assert.Equal(string.Empty, token);
    }

    [Fact]
    public async Task GenerateTokenAsync_InvalidPassword_ReturnsEmpty()
    {
        // Act
        var token = await _authService.GenerateTokenAsync("admin", "wrongpass");
        
        // Assert
        Assert.Equal(string.Empty, token);
    }

    [Fact]
    public async Task GenerateTokenAsync_MissingConfig_ThrowsException()
    {
        // Arrange - Configuration manquante
        var emptyConfig = new Mock<IConfiguration>();
        var invalidAuthService = new AuthService(_mockPasswordHasher.Object, _mockConfig.Object, _mockLoggerFactory.Object, _mockUserService.Object, _mockUserRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => invalidAuthService.GenerateTokenAsync("admin", "pass")
        );
    }
}