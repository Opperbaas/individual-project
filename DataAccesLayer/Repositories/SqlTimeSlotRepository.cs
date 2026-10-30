using BusinessLayer.Entities;
using BusinessLayer.Interfaces;
using DataAccesLayer.Database;

namespace DataAccesLayer.Repositories;

public sealed class SqlTimeSlotRepository : ITimeSlotRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SqlTimeSlotRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TimeSlot> CreateAsync(TimeSlot timeSlot)
    {
        ArgumentNullException.ThrowIfNull(timeSlot);

        _dbContext.TimeSlots.Add(timeSlot);
        await _dbContext.SaveChangesAsync();

        return timeSlot;
    }
}
