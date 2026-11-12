using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IAvailabilityRepository
{
    Task<IReadOnlyList<Availability>> GetByUserIdAsync(int userId);
    Task ReplaceForUserAsync(
        int userId,
        IEnumerable<Availability> availability);
    Task<Availability> CreateAsync(Availability availability);
}
