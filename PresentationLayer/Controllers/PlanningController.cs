using System.Security.Claims;
using BusinessLayer.Entities;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class PlanningController : Controller
{
    private readonly PlanningService _planningService;

    public PlanningController(PlanningService planningService)
    {
        _planningService = planningService;
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
    [Authorize(Roles = "Organizer")]
    public IActionResult Create(int eventId)
    {
        if (eventId <= 0)
        {
            return BadRequest();
        }

        return View("~/Views/PlanningPage/Create.cshtml", new CreatePlanningViewModel
        {
            EventId = eventId
        });
    }

    [HttpPost]
    [Authorize(Roles = "Organizer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePlanningViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/PlanningPage/Create.cshtml", model);
        }

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Forbid();
        }

        try
        {
            var planning = await _planningService.CreateAsync(new Planning
            {
                EventId = model.EventId,
                CreatedByUserId = userId,
                Name = model.Name
            });

            return RedirectToAction(nameof(Details), new { id = planning.PlanningId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/PlanningPage/Create.cshtml", model);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Organizer")]
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
    [Authorize(Roles = "Organizer")]
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