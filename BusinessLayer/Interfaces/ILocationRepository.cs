using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ILocationRepository
{
    Task<Location> CreateAsync(Location location);
}
