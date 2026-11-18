using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class LocationService : ILocationService
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

    public Task<Location?> GetByIdAsync(int locationId)
    {
        if (locationId <= 0)
        {
            throw new ArgumentException("De locatie is ongeldig.");
        }

        return _locationRepository.GetByIdAsync(locationId);
    }

    public Task<IReadOnlyList<Location>> GetAllAsync()
    {
        return _locationRepository.GetAllAsync();
    }

    public Task UpdateAsync(
        int locationId,
        string name,
        string locationType,
        int capacity,
        string description)
    {
        if (locationId <= 0)
        {
            throw new ArgumentException("De locatie is ongeldig.");
        }

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(locationType) ||
            capacity <= 0)
        {
            throw new ArgumentException(
                "Naam, locatietype en capaciteit zijn verplicht.");
        }

        return _locationRepository.UpdateAsync(new Location
        {
            LocationId = locationId,
            Name = name.Trim(),
            LocationType = locationType.Trim(),
            Capacity = capacity,
            Description = description?.Trim() ?? ""
        });
    }

    public Task DeleteAsync(int locationId)
    {
        if (locationId <= 0)
        {
            throw new ArgumentException("De locatie is ongeldig.");
        }

        return _locationRepository.DeleteAsync(locationId);
    }
}
