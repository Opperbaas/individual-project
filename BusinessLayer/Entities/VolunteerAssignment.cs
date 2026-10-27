namespace BusinessLayer.Entities;

public sealed class VolunteerAssignment
{
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
    public int ActivityId { get; set; }

    public User User { get; set; } = null!;
    public Activity Activity { get; set; } = null!;
}
