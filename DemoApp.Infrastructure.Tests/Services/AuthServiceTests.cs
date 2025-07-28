using DemoApp.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DemoApp.Infrastructure.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig = new();
    private readonly Mock<ILoggerFactory> _mockLoggerFactory = new();
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

        _mockConfig.Setup(c => c[It.IsAny<string>()])
            .Returns<string>(key => configValues.TryGetValue(key, out var value) ? value : null);

        _mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(Mock.Of<ILogger>());

        _authService = new AuthService(_mockConfig.Object, _mockLoggerFactory.Object);
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
        var invalidAuthService = new AuthService(emptyConfig.Object, _mockLoggerFactory.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => invalidAuthService.GenerateTokenAsync("admin", "pass")
        );
    }
}