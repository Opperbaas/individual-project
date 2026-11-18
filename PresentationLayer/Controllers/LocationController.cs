using BusinessLayer.Enums;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class LocationController : Controller
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public IActionResult Create(int eventId = 0)
    {
        return View("~/Views/Location/Create.cshtml",
            new CreateLocationViewModel { EventId = eventId });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLocationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Location/Create.cshtml", model);
        }

        try
        {
            await _locationService.CreateAsync(
                model.Name,
                model.LocationType,
                model.Capacity,
                model.Description);

            TempData["SuccessMessage"] = "De locatie is toegevoegd.";

            if (model.EventId > 0)
            {
                return RedirectToAction(
                    "Manage",
                    "SportEvent",
                    new { id = model.EventId });
            }

            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/Location/Create.cshtml", model);
        }
    }
}
