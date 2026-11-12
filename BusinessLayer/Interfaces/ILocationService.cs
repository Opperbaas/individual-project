using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ILocationService
{
    Task<Location> CreateAsync(
        string name,
        string locationType,
        int capacity,
        string description);
}
