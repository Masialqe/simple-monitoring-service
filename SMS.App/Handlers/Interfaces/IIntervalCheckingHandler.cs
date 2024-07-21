using SMS.App.Models.Entities;
using SMS.App.Models.Results;

namespace SMS.App.Handlers.Interfaces
{
    public interface IIntervalCheckingHandler
    {
        HashSet<CheckResult> FailedChecks { get; }
        Task CheckAvailability(Target toCheck);
    }
}