using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;
using Microsoft.EntityFrameworkCore;

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

    public Task<Activity?> GetByIdAsync(int activityId)
    {
        return _dbContext.Activities
            .AsNoTracking()
            .SingleOrDefaultAsync(activity => activity.ActivityId == activityId);
    }

    public async Task UpdateAsync(Activity activity)
    {
        var existing = await _dbContext.Activities
            .SingleOrDefaultAsync(item => item.ActivityId == activity.ActivityId);
        if (existing is null)
        {
            throw new KeyNotFoundException("De activiteit bestaat niet.");
        }

        existing.ActivityName = activity.ActivityName;
        existing.ActivityType = activity.ActivityType;
        existing.Description = activity.Description;
        existing.MaxParticipants = activity.MaxParticipants;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int activityId)
    {
        var existing = await _dbContext.Activities
            .SingleOrDefaultAsync(item => item.ActivityId == activityId);
        if (existing is null)
        {
            throw new KeyNotFoundException("De activiteit bestaat niet.");
        }

        if (await _dbContext.ActivitySchedules
                .AnyAsync(item => item.ActivityId == activityId))
        {
            throw new InvalidOperationException(
                "De activiteit kan niet worden verwijderd omdat deze in een planning wordt gebruikt.");
        }

        _dbContext.Activities.Remove(existing);
        await _dbContext.SaveChangesAsync();
    }
}
