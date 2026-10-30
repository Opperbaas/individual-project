using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IActivityRepository
{
    Task<Activity> CreateAsync(Activity activity);
}
