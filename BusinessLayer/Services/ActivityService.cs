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

        if (!ExecutionTypes.All.Contains(executionType,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Kies een geldige sportvorm.",
                nameof(executionType));
        }

        if (!IsCompatible(executionType, locationType))
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
            var activity = await _activityRepository.CreateAsync(new Activity
            {
                EventId = eventId,
                ActivityName =
                    $"{GetActivityName(executionType, locationType)}{suffix}",
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

    public Task DeleteAsync(int activityId)
    {
        if (activityId <= 0)
        {
            throw new ArgumentException("De activiteit is ongeldig.");
        }

        return _activityRepository.DeleteAsync(activityId);
    }

    private static string GetActivityName(
        string executionType,
        string locationType)
    {
        return (executionType, locationType) switch
        {
            ("Teamsporten", "Voetbalveld") => "Voetbalwedstrijd",
            ("Teamsporten", "Sporthal") => "Teamzaaltoernooi",
            ("Teamsporten", "Stadion") => "Grote teamcompetitie",
            ("Teamsporten", "Openluchtgebied") => "Buitenteamwedstrijd",
            ("Teamsporten", "Tennisbaan") => "Dubbelseernooi",
            ("Teamsporten", "Basketbalhal") => "Basketbaltoernooi",

            ("Individuele sporten", "Tennisbaan") => "Tennistoernooi",
            ("Individuele sporten", "Zwembad") => "Zwemmomento",
            ("Individuele sporten", "Atletiekbaan") => "Atletiekwedstrijd",
            ("Individuele sporten", "Fitnessruimte") => "Individuele trainingssessie",
            ("Individuele sporten", "Sporthal") => "Individuele zaalwedstrijd",
            ("Individuele sporten", "Openluchtgebied") => "Recreatieve looptocht",

            ("Fit- en gymsporten", "Fitnessruimte") => "Fitnesstraining",
            ("Fit- en gymsporten", "Sporthal") => "Gymnastiekcircuit",
            ("Fit- en gymsporten", "Openluchtgebied") => "Buitenfitness",
            ("Fit- en gymsporten", "Stadion") => "Groepsconditietraining",

            ("Avontuurlijke en risicosporten", "Openluchtgebied") =>
                "Avontuurtocht",
            ("Avontuurlijke en risicosporten", "Stadion") =>
                "Avontuurlijk obstakeltoernooi",

            ("Lucht- en watersporten", "Zwembad") => "Aquaticsessie",
            ("Lucht- en watersporten", "Openluchtgebied") => "Watersportclinic",
            ("Lucht- en watersporten", "Stadion") => "Lucht- en watersportdemonstratie",

            ("Balsporten en racket-/batjesporten", "Voetbalveld") =>
                "Balspeltoernooi",
            ("Balsporten en racket-/batjesporten", "Sporthal") =>
                "Racket- en balsportdag",
            ("Balsporten en racket-/batjesporten", "Tennisbaan") =>
                "Racketclinic",
            ("Balsporten en racket-/batjesporten", "Basketbalhal") =>
                "Balvaardigheidsdag",

            ("Strand- en recreatiesporten", "Openluchtgebied") =>
                "Recreatieve strandactiviteit",
            ("Strand- en recreatiesporten", "Voetbalveld") =>
                "Recreatieve veldspelen",
            ("Strand- en recreatiesporten", "Sporthal") =>
                "Recreatieve zaalspelen",

            _ => "Algemene sportactiviteit"
        };
    }

    private static bool IsCompatible(
        string executionType,
        string locationType)
    {
        return (executionType, locationType) switch
        {
            ("Teamsporten", "Voetbalveld") => true,
            ("Teamsporten", "Sporthal") => true,
            ("Teamsporten", "Stadion") => true,
            ("Teamsporten", "Openluchtgebied") => true,
            ("Teamsporten", "Basketbalhal") => true,
            ("Teamsporten", "Tennisbaan") => true,

            ("Individuele sporten", "Tennisbaan") => true,
            ("Individuele sporten", "Zwembad") => true,
            ("Individuele sporten", "Atletiekbaan") => true,
            ("Individuele sporten", "Fitnessruimte") => true,
            ("Individuele sporten", "Sporthal") => true,
            ("Individuele sporten", "Openluchtgebied") => true,

            ("Fit- en gymsporten", "Fitnessruimte") => true,
            ("Fit- en gymsporten", "Sporthal") => true,
            ("Fit- en gymsporten", "Openluchtgebied") => true,
            ("Fit- en gymsporten", "Stadion") => true,

            ("Avontuurlijke en risicosporten", "Openluchtgebied") => true,
            ("Avontuurlijke en risicosporten", "Stadion") => true,

            ("Lucht- en watersporten", "Zwembad") => true,
            ("Lucht- en watersporten", "Openluchtgebied") => true,
            ("Lucht- en watersporten", "Stadion") => true,

            ("Balsporten en racket-/batjesporten", "Voetbalveld") => true,
            ("Balsporten en racket-/batjesporten", "Sporthal") => true,
            ("Balsporten en racket-/batjesporten", "Tennisbaan") => true,
            ("Balsporten en racket-/batjesporten", "Basketbalhal") => true,

            ("Strand- en recreatiesporten", "Openluchtgebied") => true,
            ("Strand- en recreatiesporten", "Voetbalveld") => true,
            ("Strand- en recreatiesporten", "Sporthal") => true,

            _ => false
        };
    }

}
