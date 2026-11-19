namespace BusinessLayer.Services;

public interface IActivityTemplateService
{
    bool IsCompatible(string executionType, string locationType);
    string GetActivityName(string executionType, string locationType);
}
