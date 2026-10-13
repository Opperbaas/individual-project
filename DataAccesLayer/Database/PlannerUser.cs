namespace DataAccesLayer.Database;

public sealed class PlannerUser
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public PlannerRole Role { get; set; } = null!;
}
