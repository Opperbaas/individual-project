namespace PresentationLayer.Models;

public sealed class SportEventViewModel
{
    public int EventId { get; set; }
    public string EventName { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
