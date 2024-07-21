using SMS.App.Models.Entities;

namespace SMS.App.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(Notification notification);
    }
}