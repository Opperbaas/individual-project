using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IPlanningGenerator
{
    Task<Planning> GenerateAsync(
        Planning planning,
        CancellationToken cancellationToken = default);
}
