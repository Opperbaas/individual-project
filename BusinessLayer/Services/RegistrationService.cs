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

    public async Task<Registration> RegisterAsync(int userId, int activityScheduleId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("De ingelogde gebruiker is ongeldig.", nameof(userId));
        }

        if (activityScheduleId <= 0)
        {
            throw new ArgumentException("De geplande activiteit is ongeldig.", nameof(activityScheduleId));
        }

        var schedule = await _registrationRepository
            .GetActivityScheduleForRegistrationAsync(activityScheduleId);

        if (schedule is null)
        {
            throw new InvalidOperationException("De geplande activiteit bestaat niet.");
        }

        if (await _registrationRepository.ExistsAsync(userId, activityScheduleId))
        {
            throw new InvalidOperationException(
                "Je bent al geregistreerd voor deze activiteit.");
        }

        var participantCount = schedule.Registrations.Count;
        if (participantCount >= schedule.Activity.MaxParticipants)
        {
            throw new InvalidOperationException(
                "Deze activiteit zit vol.");
        }

        return await _registrationRepository.CreateAsync(new Registration
        {
            UserId = userId,
            ActivityScheduleId = activityScheduleId,
            RegisteredAt = DateTime.UtcNow,
            Status = "Registered"
        });
    }
}
