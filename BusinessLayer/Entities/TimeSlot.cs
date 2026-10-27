namespace BusinessLayer.Entities;


public sealed class TimeSlot
{
    public int TimeSlotId { get; set; }
    public int EventId { get; set; }
    public DateOnly SlotDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public SportEvent Event { get; set; } = null!;
    public ICollection<ActivitySchedule> ActivitySchedules { get; set; } = new List<ActivitySchedule>();
}