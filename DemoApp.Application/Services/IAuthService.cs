
namespace DemoApp.Application.Services;

public interface IAuthService
{    
    Task<bool> RegisterAsync(string username, string password);
    Task<bool> ValidateUserAsync(string username, string password);
    Task<string> GenerateTokenAsync(string username, string password);
}
