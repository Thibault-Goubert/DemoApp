using DemoApp.Domain.Entities;
using DemoApp.Domain.Interfaces;
using DemoApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _ctx;
    public UserRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(User user)
    {
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
    }

    public Task<User?> GetByUsernameAsync(string username) =>
        _ctx.Users.FirstOrDefaultAsync(u => u.Username == username);

    public Task<IEnumerable<User>> GetAllAsync() =>
        _ctx.Users.ToListAsync().ContinueWith(t => (IEnumerable<User>)t.Result);
}
