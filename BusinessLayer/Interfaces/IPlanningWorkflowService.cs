using BusinessLayer.DTOs;
using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IPlanningWorkflowService
{
    Task<Planning> CreateAsync(CreatePlanningRequest request);
}
