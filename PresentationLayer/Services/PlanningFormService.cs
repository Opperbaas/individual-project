using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.Interfaces;
using PresentationLayer.Models;

namespace PresentationLayer.Services;

public sealed class PlanningFormService : IPlanningFormService
{
    private readonly IPlanningService _planningService;

    public PlanningFormService(IPlanningService planningService)
    {
        _planningService = planningService;
    }

    public async Task PopulateScheduleOptionsAsync(
        IEnumerable<PlanningScheduleViewModel> schedules,
        int eventId)
    {
        var activities = await _planningService.GetActivitiesByEventIdAsync(eventId);
        var locations = await _planningService.GetLocationsAsync();
        var timeSlots = await _planningService.GetTimeSlotsByEventIdAsync(eventId);
        var volunteers = await _planningService.GetVolunteersAsync();

        foreach (var schedule in schedules)
        {
            schedule.Activities = activities.Select(activity => new SelectListItem
            {
                Value = activity.ActivityId.ToString(),
                Text = activity.ActivityName,
                Selected = activity.ActivityId == schedule.ActivityId
            });
            schedule.Locations = locations.Select(location => new SelectListItem
            {
                Value = location.LocationId.ToString(),
                Text = location.Name,
                Selected = location.LocationId == schedule.LocationId
            });
            schedule.TimeSlots = timeSlots.Select(timeSlot => new SelectListItem
            {
                Value = timeSlot.TimeSlotId.ToString(),
                Text =
                    $"{timeSlot.SlotDate:dd-MM-yyyy} " +
                    $"{timeSlot.StartTime:HH\\:mm}-{timeSlot.EndTime:HH\\:mm}",
                Selected = timeSlot.TimeSlotId == schedule.TimeSlotId
            });
            schedule.Volunteers = volunteers.Select(volunteer => new SelectListItem
            {
                Value = volunteer.UserId.ToString(),
                Text = volunteer.Name,
                Selected = schedule.VolunteerIds.Contains(volunteer.UserId)
            });
        }
    }
}
