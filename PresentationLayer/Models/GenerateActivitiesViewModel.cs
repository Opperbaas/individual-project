namespace PresentationLayer.Models;

public sealed class GenerateActivitiesViewModel
{
    public int EventId { get; set; }
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
