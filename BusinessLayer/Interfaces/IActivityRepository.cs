using BusinessLayer.Entities;

namespace BusinessLayer.Interfaces;

public interface IActivityRepository
{
    Task<Activity> CreateAsync(Activity activity);
    Task<Activity?> GetByIdAsync(int activityId);
    Task UpdateAsync(Activity activity);
    Task DeleteAsync(int activityId);
}
