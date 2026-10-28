using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IPlanningRepository
{
    Task<Planning?> GetByIdAsync(int planningId);
    Task<IReadOnlyList<Planning>> GetByEventIdAsync(int eventId);
    Task<Planning> CreateAsync(Planning planning);
    Task UpdateAsync(Planning planning);
}