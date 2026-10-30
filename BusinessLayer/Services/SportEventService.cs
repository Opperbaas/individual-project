using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class SportEventService
{
    private readonly ISportEventRepository _sportEventRepository;

    public SportEventService(ISportEventRepository sportEventRepository)
    {
        _sportEventRepository = sportEventRepository;
    }

    public Task<SportEvent> CreateAsync(
        string eventName,
        string description,
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

        return _sportEventRepository.CreateAsync(new SportEvent
        {
            EventName = eventName.Trim(),
            Description = description?.Trim() ?? "",
            StartDate = startDate,
            EndDate = endDate
        });
    }
}
