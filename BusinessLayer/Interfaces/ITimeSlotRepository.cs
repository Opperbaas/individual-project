using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ITimeSlotRepository
{
    Task<TimeSlot> CreateAsync(TimeSlot timeSlot);
}
