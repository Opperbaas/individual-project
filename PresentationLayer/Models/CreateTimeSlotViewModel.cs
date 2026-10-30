using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public sealed class CreateTimeSlotViewModel
{
    public int EventId { get; set; }

    [Required]
    [Display(Name = "Datum")]
    public DateOnly SlotDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [Display(Name = "Begintijd")]
    public TimeOnly StartTime { get; set; } = new(9, 0);

    [Required]
    [Display(Name = "Eindtijd")]
    public TimeOnly EndTime { get; set; } = new(10, 0);
}
