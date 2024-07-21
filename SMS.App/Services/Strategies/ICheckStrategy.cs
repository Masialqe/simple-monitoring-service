using SMS.App.Models.Entities;
using SMS.App.Models.Results;

namespace SMS.App.Services.Strategies
{
    public interface ICheckStrategy
    {
        Task<CheckResult> ExecuteAsync(Target target);
    }
}
