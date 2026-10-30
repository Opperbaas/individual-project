using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public sealed class CreatePlanningViewModel
{
    [Required]
    public int EventId { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Naam")]
    public string Name { get; set; } = "";

    public List<PlanningScheduleViewModel> Schedules { get; set; } = [];
}
