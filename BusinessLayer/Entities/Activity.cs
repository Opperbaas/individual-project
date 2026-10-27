namespace BusinessLayer.Entities; 


public sealed class Activity
{
    public int ActivityId { get; set; }
    public int EventId { get; set; }
    public int MaxParticipants { get; set; }
    public string ActivityName { get; set; } = "";
    public string ActivityType { get; set; } = "";
    public string Description { get; set; } = "";
    public SportEvent Event { get; set; } = null!;
    public ICollection<ActivitySchedule> ActivitySchedules { get; set; } = new List<ActivitySchedule>();
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<VolunteerAssignment> VolunteerAssignments { get; set; } = new List<VolunteerAssignment>();
}