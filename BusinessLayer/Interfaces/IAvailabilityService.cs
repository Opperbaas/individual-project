using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IAvailabilityService
{
    Task<IReadOnlyList<Availability>> GetByUserIdAsync(int userId);
    Task ReplaceForUserAsync(
        int userId,
        IEnumerable<Availability> availability);

    Task<Availability> CreateAsync(
        int userId,
        DateOnly availableDate,
        TimeOnly startTime,
        TimeOnly endTime);
}
