namespace BusinessLayer.Entities;

public sealed class ActivitySchedule
{
    public int ActivityScheduleId { get; set; }
    public int ActivityId { get; set; }
    public int LocationId { get; set; }
    public int TimeSlotId { get; set; }
    public int PlanningId { get; set; }
    public Activity Activity { get; set; } = null!;
    public Location Location { get; set; } = null!;
    public TimeSlot TimeSlot { get; set; } = null!;
    public Planning Planning { get; set; } = null!;
}