using DemoApp.Domain.Common;

namespace DemoApp.Domain.Entities;

public class User : AggregateRoot<Guid>
{
    public string Username { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Role { get; private set; } = "User";

    public User() : base(Guid.NewGuid())
    {
        // Default constructor for EF Core
    }

    public User(string username, string passwordHash) : base(Guid.NewGuid())
    {
        Username = username;
        PasswordHash = passwordHash;
    }
}
