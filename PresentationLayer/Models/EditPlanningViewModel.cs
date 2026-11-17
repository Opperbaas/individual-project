using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models;

public sealed class EditPlanningViewModel
{
    [Required]
    public int PlanningId { get; set; }

    [Required]
    public int EventId { get; set; }

    public string EventName { get; set; } = "";
    public string Name { get; set; } = "";

    public List<PlanningScheduleViewModel> Schedules { get; set; } = [];
}
