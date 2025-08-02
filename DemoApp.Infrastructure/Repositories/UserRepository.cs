using DemoApp.Domain.Entities;
using DemoApp.Domain.Interfaces.Repositories;
using DemoApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<AppDbContext> _ctx;
    public UserRepository(IDbContextFactory<AppDbContext> ctx) => _ctx = ctx;

    public async Task AddAsync(User user)
    {
        await using var db = await _ctx.CreateDbContextAsync();
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        await using var db = await _ctx.CreateDbContextAsync();
        return await db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
    
    public async Task<bool> CheckUserExistsAsync(string username)
    {
        await using var db = await _ctx.CreateDbContextAsync();
        return await db.Users.AnyAsync(u => u.Username == username);
    }
}
