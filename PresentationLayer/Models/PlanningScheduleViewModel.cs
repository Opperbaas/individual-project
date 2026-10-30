using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PresentationLayer.Models;

public sealed class PlanningScheduleViewModel
{
    public int ActivityScheduleId { get; set; }

    [Required]
    [Display(Name = "Activiteit")]
    public int ActivityId { get; set; }

    [Required]
    [Display(Name = "Locatie")]
    public int LocationId { get; set; }

    [Required]
    [Display(Name = "Tijdslot")]
    public int TimeSlotId { get; set; }

    [Display(Name = "Vrijwilligers")]
    public List<int> VolunteerIds { get; set; } = [];

    public IEnumerable<SelectListItem> Activities { get; set; } = [];
    public IEnumerable<SelectListItem> Locations { get; set; } = [];
    public IEnumerable<SelectListItem> TimeSlots { get; set; } = [];
    public IEnumerable<SelectListItem> Volunteers { get; set; } = [];
}