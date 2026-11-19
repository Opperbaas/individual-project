using BusinessLayer.Interfaces;
using BusinessLayer.Enums;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class ActivityController : Controller
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public IActionResult Create(int eventId)
    {
        if (eventId <= 0)
        {
            return BadRequest("Een geldig sportevenement is vereist.");
        }

        return View("~/Views/Activity/Create.cshtml",
            new CreateActivityViewModel { EventId = eventId });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateActivityViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Activity/Create.cshtml", model);
        }

        try
        {
            await _activityService.CreateAsync(
                model.EventId,
                model.ActivityName,
                model.ActivityType,
                model.Description,
                model.MaxParticipants);

            return RedirectToAction(
                "Index",
                "Planning",
                new { eventId = model.EventId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/Activity/Create.cshtml", model);
        }
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Organizer)]
    public async Task<IActionResult> Edit(int id)
    {
        var activity = await _activityService.GetByIdAsync(id);
        if (activity is null)
        {
            return NotFound();
        }

        return View("~/Views/Activity/Edit.cshtml",
            new CreateActivityViewModel
            {
                EventId = activity.EventId,
                ActivityId = id,
                ActivityName = activity.ActivityName,
                ActivityType = activity.ActivityType,
                Description = activity.Description,
                MaxParticipants = activity.MaxParticipants
            });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CreateActivityViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Activity/Edit.cshtml", model);
        }

        try
        {
            await _activityService.UpdateAsync(
                model.ActivityId,
                model.EventId,
                model.ActivityName,
                model.ActivityType,
                model.Description,
                model.MaxParticipants);
            return RedirectToAction(
                "Manage",
                "SportEvent",
                new { id = model.EventId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("~/Views/Activity/Edit.cshtml", model);
        }
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Organizer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var activity = await _activityService.GetByIdAsync(id);
        if (activity is null)
        {
            return NotFound();
        }

        try
        {
            await _activityService.DeleteAsync(id);
            TempData["SuccessMessage"] = "De activiteit is verwijderd.";
            return RedirectToAction(
                "Manage",
                "SportEvent",
                new { id = activity.EventId });
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(
                "Manage",
                "SportEvent",
                new { id = activity.EventId });
        }
    }
}
