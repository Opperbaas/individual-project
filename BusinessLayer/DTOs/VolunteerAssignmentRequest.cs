namespace BusinessLayer.DTOs;

public sealed class VolunteerAssignmentRequest
{
    public int ActivityId { get; init; }
    public int TimeSlotId { get; init; }
    public IReadOnlyCollection<int> VolunteerIds { get; init; } = [];
}
