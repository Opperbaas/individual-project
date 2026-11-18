using BusinessLayer.Entities;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces;

public interface ITimeSlotService
{
    Task<TimeSlot> CreateAsync(
        int eventId,
        DateOnly slotDate,
        TimeOnly startTime,
        TimeOnly endTime);

    Task<IReadOnlyList<TimeSlot>> GenerateAsync(
        GenerateTimeSlotsRequest request);
    Task<TimeSlot?> GetByIdAsync(int timeSlotId);
    Task UpdateAsync(
        int timeSlotId,
        int eventId,
        DateOnly slotDate,
        TimeOnly startTime,
        TimeOnly endTime);
    Task DeleteAsync(int timeSlotId);
}
