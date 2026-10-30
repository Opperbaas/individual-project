using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class TimeSlotService
{
    private readonly ITimeSlotRepository _timeSlotRepository;

    public TimeSlotService(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    public Task<TimeSlot> CreateAsync(
        int eventId,
        DateOnly slotDate,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (eventId <= 0)
        {
            throw new ArgumentException("Het sportevenement is ongeldig.", nameof(eventId));
        }

        if (startTime >= endTime)
        {
            throw new ArgumentException(
                "De begintijd moet vóór de eindtijd liggen.");
        }

        return _timeSlotRepository.CreateAsync(new TimeSlot
        {
            EventId = eventId,
            SlotDate = slotDate,
            StartTime = startTime,
            EndTime = endTime
        });
    }
}
