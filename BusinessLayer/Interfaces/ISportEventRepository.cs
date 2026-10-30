using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface ISportEventRepository
{
    Task<SportEvent> CreateAsync(SportEvent sportEvent);
}
