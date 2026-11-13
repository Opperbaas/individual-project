using System.Security.Claims;
using BusinessLayer.Enums;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Interfaces;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class AvailabilityController : Controller
{
    private readonly IAvailabilityService _availabilityService;
    private readonly IAvailabilityCalendarService _calendarService;

    public AvailabilityController(
        IAvailabilityService availabilityService,
        IAvailabilityCalendarService calendarService)
    {
        _availabilityService = availabilityService;
        _calendarService = calendarService;
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Volunteer)]
    public async Task<IActionResult> Create(DateOnly? weekStart)
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Forbid();
        }

        var savedAvailability = await _availabilityService
            .GetByUserIdAsync(userId);

        return View(
            "~/Views/Availability/Create.cshtml",
            _calendarService.CreateModel(
                weekStart ?? DateOnly.FromDateTime(DateTime.Today),
                savedAvailability));
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Volunteer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCalendar(AvailabilityCalendarViewModel model)
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Forbid();
        }

        var availability = _calendarService.ParseAvailability(
            model.SelectedSlots);

        if (!ModelState.IsValid)
        {
            model = _calendarService.CreateModel(
                model.WeekStart,
                availability);
            return View("~/Views/Availability/Create.cshtml", model);
        }

        try
        {
            await _availabilityService.ReplaceForUserAsync(
                userId,
                availability);

            TempData["SuccessMessage"] = "Je beschikbaarheid is opgeslagen.";
            return RedirectToAction(nameof(Create));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            model = _calendarService.CreateModel(
                model.WeekStart,
                availability);
            return View("~/Views/Availability/Create.cshtml", model);
        }
    }
}
