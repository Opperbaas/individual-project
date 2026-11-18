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

    Task<IReadOnlyList<Activity>> GenerateAsync(
        int eventId,
        string executionType,
        string locationType,
        string locationName,
        int locationCapacity,
        int maxParticipants,
        int count);
    Task<Activity?> GetByIdAsync(int activityId);
    Task UpdateAsync(
        int activityId,
        int eventId,
        string activityName,
        string activityType,
        string description,
        int maxParticipants);
    Task DeleteAsync(int activityId);
}
