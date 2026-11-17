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
}
