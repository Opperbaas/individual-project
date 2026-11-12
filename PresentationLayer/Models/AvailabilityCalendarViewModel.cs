using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public sealed class AvailabilityCalendarViewModel
{
    [Required]
    public DateOnly WeekStart { get; set; }

    public IReadOnlyList<AvailabilityCalendarDayViewModel> Days { get; set; } =
        [];

    public List<string> SelectedSlots { get; set; } = [];
}

public sealed class AvailabilityCalendarDayViewModel
{
    public DateOnly Date { get; init; }
    public string DayName { get; init; } = "";
    public bool IsToday { get; init; }
}
