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

    public PlanningController(
        IPlanningService planningService,
        IPlanningWorkflowService planningWorkflowService,
        IPlanningFormService planningFormService)
    {
        _planningService = planningService;
        _planningWorkflowService = planningWorkflowService;
        _planningFormService = planningFormService;
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