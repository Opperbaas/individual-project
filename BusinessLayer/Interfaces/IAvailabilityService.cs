using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IAvailabilityService
{
    Task<Availability> CreateAsync(
        int userId,
        DateOnly availableDate,
        TimeOnly startTime,
        TimeOnly endTime);
}
