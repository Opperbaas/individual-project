using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class ActivityService
{
    private readonly IActivityRepository _activityRepository;

    public ActivityService(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public Task<Activity> CreateAsync(
        int eventId,
        string activityName,
        string activityType,
        string description,
        int maxParticipants)
    {
        if (eventId <= 0)
        {
            throw new ArgumentException("Het sportevenement is ongeldig.", nameof(eventId));
        }

        if (string.IsNullOrWhiteSpace(activityName))
        {
            throw new ArgumentException(
                "Een activiteit moet een naam hebben.",
                nameof(activityName));
        }

        if (string.IsNullOrWhiteSpace(activityType))
        {
            throw new ArgumentException(
                "Een activiteit moet een type hebben.",
                nameof(activityType));
        }

        if (maxParticipants <= 0)
        {
            throw new ArgumentException(
                "Het maximum aantal deelnemers moet groter dan nul zijn.",
                nameof(maxParticipants));
        }

        return _activityRepository.CreateAsync(new Activity
        {
            EventId = eventId,
            ActivityName = activityName.Trim(),
            ActivityType = activityType.Trim(),
            Description = description?.Trim() ?? "",
            MaxParticipants = maxParticipants
        });
    }
}
