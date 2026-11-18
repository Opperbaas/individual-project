using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Repositories;

public sealed class SqlTimeSlotRepository : ITimeSlotRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlTimeSlotRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TimeSlot>> GetByEventIdAsync(int eventId)
    {
        return await _dbContext.TimeSlots
            .Where(timeSlot => timeSlot.EventId == eventId)
            .ToListAsync();
    }

    public async Task<TimeSlot> CreateAsync(TimeSlot timeSlot)
    {
        ArgumentNullException.ThrowIfNull(timeSlot);

        _dbContext.TimeSlots.Add(timeSlot);
        await _dbContext.SaveChangesAsync();

        return timeSlot;
    }

    public async Task<IReadOnlyList<TimeSlot>> CreateManyAsync(
        IEnumerable<TimeSlot> timeSlots)
    {
        var slots = timeSlots.ToList();

        _dbContext.TimeSlots.AddRange(slots);
        await _dbContext.SaveChangesAsync();

        return slots;
    }

    public Task<TimeSlot?> GetByIdAsync(int timeSlotId)
    {
        return _dbContext.TimeSlots
            .AsNoTracking()
            .SingleOrDefaultAsync(timeSlot => timeSlot.TimeSlotId == timeSlotId);
    }

    public async Task UpdateAsync(TimeSlot timeSlot)
    {
        var existing = await _dbContext.TimeSlots
            .SingleOrDefaultAsync(item => item.TimeSlotId == timeSlot.TimeSlotId);
        if (existing is null)
        {
            throw new KeyNotFoundException("Het tijdslot bestaat niet.");
        }

        existing.SlotDate = timeSlot.SlotDate;
        existing.StartTime = timeSlot.StartTime;
        existing.EndTime = timeSlot.EndTime;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int timeSlotId)
    {
        var existing = await _dbContext.TimeSlots
            .SingleOrDefaultAsync(item => item.TimeSlotId == timeSlotId);
        if (existing is null)
        {
            throw new KeyNotFoundException("Het tijdslot bestaat niet.");
        }

        if (await _dbContext.ActivitySchedules
                .AnyAsync(item => item.TimeSlotId == timeSlotId))
        {
            throw new InvalidOperationException(
                "Het tijdslot kan niet worden verwijderd omdat dit in een planning wordt gebruikt.");
        }

        _dbContext.TimeSlots.Remove(existing);
        await _dbContext.SaveChangesAsync();
    }
}
