using DemoApp.Application.DTO;

namespace DemoApp.Application.Services;

public interface IUserService
{
    Task<bool> CheckUserExistsAsync(string username);
    Task<UserDto?> GetUserAsync(string username);
}
