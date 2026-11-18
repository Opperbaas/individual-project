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

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public async Task<IActionResult> Edit(int id)
    {
        var location = await _locationService.GetByIdAsync(id);
        if (location is null)
        {
            return NotFound();
        }

        return View("~/Views/Location/Edit.cshtml",
            new CreateLocationViewModel
            {
                LocationId = id,
                Name = location.Name,
                LocationType = location.LocationType,
                Capacity = location.Capacity,
                Description = location.Description
            });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CreateLocationViewModel model)
    {
        try
        {
            await _locationService.UpdateAsync(
                model.LocationId,
                model.Name,
                model.LocationType,
                model.Capacity,
                model.Description);
            TempData["SuccessMessage"] = "De locatie is aangepast.";
            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/Location/Edit.cshtml", model);
        }
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _locationService.DeleteAsync(id);
            TempData["SuccessMessage"] = "De locatie is verwijderd.";
            return RedirectToAction("Index", "Home");
        }
        catch (InvalidOperationException exception)
        {
            TempData["SuccessMessage"] = exception.Message;
            return RedirectToAction("Index", "Home");
        }
    }
}
