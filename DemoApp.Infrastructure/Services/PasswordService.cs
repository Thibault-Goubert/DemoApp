using Microsoft.AspNetCore.Identity;

namespace DemoApp.Infrastructure.Services;

public class PasswordService
{
    private readonly PasswordHasher<string> _hasher = new();

    public string HashPassword(string username, string password)
    {
        return _hasher.HashPassword(username, password);
    }

    public bool VerifyPassword(string username, string password, string hash)
    {
        return _hasher.VerifyHashedPassword(username, hash, password) == PasswordVerificationResult.Success;
    }
}
