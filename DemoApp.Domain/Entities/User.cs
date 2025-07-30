namespace DemoApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; }
    public string PasswordHash { get; set; } // Mot de passe hashé
    public string Role { get; set; } = "User";
}
