using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IRegistrationService
{
    Task<Registration> RegisterAsync(int userId, int activityId);
}
