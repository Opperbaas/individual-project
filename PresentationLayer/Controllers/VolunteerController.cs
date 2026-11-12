using System.Security.Claims;
using BusinessLayer.Enums;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

[Authorize(Roles = RoleNames.Volunteer)]
public sealed class VolunteerController : Controller
{
    private readonly IVolunteerAssignmentService _volunteerAssignmentService;

    public VolunteerController(
        IVolunteerAssignmentService volunteerAssignmentService)
    {
        _volunteerAssignmentService = volunteerAssignmentService;
    }

    public async Task<IActionResult> Index()
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Forbid();
        }

        var plannings = await _volunteerAssignmentService
            .GetPlanningsByVolunteerIdAsync(userId);

        return View(plannings);
    }
}
