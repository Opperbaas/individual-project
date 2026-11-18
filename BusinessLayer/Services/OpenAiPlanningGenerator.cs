using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class OpenAiPlanningGenerator : IPlanningGenerator
{
    private readonly HttpClient _httpClient;
    private readonly IPlanningRepository _planningRepository;
    private readonly OpenAiSettings _settings;

    public OpenAiPlanningGenerator(
        HttpClient httpClient,
        IPlanningRepository planningRepository,
        OpenAiSettings settings)
    {
        _httpClient = httpClient;
        _planningRepository = planningRepository;
        _settings = settings;
    }

    public async Task<Planning> GenerateAsync(
        Planning planning,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(planning);

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI is niet geconfigureerd. Stel OpenAI:ApiKey in.");
        }

        var activities =
            await _planningRepository.GetActivitiesByEventIdAsync(planning.EventId);
        var locations = await _planningRepository.GetLocationsAsync();
        var allTimeSlots =
            await _planningRepository.GetTimeSlotsByEventIdAsync(planning.EventId);
        var eventStart = planning.Event is null
            ? (DateOnly?)null
            : DateOnly.FromDateTime(planning.Event.StartDate);
        var eventEnd = planning.Event is null
            ? (DateOnly?)null
            : DateOnly.FromDateTime(planning.Event.EndDate);

        var timeSlots = eventStart is null || eventEnd is null
            ? allTimeSlots
            : allTimeSlots
                .Where(timeSlot =>
                    timeSlot.SlotDate >= eventStart &&
                    timeSlot.SlotDate <= eventEnd)
                .ToList();

        if (activities.Count == 0 || locations.Count == 0 || timeSlots.Count == 0)
        {
            throw new InvalidOperationException(
                "Er zijn activiteiten, locaties en tijdsloten nodig om een planning te genereren.");
        }

        var input = new
        {
            eventId = planning.EventId,
            activities = activities.Select(activity => new
            {
                id = activity.ActivityId,
                name = activity.ActivityName,
                type = activity.ActivityType,
                description = activity.Description,
                maxParticipants = activity.MaxParticipants
            }),
            locations = locations.Select(location => new
            {
                id = location.LocationId,
                name = location.Name,
                type = location.LocationType,
                capacity = location.Capacity,
                description = location.Description
            }),
            timeSlots = timeSlots.Select(timeSlot => new
            {
                id = timeSlot.TimeSlotId,
                date = timeSlot.SlotDate.ToString("yyyy-MM-dd"),
                start = timeSlot.StartTime.ToString("HH:mm"),
                end = timeSlot.EndTime.ToString("HH:mm")
            })
        };

        var request = new
        {
            model = _settings.Model,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content =
                        """
                        Maak een haalbare sportevenementplanning.
                        Gebruik uitsluitend de aangeleverde IDs.
                        Plan iedere activiteit exact één keer.
                        Gebruik geen dezelfde locatie voor activiteiten waarvan de
                        tijdsloten op dezelfde datum overlappen.
                        Gebruik geen dezelfde activiteit meer dan één keer.
                        Kies uitsluitend tijdsloten die binnen de eventdata liggen.
                        Geef uitsluitend JSON terug in de vorm:
                        {"schedules":[{"activityId":1,"locationId":2,"timeSlotId":3}]}
                        """
                },
                new
                {
                    role = "user",
                    content = JsonSerializer.Serialize(input)
                }
            }
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "chat/completions");
        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        httpRequest.Content = JsonContent.Create(request);

        using var response = await _httpClient.SendAsync(
            httpRequest,
            cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = GetOpenAiErrorMessage(responseBody);
            throw new InvalidOperationException(
                $"OpenAI kon geen planning genereren " +
                $"({(int)response.StatusCode} {response.ReasonPhrase}). " +
                $"{errorMessage}");
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var content = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException(
                    "OpenAI gaf geen planningsvoorstel terug.");
            }

            var generated = JsonSerializer.Deserialize<GeneratedPlanning>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (generated?.Schedules is null || generated.Schedules.Count == 0)
            {
                throw new InvalidOperationException(
                    "OpenAI gaf geen activiteitenschema's terug.");
            }

            var uniqueSchedules = new HashSet<string>();

            foreach (var schedule in generated.Schedules)
            {
                var activity = activities.SingleOrDefault(
                    item => item.ActivityId == schedule.ActivityId);
                var location = locations.SingleOrDefault(
                    item => item.LocationId == schedule.LocationId);
                var timeSlot = timeSlots.SingleOrDefault(
                    item => item.TimeSlotId == schedule.TimeSlotId);

                if (activity is null || location is null || timeSlot is null)
                {
                    throw new InvalidOperationException(
                        "OpenAI gaf een ID terug die niet bij dit evenement hoort.");
                }

                var scheduleKey =
                    $"{schedule.ActivityId}:{schedule.LocationId}:{schedule.TimeSlotId}";
                if (!uniqueSchedules.Add(scheduleKey))
                {
                    throw new InvalidOperationException(
                        "OpenAI gaf hetzelfde activiteitenschema meerdere keren terug.");
                }

                planning.ActivitySchedules.Add(new ActivitySchedule
                {
                    ActivityId = activity.ActivityId,
                    LocationId = location.LocationId,
                    TimeSlotId = timeSlot.TimeSlotId,
                    Activity = activity,
                    Location = location,
                    TimeSlot = timeSlot
                });
            }

            planning.IsAiGenerated = true;
            return planning;
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "OpenAI gaf geen geldig JSON-planningsvoorstel terug.",
                exception);
        }
        catch (KeyNotFoundException exception)
        {
            throw new InvalidOperationException(
                "OpenAI gaf een onverwachte antwoordstructuur terug.",
                exception);
        }
    }

    private static string GetOpenAiErrorMessage(string responseBody)
    {
        try
        {
            using var document = JsonDocument.Parse(responseBody);
            return document.RootElement
                .GetProperty("error")
                .GetProperty("message")
                .GetString()
                ?? "Controleer de OpenAI-configuratie.";
        }
        catch (JsonException)
        {
            return "Controleer de OpenAI-configuratie en het ingestelde model.";
        }
        catch (InvalidOperationException)
        {
            return "Controleer de OpenAI-configuratie en het ingestelde model.";
        }
    }

    private sealed class GeneratedPlanning
    {
        public List<GeneratedSchedule> Schedules { get; init; } = [];
    }

    private sealed class GeneratedSchedule
    {
        public int ActivityId { get; init; }
        public int LocationId { get; init; }
        public int TimeSlotId { get; init; }
    }
}
