using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class PlanningService
{
    private readonly IPlanningRepository _planningRepository;

    public PlanningService(IPlanningRepository planningRepository)
    {
        _planningRepository = planningRepository;
    }

    public Task<Planning?> GetByIdAsync(int planningId)
    {
        if (planningId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(planningId));
        }

        return _planningRepository.GetByIdAsync(planningId);
    }

    public Task<IReadOnlyList<Planning>> GetByEventIdAsync(int eventId)
    {
        if (eventId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(eventId));
        }

        return _planningRepository.GetByEventIdAsync(eventId);
    }

    public Task<Planning> CreateAsync(Planning planning)
    {
        ArgumentNullException.ThrowIfNull(planning);
        ValidatePlanning(planning);

        planning.CreatedAt = DateTime.UtcNow;
        if (string.IsNullOrWhiteSpace(planning.Status))
        {
            planning.Status = "Draft";
        }

        return _planningRepository.CreateAsync(planning);
    }

    public async Task UpdateAsync(Planning planning)
    {
        ArgumentNullException.ThrowIfNull(planning);
        ValidatePlanning(planning);

        var existingPlanning = await _planningRepository.GetByIdAsync(planning.PlanningId);
        if (existingPlanning is null)
        {
            throw new KeyNotFoundException(
                $"Planning met ID {planning.PlanningId} bestaat niet.");
        }

        await _planningRepository.UpdateAsync(planning);
    }

    private static void ValidatePlanning(Planning planning)
    {
        if (planning.PlanningId < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(planning.PlanningId),
                "PlanningId mag niet negatief zijn.");
        }

        if (planning.EventId <= 0)
        {
            throw new ArgumentException(
                "Een planning moet aan een evenement gekoppeld zijn.",
                nameof(planning.EventId));
        }

        if (planning.CreatedByUserId <= 0)
        {
            throw new ArgumentException(
                "Een planning moet een maker hebben.",
                nameof(planning.CreatedByUserId));
        }

        if (string.IsNullOrWhiteSpace(planning.Name))
        {
            throw new ArgumentException(
                "Een planning moet een naam hebben.",
                nameof(planning.Name));
        }
    }
}