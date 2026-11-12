using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Repositories;

public sealed class SqlAvailabilityRepository : IAvailabilityRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlAvailabilityRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Availability>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Availabilities
            .AsNoTracking()
            .Where(availability => availability.UserId == userId)
            .OrderBy(availability => availability.AvailableDate)
            .ThenBy(availability => availability.StartTime)
            .ToListAsync();
    }

    public async Task ReplaceForUserAsync(
        int userId,
        IEnumerable<Availability> availability)
    {
        var existing = await _dbContext.Availabilities
            .Where(item => item.UserId == userId)
            .ToListAsync();

        _dbContext.Availabilities.RemoveRange(existing);
        _dbContext.Availabilities.AddRange(availability);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Availability> CreateAsync(Availability availability)
    {
        ArgumentNullException.ThrowIfNull(availability);

        _dbContext.Availabilities.Add(availability);
        await _dbContext.SaveChangesAsync();

        return availability;
    }
}
