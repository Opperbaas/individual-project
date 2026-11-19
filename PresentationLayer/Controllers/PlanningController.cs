using System.Security.Claims;
using BusinessLayer.DTOs;
using BusinessLayer.Enums;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Interfaces;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class PlanningController : Controller
{
    private readonly IPlanningService _planningService;
    private readonly IPlanningWorkflowService _planningWorkflowService;
    private readonly IPlanningFormService _planningFormService;
    private readonly IVolunteerAssignmentService _volunteerAssignmentService;

    public PlanningController(
        IPlanningService planningService,
        IPlanningWorkflowService planningWorkflowService,
        IPlanningFormService planningFormService,
        IVolunteerAssignmentService volunteerAssignmentService)
    {
        _planningService = planningService;
        _planningWorkflowService = planningWorkflowService;
        _planningFormService = planningFormService;
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

        await _planningFormService.PopulateScheduleOptionsAsync(
            model.Schedules,
            eventId);
        return View("~/Views/PlanningPage/Create.cshtml", model);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePlanningViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await _planningFormService.PopulateScheduleOptionsAsync(
                model.Schedules,
                model.EventId);
            return View("~/Views/PlanningPage/Create.cshtml", model);
        }

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Forbid();
        }

        try
        {
            var request = new CreatePlanningRequest
            {
                EventId = model.EventId,
                CreatedByUserId = userId,
                Name = model.Name,
                Schedules = model.Schedules
                    .Select(schedule => new CreatePlanningScheduleRequest
                    {
                        ActivityId = schedule.ActivityId,
                        LocationId = schedule.LocationId,
                        TimeSlotId = schedule.TimeSlotId,
                        VolunteerIds = schedule.VolunteerIds
                    })
                    .ToList()
            };

            var planning = await _planningWorkflowService.CreateAsync(request);

            return RedirectToAction(nameof(Details), new { id = planning.PlanningId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await _planningFormService.PopulateScheduleOptionsAsync(
                model.Schedules,
                model.EventId);
            return View("~/Views/PlanningPage/Create.cshtml", model);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await _planningFormService.PopulateScheduleOptionsAsync(
                model.Schedules,
                model.EventId);
            return View("~/Views/PlanningPage/Create.cshtml", model);
        }
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateOptimized(
        int eventId,
        string name,
        CancellationToken cancellationToken)
    {
        if (eventId <= 0)
        {
            return BadRequest();
        }

        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Forbid();
        }

        try
        {
            var planning = await _planningWorkflowService.GenerateOptimizedAsync(
                eventId,
                userId,
                string.IsNullOrWhiteSpace(name) ? "Geoptimaliseerde planning" : name,
                cancellationToken);

            return RedirectToAction(nameof(Details), new { id = planning.PlanningId });
        }
        catch (ArgumentException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Index), new { eventId });
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Index), new { eventId });
        }
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateWithAi(
        int eventId,
        string name,
        CancellationToken cancellationToken)
    {
        if (eventId <= 0)
        {
            return BadRequest();
        }

        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Forbid();
        }

        try
        {
            var planning = await _planningWorkflowService.GenerateWithAiAsync(
                eventId,
                userId,
                string.IsNullOrWhiteSpace(name) ? "AI-planning" : name,
                cancellationToken);

            return RedirectToAction(nameof(Details), new { id = planning.PlanningId });
        }
        catch (ArgumentException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Index), new { eventId });
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Index), new { eventId });
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

        var model = new EditPlanningViewModel
        {
            PlanningId = planning.PlanningId,
            EventId = planning.EventId,
            EventName = planning.Event.EventName,
            Name = planning.Name,
            Schedules = planning.ActivitySchedules
                .Select(schedule => new PlanningScheduleViewModel
                {
                    ActivityScheduleId = schedule.ActivityScheduleId,
                    ActivityId = schedule.ActivityId,
                    LocationId = schedule.LocationId,
                    TimeSlotId = schedule.TimeSlotId,
                    VolunteerIds = schedule.VolunteerAssignments
                        .Select(assignment => assignment.UserId)
                        .ToList()
                })
                .ToList()
        };

        await _planningFormService.PopulateScheduleOptionsAsync(
            model.Schedules,
            model.EventId);
        return View("~/Views/PlanningPage/Edit.cshtml", model);
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

        try
        {
            var scheduleRequests = planning.ActivitySchedules
                .Select(schedule =>
                {
                    var submittedSchedule = model.Schedules
                        .SingleOrDefault(item =>
                            item.ActivityScheduleId ==
                            schedule.ActivityScheduleId);

                    return new VolunteerAssignmentRequest
                    {
                        ActivityScheduleId = schedule.ActivityScheduleId,
                        TimeSlotId = schedule.TimeSlotId,
                        VolunteerIds = submittedSchedule?.VolunteerIds ?? []
                    };
                })
                .ToList();

            await _volunteerAssignmentService.ReplaceAsync(scheduleRequests);
            return RedirectToAction(nameof(Details), new { id = planning.PlanningId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await _planningFormService.PopulateScheduleOptionsAsync(
                model.Schedules,
                model.EventId);
            return View("~/Views/PlanningPage/Edit.cshtml", model);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await _planningFormService.PopulateScheduleOptionsAsync(
                model.Schedules,
                model.EventId);
            return View("~/Views/PlanningPage/Edit.cshtml", model);
        }
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        try
        {
            var planning = await _planningService.GetByIdAsync(id);
            if (planning is null)
            {
                return NotFound();
            }

            await _planningService.DeleteAsync(id);
            return RedirectToAction(
                nameof(Index),
                new { eventId = planning.EventId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}