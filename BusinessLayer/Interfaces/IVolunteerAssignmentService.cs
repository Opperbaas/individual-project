using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces;

public interface IVolunteerAssignmentService
{
    Task AssignAsync(IEnumerable<VolunteerAssignmentRequest> requests);
    Task ValidateAsync(IEnumerable<VolunteerAssignmentRequest> requests);
}
