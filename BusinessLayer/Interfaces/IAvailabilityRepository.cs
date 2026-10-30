using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IAvailabilityRepository
{
    Task<Availability> CreateAsync(Availability availability);
}
