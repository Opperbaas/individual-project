using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;

namespace DataAccesLayer.Repositories;

public sealed class SqlSportEventRepository : ISportEventRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlSportEventRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SportEvent> CreateAsync(SportEvent sportEvent)
    {
        ArgumentNullException.ThrowIfNull(sportEvent);

        _dbContext.SportEvents.Add(sportEvent);
        await _dbContext.SaveChangesAsync();

        return sportEvent;
    }
}
