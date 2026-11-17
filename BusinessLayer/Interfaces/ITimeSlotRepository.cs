using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ITimeSlotRepository
{
    Task<IReadOnlyList<TimeSlot>> GetByEventIdAsync(int eventId);
    Task<TimeSlot> CreateAsync(TimeSlot timeSlot);
    Task<IReadOnlyList<TimeSlot>> CreateManyAsync(
        IEnumerable<TimeSlot> timeSlots);
}
