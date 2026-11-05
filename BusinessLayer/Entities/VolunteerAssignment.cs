namespace BusinessLayer.Entities;

public sealed class VolunteerAssignment
{
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
    public int ActivityScheduleId { get; set; }

    public ActivitySchedule ActivitySchedule { get; set; } = null!;
    public User User { get; set; } = null!;
}
