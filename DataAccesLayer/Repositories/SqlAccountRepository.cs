using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Repositories;

public sealed class SqlAccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlAccountRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<AccountUserRecord?> FindByEmailAsync(string email)
    {
        return _dbContext.Users
            .Where(user => user.Email == email)
            .Select(user => new AccountUserRecord
            {
                UserId = user.UserId,
                RoleId = user.RoleId,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                RoleName = user.Role.Name
            })
            .SingleOrDefaultAsync();
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        return _dbContext.Users.AnyAsync(user => user.Email == email);
    }

    public async Task CreateAsync(string name, string email, string passwordHash)
    {
        var roleId = await _dbContext.Roles
            .Where(role => role.Name == "User")
            .Select(role => (int?)role.RoleId)
            .SingleOrDefaultAsync();

        if (roleId is null)
        {
            throw new InvalidOperationException("De standaardrol 'User' ontbreekt in dbo.Role.");
        }

        _dbContext.Users.Add(new PlannerUser
        {
            RoleId = roleId.Value,
            Name = name,
            Email = email,
            PasswordHash = passwordHash
        });

        await _dbContext.SaveChangesAsync();
    }
}
