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

    public Task<ActivitySchedule?> GetActivityScheduleForRegistrationAsync(int activityScheduleId)
    {
        return _dbContext.ActivitySchedules
            .Include(schedule => schedule.Activity)
            .Include(schedule => schedule.Registrations)
            .SingleOrDefaultAsync(schedule => schedule.ActivityScheduleId == activityScheduleId);
    }

    public Task<bool> ExistsAsync(int userId, int activityScheduleId)
    {
        return _dbContext.Registrations.AnyAsync(registration =>
            registration.UserId == userId &&
            registration.ActivityScheduleId == activityScheduleId);
    }

    public async Task<Registration> CreateAsync(Registration registration)
    {
        ArgumentNullException.ThrowIfNull(registration);

        _dbContext.Registrations.Add(registration);
        await _dbContext.SaveChangesAsync();

        return registration;
    }
}
