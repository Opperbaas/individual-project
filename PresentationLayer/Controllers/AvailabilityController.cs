using System.Security.Claims;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class AvailabilityController : Controller
{
    private readonly AvailabilityService _availabilityService;

    public AvailabilityController(AvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpGet]
    [Authorize(Roles = "Volunteer")]
    public IActionResult Create()
    {
        return View("~/Views/Availability/Create.cshtml");
    }

    [HttpPost]
    [Authorize(Roles = "Volunteer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAvailabilityViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Availability/Create.cshtml", model);
        }

        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Forbid();
        }

        try
        {
            await _availabilityService.CreateAsync(
                userId,
                model.AvailableDate,
                model.StartTime,
                model.EndTime);

            TempData["SuccessMessage"] = "Je beschikbaarheid is opgeslagen.";
            return RedirectToAction("Index", "Home");
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/Availability/Create.cshtml", model);
        }
    }
}
