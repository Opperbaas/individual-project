using BusinessLayer.DTOs;
using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IPlanningWorkflowService
{
    Task<Planning> CreateAsync(CreatePlanningRequest request);
    Task<Planning> GenerateWithAiAsync(
        int eventId,
        int createdByUserId,
        string name,
        CancellationToken cancellationToken = default);
}
