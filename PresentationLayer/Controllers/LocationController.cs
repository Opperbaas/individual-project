using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class LocationController : Controller
{
    private readonly LocationService _locationService;

    public LocationController(LocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    [Authorize(Roles = "Organizer")]
    public IActionResult Create()
    {
        return View("~/Views/Location/Create.cshtml");
    }

    [HttpPost]
    [Authorize(Roles = "Organizer")]
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
            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/Location/Create.cshtml", model);
        }
    }
}
