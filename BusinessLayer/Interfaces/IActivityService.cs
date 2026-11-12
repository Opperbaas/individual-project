using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IActivityService
{
    Task<Activity> CreateAsync(
        int eventId,
        string activityName,
        string activityType,
        string description,
        int maxParticipants);
}
