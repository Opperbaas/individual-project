using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class SportEventService : ISportEventService
{
    private readonly ISportEventRepository _sportEventRepository;

    public SportEventService(ISportEventRepository sportEventRepository)
    {
        _sportEventRepository = sportEventRepository;
    }

    public Task<IReadOnlyList<SportEvent>> GetAllAsync()
    {
        return _sportEventRepository.GetAllAsync();
    }

    public Task<SportEvent?> GetByIdAsync(int eventId)
    {
        if (eventId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(eventId));
        }

        return _sportEventRepository.GetByIdAsync(eventId);
    }

    public Task<SportEvent> CreateAsync(
        string eventName,
        string description,
        DateTime startDate,
        DateTime endDate)
    {
        ValidateSportEvent(
            eventName,
            startDate,
            endDate);

        return _sportEventRepository.CreateAsync(new SportEvent
        {
            EventName = eventName.Trim(),
            Description = description?.Trim() ?? "",
            StartDate = startDate,
            EndDate = endDate
        });
    }

    public async Task UpdateAsync(
        int eventId,
        string eventName,
        string description,
        DateTime startDate,
        DateTime endDate)
    {
        if (eventId <= 0)
        {
            throw new ArgumentException("Het sportevenement is ongeldig.");
        }

        ValidateSportEvent(
            eventName,
            startDate,
            endDate);

        await _sportEventRepository.UpdateAsync(new SportEvent
        {
            EventId = eventId,
            EventName = eventName.Trim(),
            Description = description?.Trim() ?? "",
            StartDate = startDate,
            EndDate = endDate
        });
    }

    private static void ValidateSportEvent(
        string eventName,
        DateTime startDate,
        DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            throw new ArgumentException(
                "Een sportevenement moet een naam hebben.",
                nameof(eventName));
        }

        if (startDate > endDate)
        {
            throw new ArgumentException(
                "De begindatum mag niet na de einddatum liggen.");
        }
    }

    public Task DeleteAsync(int eventId)
    {
        if (eventId <= 0)
        {
            throw new ArgumentException("Het sportevenement is ongeldig.");
        }

        return _sportEventRepository.DeleteAsync(eventId);
    }
}
