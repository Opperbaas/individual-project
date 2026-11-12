using System.Security.Claims;
using System.Globalization;
using BusinessLayer.Entities;
using BusinessLayer.Enums;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class AvailabilityController : Controller
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilityController(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Volunteer)]
    public async Task<IActionResult> Create(DateOnly? weekStart)
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Forbid();
        }

        var savedAvailability = await _availabilityService
            .GetByUserIdAsync(userId);

        return View(
            "~/Views/Availability/Create.cshtml",
            CreateCalendarModel(
                weekStart ?? DateOnly.FromDateTime(DateTime.Today),
                ToSlotKeys(savedAvailability)));
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Volunteer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCalendar(AvailabilityCalendarViewModel model)
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Forbid();
        }

        var ranges = ParseRanges(model.SelectedSlots);

        if (!ModelState.IsValid)
        {
            model = CreateCalendarModel(model.WeekStart, model.SelectedSlots);
            return View("~/Views/Availability/Create.cshtml", model);
        }

        try
        {
            await _availabilityService.ReplaceForUserAsync(
                userId,
                ranges.Select(range => new Availability
                {
                    AvailableDate = range.Date,
                    StartTime = range.StartTime,
                    EndTime = range.EndTime
                }));

            TempData["SuccessMessage"] = "Je beschikbaarheid is opgeslagen.";
            return RedirectToAction(nameof(Create));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            model = CreateCalendarModel(model.WeekStart, model.SelectedSlots);
            return View("~/Views/Availability/Create.cshtml", model);
        }
    }

    private static AvailabilityCalendarViewModel CreateCalendarModel(
            DateOnly date,
            IEnumerable<string>? selectedSlots = null)
        {
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
                SelectedSlots = selectedSlots?.ToList() ?? []
            };
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

        private static List<AvailabilityRange> ParseRanges(
            IEnumerable<string> selectedSlots)
        {
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

            var ranges = new List<AvailabilityRange>();
            foreach (var dateSlots in slotsByDate)
            {
                var slots = dateSlots.Value
                    .Distinct()
                    .OrderBy(time => time)
                    .ToList();

                if (slots.Count == 0)
                {
                    continue;
                }

                var rangeStart = slots[0];
                var previous = slots[0];

                foreach (var slot in slots.Skip(1))
                {
                    if (slot != previous.AddMinutes(30))
                    {
                        ranges.Add(new AvailabilityRange(
                            dateSlots.Key,
                            rangeStart,
                            previous.AddMinutes(30)));
                        rangeStart = slot;
                    }

                    previous = slot;
                }

                ranges.Add(new AvailabilityRange(
                    dateSlots.Key,
                    rangeStart,
                    previous.AddMinutes(30)));
            }

            return ranges;
        }

        private sealed record AvailabilityRange(
            DateOnly Date,
            TimeOnly StartTime,
            TimeOnly EndTime);
}
