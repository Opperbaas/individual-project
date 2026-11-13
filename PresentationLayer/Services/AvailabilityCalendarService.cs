using System.Globalization;
using BusinessLayer.Entities;
using PresentationLayer.Interfaces;
using PresentationLayer.Models;

namespace PresentationLayer.Services;

public sealed class AvailabilityCalendarService
    : IAvailabilityCalendarService
{
    public AvailabilityCalendarViewModel CreateModel(
        DateOnly date,
        IEnumerable<Availability> availability)
    {
        ArgumentNullException.ThrowIfNull(availability);

        var weekStart = GetMonday(date);
        var days = Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var day = weekStart.AddDays(offset);
                return new AvailabilityCalendarDayViewModel
                {
                    Date = day,
                    DayName = day.ToString(
                        "ddd",
                        CultureInfo.GetCultureInfo("nl-NL")),
                    IsToday = day == DateOnly.FromDateTime(DateTime.Today)
                };
            })
            .ToArray();

        return new AvailabilityCalendarViewModel
        {
            WeekStart = weekStart,
            Days = days,
            SelectedSlots = ToSlotKeys(availability).ToList()
        };
    }

    public IReadOnlyList<Availability> ParseAvailability(
        IEnumerable<string> selectedSlots)
    {
        ArgumentNullException.ThrowIfNull(selectedSlots);

        var slotsByDate = new Dictionary<DateOnly, List<TimeOnly>>();

        foreach (var selectedSlot in selectedSlots)
        {
            var parts = selectedSlot.Split('|', 2);
            if (parts.Length != 2 ||
                !DateOnly.TryParseExact(
                    parts[0],
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var date) ||
                !TimeOnly.TryParseExact(
                    parts[1],
                    "HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var time))
            {
                continue;
            }

            if (!slotsByDate.TryGetValue(date, out var slots))
            {
                slots = [];
                slotsByDate[date] = slots;
            }

            slots.Add(time);
        }

        return slotsByDate
            .SelectMany(CreateRanges)
            .Select(range => new Availability
            {
                AvailableDate = range.Date,
                StartTime = range.StartTime,
                EndTime = range.EndTime
            })
            .ToArray();
    }

    private static IEnumerable<AvailabilityRange> CreateRanges(
        KeyValuePair<DateOnly, List<TimeOnly>> dateSlots)
    {
        var slots = dateSlots.Value
            .Distinct()
            .OrderBy(time => time)
            .ToList();

        if (slots.Count == 0)
        {
            yield break;
        }

        var rangeStart = slots[0];
        var previous = slots[0];

        foreach (var slot in slots.Skip(1))
        {
            if (slot != previous.AddMinutes(30))
            {
                yield return new AvailabilityRange(
                    dateSlots.Key,
                    rangeStart,
                    previous.AddMinutes(30));
                rangeStart = slot;
            }

            previous = slot;
        }

        yield return new AvailabilityRange(
            dateSlots.Key,
            rangeStart,
            previous.AddMinutes(30));
    }

    private static IEnumerable<string> ToSlotKeys(
        IEnumerable<Availability> availability)
    {
        foreach (var range in availability)
        {
            for (var time = range.StartTime;
                 time < range.EndTime;
                 time = time.AddMinutes(30))
            {
                yield return $"{range.AvailableDate:yyyy-MM-dd}|{time:HH\\:mm}";
            }
        }
    }

    private static DateOnly GetMonday(DateOnly date)
    {
        var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-daysSinceMonday);
    }

    private sealed record AvailabilityRange(
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime);
}
