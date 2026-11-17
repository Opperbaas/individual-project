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
}
