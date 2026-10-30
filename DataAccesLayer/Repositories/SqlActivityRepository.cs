using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;

namespace DataAccesLayer.Repositories;

public sealed class SqlActivityRepository : IActivityRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlActivityRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Activity> CreateAsync(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        _dbContext.Activities.Add(activity);
        await _dbContext.SaveChangesAsync();

        return activity;
    }
}
