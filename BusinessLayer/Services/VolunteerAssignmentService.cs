using BusinessLayer.DTOs;
using BusinessLayer.Entities;
using BusinessLayer.Interfaces;

namespace BusinessLayer.Services;

public sealed class VolunteerAssignmentService : IVolunteerAssignmentService
{
    private readonly IVolunteerAssignmentRepository _repository;

    public VolunteerAssignmentService(
        IVolunteerAssignmentRepository repository)
    {
        _repository = repository;
    }

    public async Task AssignAsync(
        IEnumerable<VolunteerAssignmentRequest> requests)
    {
        var assignments = await PrepareAssignmentsAsync(requests);
        await _repository.CreateManyAsync(assignments);
    }

    public async Task ValidateAsync(
        IEnumerable<VolunteerAssignmentRequest> requests)
    {
        await PrepareAssignmentsAsync(requests);
    }

    private async Task<List<VolunteerAssignment>> PrepareAssignmentsAsync(
        IEnumerable<VolunteerAssignmentRequest> requests)
    {
        var requestList = requests.ToList();
        var volunteerIds = requestList
            .SelectMany(request => request.VolunteerIds)
            .ToArray();

        var volunteers = await _repository.GetVolunteersByIdsAsync(volunteerIds);
        var validVolunteerIds = volunteers
            .Select(volunteer => volunteer.UserId)
            .ToHashSet();

        if (validVolunteerIds.Count != volunteerIds.Distinct().Count())
        {
            throw new InvalidOperationException(
                "Een of meer geselecteerde gebruikers zijn geen vrijwilliger.");
        }

        var assignments = new List<VolunteerAssignment>();
        var assignedTimes = new Dictionary<int, List<TimeSlot>>();

        foreach (var request in requestList)
        {
            if (request.ActivityScheduleId <= 0)
            {
                throw new InvalidOperationException(
                    "Het activiteitsschema bestaat niet.");
            }

            var timeSlot = await _repository.GetTimeSlotAsync(request.TimeSlotId);
            if (timeSlot is null)
            {
                throw new InvalidOperationException("Het gekozen tijdslot bestaat niet.");
            }

            foreach (var volunteerId in request.VolunteerIds.Distinct())
            {
                if (!await _repository.HasAvailabilityAsync(
                        volunteerId,
                        timeSlot.SlotDate,
                        timeSlot.StartTime,
                        timeSlot.EndTime))
                {
                    throw new InvalidOperationException(
                        $"Vrijwilliger {volunteerId} is niet beschikbaar tijdens dit tijdslot.");
                }

                var previousSlots = assignedTimes.GetValueOrDefault(volunteerId, []);
                if (previousSlots.Any(previous => Overlaps(previous, timeSlot)))
                {
                    throw new InvalidOperationException(
                        $"Vrijwilliger {volunteerId} is dubbel ingepland.");
                }

                previousSlots.Add(timeSlot);
                assignedTimes[volunteerId] = previousSlots;
                assignments.Add(new VolunteerAssignment
                {
                    UserId = volunteerId,
                    ActivityScheduleId = request.ActivityScheduleId
                });
            }
        }

        return assignments;
    }

    private static bool Overlaps(TimeSlot first, TimeSlot second)
    {
        return first.SlotDate == second.SlotDate &&
               first.StartTime < second.EndTime &&
               second.StartTime < first.EndTime;
    }
}
