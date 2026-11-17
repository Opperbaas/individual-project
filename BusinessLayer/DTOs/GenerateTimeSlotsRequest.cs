namespace BusinessLayer.DTOs;

public sealed class GenerateTimeSlotsRequest
{
    public int EventId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public TimeOnly DailyStartTime { get; init; }
    public TimeOnly DailyEndTime { get; init; }
    public int IntervalMinutes { get; init; }
    public IReadOnlyCollection<DayOfWeek> Days { get; init; } = [];
}
