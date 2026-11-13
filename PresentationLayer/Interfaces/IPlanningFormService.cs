using PresentationLayer.Models;

namespace PresentationLayer.Interfaces;

public interface IPlanningFormService
{
    Task PopulateScheduleOptionsAsync(
        IEnumerable<PlanningScheduleViewModel> schedules,
        int eventId);
}
