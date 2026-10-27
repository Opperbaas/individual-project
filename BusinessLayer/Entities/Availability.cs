namespace BusinessLayer.Entities;

public sealed class Availability
{
    public int AvailabilityId { get; set; }
    public int UserId { get; set; }
    public DateOnly AvailableDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public User User { get; set; } = null!;
}
