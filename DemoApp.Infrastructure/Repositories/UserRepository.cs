using DemoApp.Domain.Entities;
using DemoApp.Domain.Interfaces;
using DemoApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

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

    public async Task<IEnumerable<User>> GetAllAsync(){ 
        await using var db = await _ctx.CreateDbContextAsync();
        return await db.Users.ToListAsync().ContinueWith(t => (IEnumerable<User>)t.Result);
    } 
}
