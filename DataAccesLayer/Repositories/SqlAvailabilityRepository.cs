using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;

namespace DataAccesLayer.Repositories;

public sealed class SqlAvailabilityRepository : IAvailabilityRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlAvailabilityRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Availability> CreateAsync(Availability availability)
    {
        ArgumentNullException.ThrowIfNull(availability);

        _dbContext.Availabilities.Add(availability);
        await _dbContext.SaveChangesAsync();

        return availability;
    }
}
