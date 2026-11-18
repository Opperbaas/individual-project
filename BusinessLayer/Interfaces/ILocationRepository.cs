using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ILocationRepository
{
    Task<Location> CreateAsync(Location location);
    Task<Location?> GetByIdAsync(int locationId);
    Task<IReadOnlyList<Location>> GetAllAsync();
    Task UpdateAsync(Location location);
    Task DeleteAsync(int locationId);
}
