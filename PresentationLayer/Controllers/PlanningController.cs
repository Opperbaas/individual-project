using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

public sealed class PlanningController : Controller
{
    private readonly PlanningService _planningService;

    public PlanningController(PlanningService planningService)
    {
        _planningService = planningService;
    }

    public async Task<IActionResult> Index(int eventId)
    {
        if (eventId <= 0)
        {
            return BadRequest();
        }

        var plannings = await _planningService.GetByEventIdAsync(eventId);
        return View("PlanningPage/Index", plannings);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var planning = await _planningService.GetByIdAsync(id);
        if (planning is null)
        {
            return NotFound();
        }

        return View("PlanningPage/Details", planning);
    }
}