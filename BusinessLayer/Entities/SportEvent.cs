namespace BusinessLayer.Entities;


public sealed class SportEvent
{
    public int EventId { get; set; }
    public string EventName { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    public ICollection<Planning> Plannings { get; set; } = new List<Planning>();
}