using BusinessLayer.Enums;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class SportEventController : Controller
{
    private readonly ISportEventService _sportEventService;
    private readonly IPlanningService _planningService;
    private readonly IActivityService _activityService;

    public SportEventController(
        ISportEventService sportEventService,
        IPlanningService planningService,
        IActivityService activityService)
    {
        _sportEventService = sportEventService;
        _planningService = planningService;
        _activityService = activityService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sportEvents = await _sportEventService.GetAllAsync();
        var model = sportEvents
            .Select(sportEvent => new SportEventViewModel
            {
                EventId = sportEvent.EventId,
                EventName = sportEvent.EventName,
                Description = sportEvent.Description,
                StartDate = sportEvent.StartDate,
                EndDate = sportEvent.EndDate
            })
            .ToList();

        return View("~/Views/SportEvent/Index.cshtml", model);
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public async Task<IActionResult> Manage(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var sportEvent = await _sportEventService.GetByIdAsync(id);
        if (sportEvent is null)
        {
            return NotFound();
        }

        var activities =
            await _planningService.GetActivitiesByEventIdAsync(id);
        var locations = await _planningService.GetLocationsAsync();
        var timeSlots =
            await _planningService.GetTimeSlotsByEventIdAsync(id);

        var model = new ManageSportEventViewModel
        {
            EventId = sportEvent.EventId,
            EventName = sportEvent.EventName,
            Description = sportEvent.Description,
            StartDate = sportEvent.StartDate,
            EndDate = sportEvent.EndDate,
            Activities = activities,
            Locations = locations,
            TimeSlots = timeSlots
        };

        return View("~/Views/SportEvent/Manage.cshtml", model);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateActivities(
        GenerateActivitiesViewModel model)
    {
        if (model.EventId <= 0)
        {
            TempData["ErrorMessage"] = "Een geldig evenement is vereist.";
            return RedirectToAction(nameof(Manage), new { id = model.EventId });
        }

        if (model.ActivityRows.Count == 0)
        {
            TempData["ErrorMessage"] =
                "Voeg minimaal één rij toe om activiteiten te genereren.";
            return RedirectToAction(nameof(Manage), new { id = model.EventId });
        }

        var locations = await _planningService.GetLocationsAsync();

        try
        {
            var generatedCount = 0;

            foreach (var row in model.ActivityRows)
            {
                var location = locations.SingleOrDefault(
                    item => item.LocationId == row.LocationId);

                if (location is null)
                {
                    throw new ArgumentException(
                        "Kies voor iedere rij een bestaande locatie.");
                }

                if (!string.Equals(
                        location.LocationType,
                        row.LocationType,
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(
                        "De gekozen locatie past niet bij het gekozen locatietype.");
                }

                var activities = await _activityService.GenerateAsync(
                    model.EventId,
                    row.ExecutionType,
                    row.LocationType,
                    location.Name,
                    location.Capacity,
                    location.Capacity,
                    row.Count);

                generatedCount += activities.Count;
            }

            TempData["SuccessMessage"] =
                $"{generatedCount} activiteit(en) gegenereerd.";
        }
        catch (ArgumentException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }

        return RedirectToAction(nameof(Manage), new { id = model.EventId });
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public IActionResult Create()
    {
        return View("~/Views/SportEvent/Create.cshtml");
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSportEventViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/SportEvent/Create.cshtml", model);
        }

        try
        {
            var sportEvent = await _sportEventService.CreateAsync(
                model.EventName,
                model.Description,
                model.StartDate,
                model.EndDate);

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/SportEvent/Create.cshtml", model);
        }
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public async Task<IActionResult> Edit(int id)
    {
        var sportEvent = await _sportEventService.GetByIdAsync(id);
        if (sportEvent is null)
        {
            return NotFound();
        }

        return View("~/Views/SportEvent/Edit.cshtml",
            new CreateSportEventViewModel
            {
                EventId = sportEvent.EventId,
                EventName = sportEvent.EventName,
                Description = sportEvent.Description,
                StartDate = sportEvent.StartDate,
                EndDate = sportEvent.EndDate
            });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CreateSportEventViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/SportEvent/Edit.cshtml", model);
        }

        try
        {
            await _sportEventService.UpdateAsync(
                model.EventId,
                model.EventName,
                model.Description,
                model.StartDate,
                model.EndDate);
            return RedirectToAction(nameof(Manage), new { id = model.EventId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/SportEvent/Edit.cshtml", model);
        }
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _sportEventService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Het evenement is verwijderd.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Manage), new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
