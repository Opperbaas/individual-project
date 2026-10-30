namespace BusinessLayer.DTOs;

public sealed class PlanningSummaryDto
{
    public int PlanningId { get; set; }
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
