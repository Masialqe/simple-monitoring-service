using SMS.App.Models.Results;

namespace SMS.App.Handlers.Interfaces
{
    public interface INotificationHandler
    {
        Task NotifyFailedCheckAsync(IEnumerable<CheckResult> unavailableServices);
        Task NotifyStoppingService();
    }
}