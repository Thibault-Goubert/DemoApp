namespace DemoApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Username { get; set; }
    public string? Password { get; set; } // En clair pour l’exemple ; hashé en prod
}
