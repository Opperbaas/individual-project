using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IPlanningService
{
    Task<Planning?> GetByIdAsync(int planningId);
    Task<IReadOnlyList<Planning>> GetByEventIdAsync(int eventId);
    Task<IReadOnlyList<Activity>> GetActivitiesByEventIdAsync(int eventId);
    Task<IReadOnlyList<Location>> GetLocationsAsync();
    Task<IReadOnlyList<TimeSlot>> GetTimeSlotsByEventIdAsync(int eventId);
    Task<IReadOnlyList<User>> GetVolunteersAsync();
    Task<Planning> CreateAsync(Planning planning);
    Task UpdateAsync(Planning planning);
    Task<IReadOnlyList<string>> ValidateAsync(int planningId);
    Task FinalizeAsync(int planningId);
}
