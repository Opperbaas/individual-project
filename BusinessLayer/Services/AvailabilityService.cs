using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class AvailabilityService : IAvailabilityService
{
    private readonly IAvailabilityRepository _availabilityRepository;

    public AvailabilityService(IAvailabilityRepository availabilityRepository)
    {
        _availabilityRepository = availabilityRepository;
    }

    public Task<IReadOnlyList<Availability>> GetByUserIdAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }

        return _availabilityRepository.GetByUserIdAsync(userId);
    }

    public async Task ReplaceForUserAsync(
        int userId,
        IEnumerable<Availability> availability)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }

        ArgumentNullException.ThrowIfNull(availability);
        var entries = availability.ToList();

        foreach (var entry in entries)
        {
            ArgumentNullException.ThrowIfNull(entry);
            if (entry.StartTime >= entry.EndTime)
            {
                throw new ArgumentException(
                    "De begintijd moet vóór de eindtijd liggen.");
            }

            entry.UserId = userId;
        }

        await _availabilityRepository.ReplaceForUserAsync(userId, entries);
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
