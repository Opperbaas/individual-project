using BusinessLayer.DTOs;
using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class PlanningWorkflowService : IPlanningWorkflowService
{
    private readonly IPlanningService _planningService;
    private readonly ManualPlanner _manualPlanner;
    private readonly IPlanningGenerator _optimizedPlanner;
    private readonly IPlanningGenerator _openAiPlanningGenerator;
    private readonly ISportEventService _sportEventService;

    public PlanningWorkflowService(
        IPlanningService planningService,
        ManualPlanner manualPlanner,
        OptimizedPlanner optimizedPlanner,
        OpenAiPlanningGenerator openAiPlanningGenerator,
        ISportEventService sportEventService)
    {
        _planningService = planningService;
        _manualPlanner = manualPlanner;
        _optimizedPlanner = optimizedPlanner;
        _openAiPlanningGenerator = openAiPlanningGenerator;
        _sportEventService = sportEventService;
    }

    public Task<Planning> CreateAsync(CreatePlanningRequest request)
    {
        return _manualPlanner.CreateAsync(request);
    }

    public async Task<Planning> GenerateOptimizedAsync(
        int eventId,
        int createdByUserId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var planning = await CreateGeneratedPlanningAsync(
            eventId,
            createdByUserId,
            name);

        planning = await _optimizedPlanner.GenerateAsync(
            planning,
            cancellationToken);

        return await _planningService.CreateAsync(planning);
    }

    public async Task<Planning> GenerateWithAiAsync(
        int eventId,
        int createdByUserId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var planning = await CreateGeneratedPlanningAsync(
            eventId,
            createdByUserId,
            name);

        planning = await _openAiPlanningGenerator.GenerateAsync(
            planning,
            cancellationToken);

        return await _planningService.CreateAsync(planning);
    }

    private async Task<Planning> CreateGeneratedPlanningAsync(
        int eventId,
        int createdByUserId,
        string name)
    {
        var sportEvent = await _sportEventService.GetByIdAsync(eventId);
        if (sportEvent is null)
        {
            throw new InvalidOperationException(
                "Het evenement waarvoor een planning wordt gegenereerd bestaat niet.");
        }

        return new Planning
        {
            EventId = eventId,
            CreatedByUserId = createdByUserId,
            Name = name,
            Event = sportEvent
        };
    }
}
