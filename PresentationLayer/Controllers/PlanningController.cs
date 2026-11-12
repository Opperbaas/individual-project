using System.Security.Claims;
using BusinessLayer.DTOs;
using BusinessLayer.Entities;
using BusinessLayer.Enums;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class PlanningController : Controller
{
    private readonly PlanningService _planningService;
    private readonly VolunteerAssignmentService _volunteerAssignmentService;

    public PlanningController(
        PlanningService planningService,
        VolunteerAssignmentService volunteerAssignmentService)
    {
        _planningService = planningService;
        _volunteerAssignmentService = volunteerAssignmentService;
    }

    public async Task<IActionResult> Index(int eventId)
    {
        if (eventId <= 0)
        {
            return BadRequest();
        }

        var plannings = await _planningService.GetByEventIdAsync(eventId);
        return View("~/Views/PlanningPage/Index.cshtml", plannings);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var planning = await _planningService.GetByIdAsync(id);
        if (planning is null)
        {
            return NotFound();
        }

        return View("~/Views/PlanningPage/Details.cshtml", planning);
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public async Task<IActionResult> Create(int eventId)
    {
        if (eventId <= 0)
        {
            return BadRequest();
        }

        var model = new CreatePlanningViewModel
        {
            EventId = eventId,
            Schedules = [new PlanningScheduleViewModel()]
        };

        await PopulateScheduleOptionsAsync(model.Schedules, eventId);
        return View("~/Views/PlanningPage/Create.cshtml", model);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePlanningViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateScheduleOptionsAsync(model.Schedules, model.EventId);
            return View("~/Views/PlanningPage/Create.cshtml", model);
        }

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Forbid();
        }

        try
        {
            var planning = new Planning
            {
                EventId = model.EventId,
                CreatedByUserId = userId,
                Name = model.Name
            };

            var activities = await _planningService.GetActivitiesByEventIdAsync(model.EventId);
            var locations = await _planningService.GetLocationsAsync();
            var timeSlots = await _planningService.GetTimeSlotsByEventIdAsync(model.EventId);

            foreach (var scheduleModel in model.Schedules)
            {
                var activity = activities.SingleOrDefault(
                    item => item.ActivityId == scheduleModel.ActivityId);
                var location = locations.SingleOrDefault(
                    item => item.LocationId == scheduleModel.LocationId);
                var timeSlot = timeSlots.SingleOrDefault(
                    item => item.TimeSlotId == scheduleModel.TimeSlotId);

                if (activity is null || location is null || timeSlot is null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Kies een geldige activiteit, locatie en tijdslot.");
                    await PopulateScheduleOptionsAsync(model.Schedules, model.EventId);
                    return View("~/Views/PlanningPage/Create.cshtml", model);
                }

                planning.ActivitySchedules.Add(new ActivitySchedule
                {
                    Activity = activity,
                    Location = location,
                    TimeSlot = timeSlot
                });
            }

            planning = await _planningService.CreateAsync(planning);

            var assignmentRequests = planning.ActivitySchedules
                .Zip(model.Schedules, (activitySchedule, schedule) =>
                    new VolunteerAssignmentRequest
                    {
                        ActivityScheduleId = activitySchedule.ActivityScheduleId,
                        TimeSlotId = schedule.TimeSlotId,
                        VolunteerIds = schedule.VolunteerIds
                    })
                .ToList();

            await _volunteerAssignmentService.ValidateAsync(assignmentRequests);
            await _volunteerAssignmentService.AssignAsync(assignmentRequests);

            return RedirectToAction(nameof(Details), new { id = planning.PlanningId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await PopulateScheduleOptionsAsync(model.Schedules, model.EventId);
            return View("~/Views/PlanningPage/Create.cshtml", model);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await PopulateScheduleOptionsAsync(model.Schedules, model.EventId);
            return View("~/Views/PlanningPage/Create.cshtml", model);
        }
    }

    private async Task PopulateScheduleOptionsAsync(
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
                Text = $"{timeSlot.SlotDate:dd-MM-yyyy} {timeSlot.StartTime:HH\\:mm}-{timeSlot.EndTime:HH\\:mm}",
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

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var planning = await _planningService.GetByIdAsync(id);
        if (planning is null)
        {
            return NotFound();
        }

        return View("~/Views/PlanningPage/Edit.cshtml", new EditPlanningViewModel
        {
            PlanningId = planning.PlanningId,
            EventId = planning.EventId,
            Name = planning.Name
        });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditPlanningViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/PlanningPage/Edit.cshtml", model);
        }

        var planning = await _planningService.GetByIdAsync(model.PlanningId);
        if (planning is null)
        {
            return NotFound();
        }

        planning.EventId = model.EventId;
        planning.Name = model.Name;

        try
        {
            await _planningService.UpdateAsync(planning);
            return RedirectToAction(nameof(Details), new { id = planning.PlanningId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/PlanningPage/Edit.cshtml", model);
        }
    }
}