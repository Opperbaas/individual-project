using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class AvailabilityService
{
    private readonly IAvailabilityRepository _availabilityRepository;

    public AvailabilityService(IAvailabilityRepository availabilityRepository)
    {
        _availabilityRepository = availabilityRepository;
    }

    public Task<Availability> CreateAsync(
        int userId,
        DateOnly availableDate,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("De ingelogde gebruiker is ongeldig.", nameof(userId));
        }

        if (startTime >= endTime)
        {
            throw new ArgumentException(
                "De begintijd moet vóór de eindtijd liggen.");
        }

        return _availabilityRepository.CreateAsync(new Availability
        {
            UserId = userId,
            AvailableDate = availableDate,
            StartTime = startTime,
            EndTime = endTime
        });
    }
}
