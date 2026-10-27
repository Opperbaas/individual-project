namespace BusinessLayer.Entities;


public sealed class Location
{
    public int LocationId { get; set; }
    public string Name { get; set; } = "";
    public string LocationType { get; set; } = "";
    public int Capacity { get; set; }
    public string Description { get; set; } = "";

    public ICollection<ActivitySchedule> ActivitySchedules { get; set; } = new List<ActivitySchedule>();
}