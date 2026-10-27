namespace BusinessLayer.Entities;


public sealed class Planning
{
    public int PlanningId { get; set; }
    public int EventId { get; set; }
    public int CreatedByUserId { get; set; }
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public bool IsAiGenerated { get; set; }
    public DateTime CreatedAt { get; set; }

    public SportEvent Event { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public ICollection<ActivitySchedule> ActivitySchedules { get; set; } = new List<ActivitySchedule>();
}