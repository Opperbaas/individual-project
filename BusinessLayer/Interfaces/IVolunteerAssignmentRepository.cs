using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IVolunteerAssignmentRepository
{
    Task<IReadOnlyList<User>> GetVolunteersByIdsAsync(
        IEnumerable<int> userIds);

    Task<TimeSlot?> GetTimeSlotAsync(int timeSlotId);
    Task<bool> HasAvailabilityAsync(
        int userId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime);

    Task CreateManyAsync(IEnumerable<VolunteerAssignment> assignments);
}
