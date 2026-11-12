using BusinessLayer.Entities;
using BusinessLayer.Enums;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Repositories;

public sealed class SqlVolunteerAssignmentRepository
    : IVolunteerAssignmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlVolunteerAssignmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Planning>> GetPlanningsByVolunteerIdAsync(
        int userId)
    {
        var plannings = await _dbContext.Plannings
            .AsNoTracking()
            .AsSplitQuery()
            .Where(planning => planning.ActivitySchedules
                .Any(schedule => schedule.VolunteerAssignments
                    .Any(assignment => assignment.UserId == userId)))
            .Include(planning => planning.Event)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.Activity)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.Location)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.TimeSlot)
            .Include(planning => planning.ActivitySchedules)
                .ThenInclude(schedule => schedule.VolunteerAssignments)
            .OrderBy(planning => planning.Event.StartDate)
            .ThenBy(planning => planning.Name)
            .ToListAsync();

        foreach (var planning in plannings)
        {
            planning.ActivitySchedules = planning.ActivitySchedules
                .Where(schedule => schedule.VolunteerAssignments
                    .Any(assignment => assignment.UserId == userId))
                .ToList();
        }

        return plannings;
    }

    public async Task<IReadOnlyList<User>> GetVolunteersByIdsAsync(
        IEnumerable<int> userIds)
    {
        var ids = userIds.Distinct().ToArray();

        return await _dbContext.Users
            .AsNoTracking()
            .Where(user => ids.Contains(user.UserId) &&
                           user.Role.Name == RoleNames.Volunteer)
            .ToListAsync();
    }

    public Task<TimeSlot?> GetTimeSlotAsync(int timeSlotId)
    {
        return _dbContext.TimeSlots
            .AsNoTracking()
            .SingleOrDefaultAsync(timeSlot => timeSlot.TimeSlotId == timeSlotId);
    }

    public Task<bool> HasAvailabilityAsync(
        int userId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        return _dbContext.Availabilities.AnyAsync(availability =>
            availability.UserId == userId &&
            availability.AvailableDate == date &&
            availability.StartTime <= startTime &&
            availability.EndTime >= endTime);
    }

    public async Task CreateManyAsync(
        IEnumerable<VolunteerAssignment> assignments)
    {
        var assignmentList = assignments.ToList();
        if (assignmentList.Count == 0)
        {
            return;
        }

        _dbContext.VolunteerAssignments.AddRange(assignmentList);
        await _dbContext.SaveChangesAsync();
    }
}
