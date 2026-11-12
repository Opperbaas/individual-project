using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ISportEventService
{
    Task<IReadOnlyList<SportEvent>> GetAllAsync();
    Task<SportEvent?> GetByIdAsync(int eventId);
    Task<SportEvent> CreateAsync(
        string eventName,
        string description,
        DateTime startDate,
        DateTime endDate);
}
