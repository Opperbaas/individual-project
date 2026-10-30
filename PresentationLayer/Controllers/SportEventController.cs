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

            return RedirectToAction(
                "Index",
                "Planning",
                new { eventId = sportEvent.EventId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/SportEvent/Create.cshtml", model);
        }
    }
}
