using BusinessLayer.Entities;
using PresentationLayer.Models;

namespace PresentationLayer.Interfaces;

public interface IAvailabilityCalendarService
{
    AvailabilityCalendarViewModel CreateModel(
        DateOnly date,
        IEnumerable<Availability> availability);

    IReadOnlyList<Availability> ParseAvailability(
        IEnumerable<string> selectedSlots);
}
