namespace DataAccesLayer.Database;

public sealed class PlannerRole
{
    public int RoleId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public ICollection<PlannerUser> Users { get; set; } = new List<PlannerUser>();
}
