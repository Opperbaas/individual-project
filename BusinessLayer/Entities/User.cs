namespace BusinessLayer.Entities;

public sealed class User
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public Role Role { get; set; } = null!;
    public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public ICollection<VolunteerAssignment> VolunteerAssignments { get; set; } = new List<VolunteerAssignment>();
    public ICollection<Planning> CreatedPlannings { get; set; } = new List<Planning>();
}