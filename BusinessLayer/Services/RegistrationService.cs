using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class RegistrationService : IRegistrationService
{
    private readonly IRegistrationRepository _registrationRepository;

    public RegistrationService(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    public async Task<Registration> RegisterAsync(int userId, int activityId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("De ingelogde gebruiker is ongeldig.", nameof(userId));
        }

        if (activityId <= 0)
        {
            throw new ArgumentException("De activiteit is ongeldig.", nameof(activityId));
        }

        var activity = await _registrationRepository
            .GetActivityForRegistrationAsync(activityId);

        if (activity is null)
        {
            throw new InvalidOperationException("De activiteit bestaat niet.");
        }

        if (await _registrationRepository.ExistsAsync(userId, activityId))
        {
            throw new InvalidOperationException(
                "Je bent al geregistreerd voor deze activiteit.");
        }

        var participantCount = activity.Registrations.Count;
        if (participantCount >= activity.MaxParticipants)
        {
            throw new InvalidOperationException(
                "Deze activiteit zit vol.");
        }

        return await _registrationRepository.CreateAsync(new Registration
        {
            UserId = userId,
            ActivityId = activityId,
            RegisteredAt = DateTime.UtcNow,
            Status = "Confirmed"
        });
    }
}
