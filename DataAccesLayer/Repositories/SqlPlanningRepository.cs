using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Repositories;

public sealed class SqlPlanningRepository : IPlanningRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlPlanningRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Planning?> GetByIdAsync(int planningId)
    {
        return CreatePlanningQuery()
            .SingleOrDefaultAsync(planning => planning.PlanningId == planningId);
    }

    public async Task<IReadOnlyList<Planning>> GetByEventIdAsync(int eventId)
    {
        return await CreatePlanningQuery()
            .Where(planning => planning.EventId == eventId)
            .OrderByDescending(planning => planning.CreatedAt)
            .ToListAsync();
    }

    public async Task<Planning> CreateAsync(Planning planning)
    {
        ArgumentNullException.ThrowIfNull(planning);

        _dbContext.Plannings.Add(planning);
        await _dbContext.SaveChangesAsync();

        return planning;
    }

    public async Task UpdateAsync(Planning planning)
    {
        ArgumentNullException.ThrowIfNull(planning);

        _dbContext.Plannings.Update(planning);
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<Planning> CreatePlanningQuery()
    {
        return _dbContext.Plannings
            .AsNoTracking()
            .AsSplitQuery()
            .Include(planning => planning.Event)
            .Include(planning => planning.CreatedByUser)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.Activity)
                    .ThenInclude(activity => activity.Registrations)
                        .ThenInclude(registration => registration.User)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.Activity)
                    .ThenInclude(activity => activity.VolunteerAssignments)
                        .ThenInclude(assignment => assignment.User)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.Location)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.TimeSlot);
    }
}