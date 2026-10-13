namespace BusinessLayer.DTOs;

public sealed class AccountUserRecord
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string? RoleName { get; set; }
}
