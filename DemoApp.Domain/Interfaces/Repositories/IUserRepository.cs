using DemoApp.Domain.Entities;

namespace DemoApp.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<bool> CheckUserExistsAsync(string username);
}
