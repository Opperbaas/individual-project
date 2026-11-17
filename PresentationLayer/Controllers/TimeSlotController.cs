using BusinessLayer.Enums;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class TimeSlotController : Controller
{
    private readonly ITimeSlotService _timeSlotService;

    public TimeSlotController(ITimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
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
    [Authorize(Roles = RoleNames.Organizer)]
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

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public IActionResult Generate(int eventId)
    {
        if (eventId <= 0)
        {
            return BadRequest("Een geldig sportevenement is vereist.");
        }

        return View("~/Views/TimeSlot/Generate.cshtml",
            new GenerateTimeSlotsViewModel { EventId = eventId });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(GenerateTimeSlotsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/TimeSlot/Generate.cshtml", model);
        }

        try
        {
            var slots = await _timeSlotService.GenerateAsync(
                new GenerateTimeSlotsRequest
                {
                    EventId = model.EventId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    DailyStartTime = model.DailyStartTime,
                    DailyEndTime = model.DailyEndTime,
                    IntervalMinutes = model.IntervalMinutes,
                    Days = model.Days
                });

            TempData["SuccessMessage"] =
                $"{slots.Count} tijdsloten aangemaakt.";
            return RedirectToAction(
                "Index",
                "Planning",
                new { eventId = model.EventId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/TimeSlot/Generate.cshtml", model);
        }
    }
}
