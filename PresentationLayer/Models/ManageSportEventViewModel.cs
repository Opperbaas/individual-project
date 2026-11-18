using BusinessLayer.Entities;

namespace PresentationLayer.Models;

public sealed class ManageSportEventViewModel
{
    public int EventId { get; set; }
    public string EventName { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public IReadOnlyList<Activity> Activities { get; set; } = [];
    public IReadOnlyList<Location> Locations { get; set; } = [];
    public IReadOnlyList<TimeSlot> TimeSlots { get; set; } = [];

    public List<GenerateActivityRowViewModel> ActivityRows { get; set; } =
        [new GenerateActivityRowViewModel()];
}

public sealed class GenerateActivityRowViewModel
{
    public string ExecutionType { get; set; } = "Teamsporten";
    public string LocationType { get; set; } = "Sporthal";
    public int LocationId { get; set; }
    public int Count { get; set; } = 1;
}
