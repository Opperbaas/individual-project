using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Repositories;

public sealed class SqlSportEventRepository : ISportEventRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlSportEventRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SportEvent>> GetAllAsync()
    {
        return await _dbContext.SportEvents
            .AsNoTracking()
            .OrderByDescending(sportEvent => sportEvent.StartDate)
            .ThenBy(sportEvent => sportEvent.EventName)
            .ToListAsync();
    }

    public Task<SportEvent?> GetByIdAsync(int eventId)
    {
        return _dbContext.SportEvents
            .AsNoTracking()
            .SingleOrDefaultAsync(sportEvent => sportEvent.EventId == eventId);
    }

    public async Task<SportEvent> CreateAsync(SportEvent sportEvent)
    {
        ArgumentNullException.ThrowIfNull(sportEvent);

        _dbContext.SportEvents.Add(sportEvent);
        await _dbContext.SaveChangesAsync();

        return sportEvent;
    }
}
