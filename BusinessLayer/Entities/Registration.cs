namespace BusinessLayer.Entities;


public sealed class Registration
{
    public int RegistrationId { get; set; }
    public int UserId { get; set; }
    public int ActivityScheduleId { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string Status { get; set; } = "";

    public User User { get; set; } = null!;
    public ActivitySchedule ActivitySchedule { get; set; } = null!;
}