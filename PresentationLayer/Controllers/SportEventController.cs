using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class SportEventController : Controller
{
    private readonly SportEventService _sportEventService;

    public SportEventController(SportEventService sportEventService)
    {
        _sportEventService = sportEventService;
    }

    [HttpGet]
    [Authorize(Roles = "Organizer,Participant,Volunteer")]
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
    [Authorize(Roles = "Organizer")]
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

        var model = new SportEventViewModel
        {
            EventId = sportEvent.EventId,
            EventName = sportEvent.EventName,
            Description = sportEvent.Description,
            StartDate = sportEvent.StartDate,
            EndDate = sportEvent.EndDate
        };

        return View("~/Views/SportEvent/Manage.cshtml", model);
    }

    [HttpGet]
    [Authorize(Roles = "Organizer")]
    public IActionResult Create()
    {
        return View("~/Views/SportEvent/Create.cshtml");
    }

    [HttpPost]
    [Authorize(Roles = "Organizer")]
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
}
