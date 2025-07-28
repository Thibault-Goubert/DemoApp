using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoApp.Domain.Entities;

namespace DemoApp.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
}
