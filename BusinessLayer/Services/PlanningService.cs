using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class PlanningService
{
    private readonly IPlanningRepository _planningRepository;
    private readonly ScheduleValidationService _scheduleValidationService;

    public PlanningService(
        IPlanningRepository planningRepository,
        ScheduleValidationService scheduleValidationService)
    {
        _planningRepository = planningRepository;
        _scheduleValidationService = scheduleValidationService;
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

    public Task<IReadOnlyList<Activity>> GetActivitiesByEventIdAsync(int eventId)
    {
        if (eventId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(eventId));
        }

        return _planningRepository.GetActivitiesByEventIdAsync(eventId);
    }

    public Task<IReadOnlyList<Location>> GetLocationsAsync()
    {
        return _planningRepository.GetLocationsAsync();
    }

    public Task<IReadOnlyList<TimeSlot>> GetTimeSlotsByEventIdAsync(int eventId)
    {
        if (eventId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(eventId));
        }

        return _planningRepository.GetTimeSlotsByEventIdAsync(eventId);
    }

    public Task<IReadOnlyList<User>> GetVolunteersAsync()
    {
        return _planningRepository.GetVolunteersAsync();
    }

    public async Task<Planning> CreateAsync(Planning planning)
    {
        ArgumentNullException.ThrowIfNull(planning);
        ValidatePlanning(planning);
        var existingPlannings = await GetPlanningsForEventAsync(planning.EventId);
        EnsureValidSchedule(planning, existingPlannings);

        planning.CreatedAt = DateTime.UtcNow;
        if (string.IsNullOrWhiteSpace(planning.Status))
        {
            planning.Status = "Draft";
        }

        return await _planningRepository.CreateAsync(planning);
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

        var existingPlannings = await GetPlanningsForEventAsync(planning.EventId);
        EnsureValidSchedule(planning, existingPlannings);
        await _planningRepository.UpdateAsync(planning);
    }

    public async Task<IReadOnlyList<string>> ValidateAsync(int planningId)
    {
        var planning = await GetExistingPlanningAsync(planningId);
        var existingPlannings = await GetPlanningsForEventAsync(planning.EventId);
        return _scheduleValidationService.Validate(planning, existingPlannings);
    }

    public async Task FinalizeAsync(int planningId)
    {
        var planning = await GetExistingPlanningAsync(planningId);
        var existingPlannings = await GetPlanningsForEventAsync(planning.EventId);
        var validationErrors = _scheduleValidationService.Validate(
            planning,
            existingPlannings);

        if (validationErrors.Count > 0)
        {
            throw new InvalidOperationException(
                $"De planning kan niet definitief worden gemaakt: {string.Join(" ", validationErrors)}");
        }

        planning.Status = "Final";
        await _planningRepository.UpdateAsync(planning);
    }

    private async Task<Planning> GetExistingPlanningAsync(int planningId)
    {
        if (planningId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(planningId));
        }

        var planning = await _planningRepository.GetByIdAsync(planningId);
        if (planning is null)
        {
            throw new KeyNotFoundException(
                $"Planning met ID {planningId} bestaat niet.");
        }

        return planning;
    }

    private void EnsureValidSchedule(
        Planning planning,
        IEnumerable<Planning> existingPlannings)
    {
        var validationErrors = _scheduleValidationService.Validate(
            planning,
            existingPlannings);
        if (validationErrors.Count > 0)
        {
            throw new ArgumentException(
                $"De planning bevat conflicten: {string.Join(" ", validationErrors)}",
                nameof(planning));
        }
    }

    private Task<IReadOnlyList<Planning>> GetPlanningsForEventAsync(int eventId)
    {
        return _planningRepository.GetByEventIdAsync(eventId);
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