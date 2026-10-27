namespace BusinessLayer.Entities;


public sealed class Registration
{
    public int RegistrationId { get; set; }
    public int UserId { get; set; }
    public int ActivityId { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string Status { get; set; } = "";

    public User User { get; set; } = null!;
    public Activity Activity { get; set; } = null!;
}