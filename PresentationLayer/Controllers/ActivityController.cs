using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public sealed class ActivityController : Controller
{
    private readonly ActivityService _activityService;

    public ActivityController(ActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpGet]
    [Authorize(Roles = "Organizer")]
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
    [Authorize(Roles = "Organizer")]
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
}
