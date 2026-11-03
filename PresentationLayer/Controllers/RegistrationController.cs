using System.Security.Claims;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers;

public sealed class RegistrationController : Controller
{
    private readonly RegistrationService _registrationService;

    public RegistrationController(RegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost]
    [Authorize(Roles = "Participant")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int activityId, int planningId)
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
        {
            return Forbid();
        }

        try
        {
            await _registrationService.RegisterAsync(userId, activityId);
            TempData["SuccessMessage"] = "Je bent geregistreerd voor de activiteit.";
        }
        catch (ArgumentException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }

        return RedirectToAction(
            "Details",
            "Planning",
            new { id = planningId });
    }
}
