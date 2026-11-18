namespace BusinessLayer.Services;

public sealed class OpenAiSettings
{
    public string ApiKey { get; init; } = "";
    public string Model { get; init; } = "gpt-4o-mini";
}
