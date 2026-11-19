using BusinessLayer.Entities;
using BusinessLayer.Enums;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityTemplateService _activityTemplateService;

    public ActivityService(
        IActivityRepository activityRepository,
        IActivityTemplateService activityTemplateService)
    {
        _activityRepository = activityRepository;
        _activityTemplateService = activityTemplateService;
    }

    public Task<Activity> CreateAsync(
        int eventId,
        string activityName,
        string activityType,
        string description,
        int maxParticipants)
    {
        ValidateActivity(
            eventId,
            activityName,
            activityType,
            maxParticipants);

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

        if (!ExecutionTypes.All.Contains(executionType,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Kies een geldige sportvorm.",
                nameof(executionType));
        }

        if (!_activityTemplateService.IsCompatible(executionType, locationType))
        {
            throw new ArgumentException(
                $"De uitvoering {executionType} past niet bij locatietype {locationType}.",
                nameof(locationType));
        }

        if (count <= 0 || count > 20)
        {
            throw new ArgumentException(
                "Het aantal activiteiten moet tussen 1 en 20 liggen.",
                nameof(count));
        }

        if (maxParticipants <= 0)
        {
            throw new ArgumentException(
                "Het maximum aantal deelnemers moet groter dan nul zijn.",
                nameof(maxParticipants));
        }

        var activities = new List<Activity>();
        for (var index = 1; index <= count; index++)
        {
            var suffix = count == 1 ? "" : $" {index}";
            var activityName =
                $"{_activityTemplateService.GetActivityName(executionType, locationType)}{suffix}";

            var activity = await _activityRepository.CreateAsync(new Activity
            {
                EventId = eventId,
                ActivityName = activityName,
                ActivityType = locationType.Trim(),
                Description =
                    $"{executionType}-activiteit op locatie {locationName} ({locationType}).",
                MaxParticipants = maxParticipants
            });
            activities.Add(activity);
        }

        return activities;
    }

    public Task<Activity?> GetByIdAsync(int activityId)
    {
        if (activityId <= 0)
        {
            throw new ArgumentException("De activiteit is ongeldig.");
        }

        return _activityRepository.GetByIdAsync(activityId);
    }

    public async Task UpdateAsync(
        int activityId,
        int eventId,
        string activityName,
        string activityType,
        string description,
        int maxParticipants)
    {
        if (activityId <= 0)
        {
            throw new ArgumentException("De activiteit is ongeldig.");
        }

        ValidateActivity(
            eventId,
            activityName,
            activityType,
            maxParticipants);

        await _activityRepository.UpdateAsync(new Activity
        {
            ActivityId = activityId,
            EventId = eventId,
            ActivityName = activityName.Trim(),
            ActivityType = activityType.Trim(),
            Description = description?.Trim() ?? "",
            MaxParticipants = maxParticipants
        });
    }

    private static void ValidateActivity(
        int eventId,
        string activityName,
        string activityType,
        int maxParticipants)
    {
        if (eventId <= 0)
        {
            throw new ArgumentException(
                "Het sportevenement is ongeldig.",
                nameof(eventId));
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
    }

    public Task DeleteAsync(int activityId)
    {
        if (activityId <= 0)
        {
            throw new ArgumentException("De activiteit is ongeldig.");
        }

        return _activityRepository.DeleteAsync(activityId);
    }
}
