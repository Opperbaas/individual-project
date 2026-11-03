using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Repositories;

public sealed class SqlRegistrationRepository : IRegistrationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlRegistrationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Activity?> GetActivityForRegistrationAsync(int activityId)
    {
        return _dbContext.Activities
            .Include(activity => activity.Registrations)
            .SingleOrDefaultAsync(activity => activity.ActivityId == activityId);
    }

    public Task<bool> ExistsAsync(int userId, int activityId)
    {
        return _dbContext.Registrations.AnyAsync(registration =>
            registration.UserId == userId &&
            registration.ActivityId == activityId);
    }

    public async Task<Registration> CreateAsync(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);

        _dbContext.Registrations.Add(registration);
        await _dbContext.SaveChangesAsync();

        return registration;
    }
}
