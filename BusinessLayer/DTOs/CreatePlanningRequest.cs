namespace BusinessLayer.DTOs;

public sealed class CreatePlanningRequest
{
    public int EventId { get; init; }
    public int CreatedByUserId { get; init; }
    public string Name { get; init; } = "";
    public IReadOnlyCollection<CreatePlanningScheduleRequest> Schedules { get; init; } = [];
}
