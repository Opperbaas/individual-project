using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class TimeSlotController : Controller
{
    private readonly TimeSlotService _timeSlotService;

    public TimeSlotController(TimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    [HttpGet]
    [Authorize(Roles = "Organizer")]
    public IActionResult Create(int eventId)
    {
        if (eventId <= 0)
        {
            return BadRequest("Een geldig sportevenement is vereist.");
        }

        return View("~/Views/TimeSlot/Create.cshtml",
            new CreateTimeSlotViewModel { EventId = eventId });
    }

    [HttpPost]
    [Authorize(Roles = "Organizer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTimeSlotViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/TimeSlot/Create.cshtml", model);
        }

        try
        {
            await _timeSlotService.CreateAsync(
                model.EventId,
                model.SlotDate,
                model.StartTime,
                model.EndTime);

            TempData["SuccessMessage"] = "Het tijdslot is toegevoegd.";
            return RedirectToAction(
                "Index",
                "Planning",
                new { eventId = model.EventId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/TimeSlot/Create.cshtml", model);
        }
    }
}
