using BusinessLayer.DTOs;
using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IVolunteerAssignmentService
{
    Task<IReadOnlyList<Planning>> GetPlanningsByVolunteerIdAsync(int userId);
    Task AssignAsync(IEnumerable<VolunteerAssignmentRequest> requests);
    Task ValidateAsync(IEnumerable<VolunteerAssignmentRequest> requests);
}
