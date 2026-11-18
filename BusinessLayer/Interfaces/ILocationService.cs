using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ILocationService
{
    Task<Location> CreateAsync(
        string name,
        string locationType,
        int capacity,
        string description);
    Task<Location?> GetByIdAsync(int locationId);
    Task<IReadOnlyList<Location>> GetAllAsync();
    Task UpdateAsync(
        int locationId,
        string name,
        string locationType,
        int capacity,
        string description);
    Task DeleteAsync(int locationId);
}
