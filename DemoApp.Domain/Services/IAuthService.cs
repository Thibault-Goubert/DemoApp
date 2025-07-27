namespace DemoApp.Domain.Interfaces;

public interface IAuthService
{
    Task<string> GenerateTokenAsync(string username, string password);
}
