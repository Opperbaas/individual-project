using BusinessLayer.Entities;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class TimeSlotService : ITimeSlotService
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

    public async Task<IReadOnlyList<TimeSlot>> GenerateAsync(
        GenerateTimeSlotsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.EventId <= 0)
        {
            throw new ArgumentException(
                "Het sportevenement is ongeldig.",
                nameof(request.EventId));
        }

        if (request.StartDate > request.EndDate)
        {
            throw new ArgumentException(
                "De begindatum mag niet na de einddatum liggen.");
        }

        if (request.DailyStartTime >= request.DailyEndTime)
        {
            throw new ArgumentException(
                "De begintijd moet vóór de eindtijd liggen.");
        }

        if (request.IntervalMinutes <= 0)
        {
            throw new ArgumentException(
                "Het interval moet groter zijn dan nul.");
        }

        var existingSlots =
            await _timeSlotRepository.GetByEventIdAsync(request.EventId);
        var existingKeys = existingSlots
            .Select(CreateKey)
            .ToHashSet();
        var generatedSlots = new List<TimeSlot>();
        var selectedDays = request.Days.ToHashSet();

        for (var date = request.StartDate;
             date <= request.EndDate;
             date = date.AddDays(1))
        {
            if (selectedDays.Count > 0 &&
                !selectedDays.Contains(date.DayOfWeek))
            {
                continue;
            }

            var startTime = request.DailyStartTime;
            while (startTime.AddMinutes(request.IntervalMinutes)
                   <= request.DailyEndTime)
            {
                var endTime = startTime.AddMinutes(request.IntervalMinutes);
                var key = (date, startTime, endTime);

                if (!existingKeys.Contains(key))
                {
                    generatedSlots.Add(new TimeSlot
                    {
                        EventId = request.EventId,
                        SlotDate = date,
                        StartTime = startTime,
                        EndTime = endTime
                    });
                    existingKeys.Add(key);
                }

                startTime = endTime;
            }
        }

        if (generatedSlots.Count == 0)
        {
            return generatedSlots;
        }

        return await _timeSlotRepository.CreateManyAsync(generatedSlots);
    }

    public Task<TimeSlot?> GetByIdAsync(int timeSlotId)
    {
        if (timeSlotId <= 0)
        {
            throw new ArgumentException("Het tijdslot is ongeldig.");
        }

        return _timeSlotRepository.GetByIdAsync(timeSlotId);
    }

    public Task UpdateAsync(
        int timeSlotId,
        int eventId,
        DateOnly slotDate,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (timeSlotId <= 0)
        {
            throw new ArgumentException("Het tijdslot is ongeldig.");
        }

        if (eventId <= 0)
        {
            throw new ArgumentException("Het sportevenement is ongeldig.");
        }

        if (startTime >= endTime)
        {
            throw new ArgumentException(
                "De begintijd moet vóór de eindtijd liggen.");
        }

        return _timeSlotRepository.UpdateAsync(new TimeSlot
        {
            TimeSlotId = timeSlotId,
            EventId = eventId,
            SlotDate = slotDate,
            StartTime = startTime,
            EndTime = endTime
        });
    }

    public Task DeleteAsync(int timeSlotId)
    {
        if (timeSlotId <= 0)
        {
            throw new ArgumentException("Het tijdslot is ongeldig.");
        }

        return _timeSlotRepository.DeleteAsync(timeSlotId);
    }

    private static (
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime) CreateKey(TimeSlot timeSlot)
    {
        return (timeSlot.SlotDate, timeSlot.StartTime, timeSlot.EndTime);
    }
}
