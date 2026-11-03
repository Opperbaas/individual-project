using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IRegistrationRepository
{
    Task<Activity?> GetActivityForRegistrationAsync(int activityId);
    Task<bool> ExistsAsync(int userId, int activityId);
    Task<Registration> CreateAsync(Registration registration);
}
