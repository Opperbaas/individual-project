using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class LocationService
{
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public Task<Location> CreateAsync(
        string name,
        string locationType,
        int capacity,
        string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Een locatie moet een naam hebben.",
                nameof(name));
        }

        if (string.IsNullOrWhiteSpace(locationType))
        {
            throw new ArgumentException(
                "Een locatie moet een type hebben.",
                nameof(locationType));
        }

        if (capacity <= 0)
        {
            throw new ArgumentException(
                "De capaciteit moet groter dan nul zijn.",
                nameof(capacity));
        }

        return _locationRepository.CreateAsync(new Location
        {
            Name = name.Trim(),
            LocationType = locationType.Trim(),
            Capacity = capacity,
            Description = description?.Trim() ?? ""
        });
    }
}
