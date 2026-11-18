using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Repositories;

public sealed class SqlLocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlLocationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Location> CreateAsync(Location location)
    {
        ArgumentNullException.ThrowIfNull(location);

        _dbContext.Locations.Add(location);
        await _dbContext.SaveChangesAsync();

        return location;
    }

    public Task<Location?> GetByIdAsync(int locationId)
    {
        return _dbContext.Locations
            .AsNoTracking()
            .SingleOrDefaultAsync(location => location.LocationId == locationId);
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync()
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .OrderBy(location => location.Name)
            .ToListAsync();
    }

    public async Task UpdateAsync(Location location)
    {
        var existing = await _dbContext.Locations
            .SingleOrDefaultAsync(item => item.LocationId == location.LocationId);
        if (existing is null)
        {
            throw new KeyNotFoundException("De locatie bestaat niet.");
        }

        existing.Name = location.Name;
        existing.LocationType = location.LocationType;
        existing.Capacity = location.Capacity;
        existing.Description = location.Description;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int locationId)
    {
        var existing = await _dbContext.Locations
            .SingleOrDefaultAsync(item => item.LocationId == locationId);
        if (existing is null)
        {
            throw new KeyNotFoundException("De locatie bestaat niet.");
        }

        if (await _dbContext.ActivitySchedules
                .AnyAsync(item => item.LocationId == locationId))
        {
            throw new InvalidOperationException(
                "De locatie kan niet worden verwijderd omdat deze in een planning wordt gebruikt.");
        }

        _dbContext.Locations.Remove(existing);
        await _dbContext.SaveChangesAsync();
    }
}
