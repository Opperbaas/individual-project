using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IRegistrationRepository
{
    Task<ActivitySchedule?> GetActivityScheduleForRegistrationAsync(int activityScheduleId);
    Task<bool> ExistsAsync(int userId, int activityScheduleId);
    Task<Registration> CreateAsync(Registration registration);
}
