using BusinessLayer.DTOs;
using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class PlanningWorkflowService : IPlanningWorkflowService
{
    private readonly IPlanningService _planningService;
    private readonly IVolunteerAssignmentService _volunteerAssignmentService;
    private readonly IPlanningGenerator _planningGenerator;
    private readonly ISportEventService _sportEventService;

    public PlanningWorkflowService(
        IPlanningService planningService,
        IVolunteerAssignmentService volunteerAssignmentService,
        IPlanningGenerator planningGenerator,
        ISportEventService sportEventService)
    {
        _planningService = planningService;
        _volunteerAssignmentService = volunteerAssignmentService;
        _planningGenerator = planningGenerator;
        _sportEventService = sportEventService;
    }

    public async Task<Planning> GenerateWithAiAsync(
        int eventId,
        int createdByUserId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var sportEvent = await _sportEventService.GetByIdAsync(eventId);
        if (sportEvent is null)
        {
            throw new InvalidOperationException(
                "Het evenement waarvoor een planning wordt gegenereerd bestaat niet.");
        }

        var planning = new Planning
        {
            EventId = eventId,
            CreatedByUserId = createdByUserId,
            Name = name,
            Event = sportEvent
        };

        planning = await _planningGenerator.GenerateAsync(
            planning,
            cancellationToken);

        return await _planningService.CreateAsync(planning);
    }

    public async Task<Planning> CreateAsync(CreatePlanningRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var activities =
            await _planningService.GetActivitiesByEventIdAsync(request.EventId);
        var locations = await _planningService.GetLocationsAsync();
        var timeSlots =
            await _planningService.GetTimeSlotsByEventIdAsync(request.EventId);

        var planning = new Planning
        {
            EventId = request.EventId,
            CreatedByUserId = request.CreatedByUserId,
            Name = request.Name
        };

        foreach (var scheduleRequest in request.Schedules)
        {
            var activity = activities.SingleOrDefault(
                item => item.ActivityId == scheduleRequest.ActivityId);
            var location = locations.SingleOrDefault(
                item => item.LocationId == scheduleRequest.LocationId);
            var timeSlot = timeSlots.SingleOrDefault(
                item => item.TimeSlotId == scheduleRequest.TimeSlotId);

            if (activity is null || location is null || timeSlot is null)
            {
                throw new ArgumentException(
                    "Kies een geldige activiteit, locatie en tijdslot.");
            }

            planning.ActivitySchedules.Add(new ActivitySchedule
            {
                ActivityId = activity.ActivityId,
                LocationId = location.LocationId,
                TimeSlotId = timeSlot.TimeSlotId,
                Activity = activity,
                Location = location,
                TimeSlot = timeSlot
            });
        }

        planning = await _planningService.CreateAsync(planning);

        var assignmentRequests = planning.ActivitySchedules
            .Zip(request.Schedules, (activitySchedule, schedule) =>
                new VolunteerAssignmentRequest
                {
                    ActivityScheduleId = activitySchedule.ActivityScheduleId,
                    TimeSlotId = schedule.TimeSlotId,
                    VolunteerIds = schedule.VolunteerIds
                })
            .ToList();

        await _volunteerAssignmentService.ValidateAsync(assignmentRequests);
        await _volunteerAssignmentService.AssignAsync(assignmentRequests);

        return planning;
    }
}
