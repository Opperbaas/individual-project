using BusinessLayer.Entities;
using BusinessLayer.Enums;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class ActivityService : IActivityService
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

    public async Task<IReadOnlyList<Activity>> GenerateAsync(
        int eventId,
        string executionType,
        string locationType,
        string locationName,
        int locationCapacity,
        int maxParticipants,
        int count)
    {
        if (eventId <= 0)
        {
            throw new ArgumentException(
                "Het sportevenement is ongeldig.",
                nameof(eventId));
        }

        if (!LocationTypes.All.Contains(locationType,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Kies een geldig locatietype.",
                nameof(locationType));
        }

        if (string.IsNullOrWhiteSpace(executionType))
        {
            throw new ArgumentException(
                "Kies een uitvoeringsvorm.",
                nameof(executionType));
        }

        if (count <= 0 || count > 20)
        {
            throw new ArgumentException(
                "Het aantal activiteiten moet tussen 1 en 20 liggen.",
                nameof(count));
        }

        if (maxParticipants <= 0 ||
            maxParticipants > locationCapacity)
        {
            throw new ArgumentException(
                "Het maximum aantal deelnemers past niet bij de capaciteit van de gekozen locatie.",
                nameof(maxParticipants));
        }

        var activities = new List<Activity>();
        for (var index = 1; index <= count; index++)
        {
            var suffix = count == 1 ? "" : $" {index}";
            var activity = await _activityRepository.CreateAsync(new Activity
            {
                EventId = eventId,
                ActivityName = $"{executionType} activiteit - {locationType}{suffix}",
                ActivityType = locationType.Trim(),
                Description =
                    $"{executionType}-activiteit op locatie {locationName} ({locationType}).",
                MaxParticipants = maxParticipants
            });
            activities.Add(activity);
        }

        return activities;
    }

}
