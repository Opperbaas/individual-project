using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class OptimizedPlanner : IPlanningGenerator
{
    private readonly IPlanningRepository _planningRepository;

    public OptimizedPlanner(IPlanningRepository planningRepository)
    {
        _planningRepository = planningRepository;
    }

    public async Task<Planning> GenerateAsync(
        Planning planning,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(planning);
        cancellationToken.ThrowIfCancellationRequested();

        var activities =
            await _planningRepository.GetActivitiesByEventIdAsync(planning.EventId);
        var locations = await _planningRepository.GetLocationsAsync();
        var allTimeSlots =
            await _planningRepository.GetTimeSlotsByEventIdAsync(planning.EventId);

        var timeSlots = planning.Event is null
            ? allTimeSlots
            : allTimeSlots
                .Where(timeSlot =>
                    timeSlot.SlotDate >= DateOnly.FromDateTime(planning.Event.StartDate) &&
                    timeSlot.SlotDate <= DateOnly.FromDateTime(planning.Event.EndDate))
                .ToList();

        if (activities.Count == 0 || locations.Count == 0 || timeSlots.Count == 0)
        {
            throw new InvalidOperationException(
                "Er zijn activiteiten, locaties en tijdsloten nodig om een planning te genereren.");
        }

        var orderedActivities = activities
            .OrderByDescending(activity => activity.MaxParticipants)
            .ThenBy(activity => activity.ActivityId)
            .ToList();
        var orderedTimeSlots = timeSlots
            .OrderBy(timeSlot => timeSlot.SlotDate)
            .ThenBy(timeSlot => timeSlot.StartTime)
            .ToList();
        var orderedLocations = locations
            .OrderByDescending(location => location.Capacity)
            .ThenBy(location => location.LocationId)
            .ToList();

        foreach (var activity in orderedActivities)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var assignment = FindAssignment(
                activity,
                orderedLocations,
                orderedTimeSlots,
                planning.ActivitySchedules);

            if (assignment is null)
            {
                throw new InvalidOperationException(
                    $"Er kon geen geldig tijdslot en geen geschikte locatie worden gevonden voor activiteit {activity.ActivityName}.");
            }

            planning.ActivitySchedules.Add(new ActivitySchedule
            {
                ActivityId = activity.ActivityId,
                LocationId = assignment.Location.LocationId,
                TimeSlotId = assignment.TimeSlot.TimeSlotId,
                Activity = activity,
                Location = assignment.Location,
                TimeSlot = assignment.TimeSlot
            });
        }

        planning.IsAiGenerated = false;
        return planning;
    }

    private static Assignment? FindAssignment(
        Activity activity,
        IReadOnlyCollection<Location> locations,
        IReadOnlyCollection<TimeSlot> timeSlots,
        IEnumerable<ActivitySchedule> schedules)
    {
        foreach (var timeSlot in timeSlots)
        {
            foreach (var location in locations.Where(
                         location => location.Capacity >= activity.MaxParticipants))
            {
                var locationIsAvailable = schedules.All(schedule =>
                    schedule.LocationId != location.LocationId ||
                    !Overlaps(schedule.TimeSlot, timeSlot));

                if (locationIsAvailable)
                {
                    return new Assignment(location, timeSlot);
                }
            }
        }

        return null;
    }

    private static bool Overlaps(TimeSlot first, TimeSlot second)
    {
        return first.SlotDate == second.SlotDate &&
               first.StartTime < second.EndTime &&
               second.StartTime < first.EndTime;
    }

    private sealed record Assignment(Location Location, TimeSlot TimeSlot);
}
