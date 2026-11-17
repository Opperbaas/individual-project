using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public sealed class GenerateTimeSlotsViewModel
{
    public int EventId { get; set; }

    [Required]
    [Display(Name = "Vanaf")]
    public DateOnly StartDate { get; set; } =
        DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [Display(Name = "Tot en met")]
    public DateOnly EndDate { get; set; } =
        DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [Display(Name = "Dagelijkse begintijd")]
    public TimeOnly DailyStartTime { get; set; } = new(9, 0);

    [Required]
    [Display(Name = "Dagelijkse eindtijd")]
    public TimeOnly DailyEndTime { get; set; } = new(17, 0);

    [Range(1, 1440)]
    [Display(Name = "Interval in minuten")]
    public int IntervalMinutes { get; set; } = 30;

    [Display(Name = "Dagen")]
    public List<DayOfWeek> Days { get; set; } =
        [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
         DayOfWeek.Thursday, DayOfWeek.Friday];
}
