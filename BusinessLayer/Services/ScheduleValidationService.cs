using BusinessLayer.Entities;

namespace BusinessLayer.Services;

public sealed class ScheduleValidationService
{
    public IReadOnlyList<string> Validate(
        Planning planning,
        IEnumerable<Planning>? existingPlannings = null,
        IEnumerable<Activity>? activities = null)
    {
        ArgumentNullException.ThrowIfNull(planning);

        var errors = new List<string>();
        var schedules = planning.ActivitySchedules.ToList();

        if (schedules.Count == 0)
        {
            errors.Add("Een planning moet ten minste één activiteitenschema bevatten.");
        }

        foreach (var activity in activities ?? [])
        {
            var scheduleCount = schedules.Count(schedule =>
                schedule.ActivityId == activity.ActivityId);

            if (scheduleCount == 0)
            {
                errors.Add(
                    $"Activiteit {activity.ActivityName} ontbreekt in de planning.");
            }

            if (scheduleCount > 1)
            {
                errors.Add(
                    $"Activiteit {activity.ActivityName} is meer dan één keer ingepland.");
            }
        }

        foreach (var schedule in schedules)
        {
            if (schedule.Activity is null)
            {
                errors.Add("Elke planningactiviteit moet een activiteit hebben.");
            }

            if (schedule.Location is null)
            {
                errors.Add("Elke planningactiviteit moet een locatie hebben.");
            }

            if (schedule.TimeSlot is null)
            {
                errors.Add("Elke planningactiviteit moet een tijdslot hebben.");
            }

            if (schedule.TimeSlot is not null &&
                planning.Event is not null &&
                (schedule.TimeSlot.SlotDate < DateOnly.FromDateTime(planning.Event.StartDate) ||
                 schedule.TimeSlot.SlotDate > DateOnly.FromDateTime(planning.Event.EndDate)))
            {
                errors.Add(
                    $"Het tijdslot van activiteit {schedule.ActivityId} valt buiten de datum van het evenement.");
            }

            if (schedule.Activity is not null &&
                schedule.Location is not null &&
                schedule.Location.Capacity < schedule.Activity.MaxParticipants)
            {
                errors.Add(
                    $"Locatie {schedule.Location.Name} heeft onvoldoende capaciteit voor activiteit {schedule.Activity.ActivityName}.");
            }
        }

        for (var index = 0; index < schedules.Count; index++)
        {
            for (var otherIndex = index + 1; otherIndex < schedules.Count; otherIndex++)
            {
                var current = schedules[index];
                var other = schedules[otherIndex];

                if (current.TimeSlot is null || other.TimeSlot is null)
                {
                    continue;
                }

                if (!Overlaps(current.TimeSlot, other.TimeSlot))
                {
                    continue;
                }

                AddConflictErrors(errors, current, other);
            }
        }

        foreach (var existingPlanning in existingPlannings ?? [])
        {
            if (existingPlanning.PlanningId == planning.PlanningId ||
                existingPlanning.EventId != planning.EventId)
            {
                continue;
            }

            foreach (var current in schedules)
            {
                foreach (var existing in existingPlanning.ActivitySchedules)
                {
                    if (current.TimeSlot is null || existing.TimeSlot is null)
                    {
                        continue;
                    }

                    if (!Overlaps(current.TimeSlot, existing.TimeSlot))
                    {
                        continue;
                    }

                    AddConflictErrors(errors, current, existing);
                }
            }
        }

        return errors.Distinct().ToArray();
    }

    private static void AddConflictErrors(
        ICollection<string> errors,
        ActivitySchedule current,
        ActivitySchedule other)
    {
        if (current.LocationId == other.LocationId)
        {
            errors.Add(
                $"Locatie {current.LocationId} is dubbel ingepland tijdens overlappende tijdsloten.");
        }

        if (current.ActivityId == other.ActivityId)
        {
            errors.Add(
                $"Activiteit {current.ActivityId} is dubbel ingepland tijdens overlappende tijdsloten.");
        }
    }

    private static bool Overlaps(TimeSlot first, TimeSlot second)
    {
        return first.SlotDate == second.SlotDate &&
               first.StartTime < second.EndTime &&
               second.StartTime < first.EndTime;
    }
}