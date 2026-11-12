using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ITimeSlotService
{
    Task<TimeSlot> CreateAsync(
        int eventId,
        DateOnly slotDate,
        TimeOnly startTime,
        TimeOnly endTime);
}
