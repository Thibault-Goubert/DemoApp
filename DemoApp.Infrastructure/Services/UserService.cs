using AutoMapper;
using DemoApp.Application.DTO;
using DemoApp.Application.Services;
using DemoApp.Domain.Interfaces.Repositories;

namespace DemoApp.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<bool> CheckUserExistsAsync(string username) =>
        await _userRepository.CheckUserExistsAsync(username);

    public async Task<UserDto?> GetUserAsync(string username) =>
        _mapper.Map<UserDto>(await _userRepository.GetByUsernameAsync(username)) ?? null;
}
