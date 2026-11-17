namespace BusinessLayer.DTOs;

public sealed class CreatePlanningScheduleRequest
{
    public int ActivityId { get; init; }
    public int LocationId { get; init; }
    public int TimeSlotId { get; init; }
    public IReadOnlyCollection<int> VolunteerIds { get; init; } = [];
}
