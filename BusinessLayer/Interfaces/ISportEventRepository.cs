using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ISportEventRepository
{
    Task<IReadOnlyList<SportEvent>> GetAllAsync();
    Task<SportEvent?> GetByIdAsync(int eventId);
    Task<SportEvent> CreateAsync(SportEvent sportEvent);
}
