using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;

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
}
