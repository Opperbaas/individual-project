using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces;

public interface IAccountRepository
{
    Task<AccountUserRecord?> FindByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task CreateAsync(string name, string email, string passwordHash);
}
