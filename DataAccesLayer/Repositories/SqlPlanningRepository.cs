using BusinessLayer.Entities;
using BusinessLayer.Enums;
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

    public async Task<IReadOnlyList<Activity>> GetActivitiesByEventIdAsync(int eventId)
    {
        return await _dbContext.Activities
            .AsNoTracking()
            .Where(activity => activity.EventId == eventId)
            .OrderBy(activity => activity.ActivityName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Location>> GetLocationsAsync()
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .OrderBy(location => location.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TimeSlot>> GetTimeSlotsByEventIdAsync(int eventId)
    {
        return await _dbContext.TimeSlots
            .AsNoTracking()
            .Where(timeSlot => timeSlot.EventId == eventId)
            .OrderBy(timeSlot => timeSlot.SlotDate)
            .ThenBy(timeSlot => timeSlot.StartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<User>> GetVolunteersAsync()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(user => user.Role.Name == RoleNames.Volunteer)
            .OrderBy(user => user.Name)
            .ToListAsync();
    }

    public async Task<Planning> CreateAsync(Planning planning)
    {
        ArgumentNullException.ThrowIfNull(planning);

        if (planning.Event is not null)
        {
            _dbContext.Entry(planning.Event).State = EntityState.Unchanged;
        }

        if (planning.CreatedByUser is not null)
        {
            _dbContext.Entry(planning.CreatedByUser).State = EntityState.Unchanged;
        }

        foreach (var schedule in planning.ActivitySchedules)
        {
            if (schedule.Activity is not null)
            {
                _dbContext.Entry(schedule.Activity).State = EntityState.Unchanged;
            }

            if (schedule.Location is not null)
            {
                _dbContext.Entry(schedule.Location).State = EntityState.Unchanged;
            }

            if (schedule.TimeSlot is not null)
            {
                _dbContext.Entry(schedule.TimeSlot).State = EntityState.Unchanged;
            }
        }

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

    public async Task DeleteAsync(int planningId)
    {
        var planning = await _dbContext.Plannings
            .Include(item => item.ActivitySchedules)
                .ThenInclude(schedule => schedule.VolunteerAssignments)
            .SingleOrDefaultAsync(item => item.PlanningId == planningId);

        if (planning is null)
        {
            throw new KeyNotFoundException(
                $"Planning met ID {planningId} bestaat niet.");
        }

        var assignments = planning.ActivitySchedules
            .SelectMany(schedule => schedule.VolunteerAssignments)
            .ToList();

        _dbContext.VolunteerAssignments.RemoveRange(assignments);
        _dbContext.ActivitySchedules.RemoveRange(planning.ActivitySchedules);
        _dbContext.Plannings.Remove(planning);
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
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.Registrations)
                    .ThenInclude(registration => registration.User)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.VolunteerAssignments)
                        .ThenInclude(assignment => assignment.User)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.Location)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.TimeSlot);
    }
}