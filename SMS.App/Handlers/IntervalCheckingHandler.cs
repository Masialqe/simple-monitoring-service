using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.App.Handlers.Interfaces;
using SMS.App.Services.Strategies;
using SMS.App.Models.Entities;
using SMS.App.Models.Results;
using SMS.App.Models.Enums;

namespace SMS.App.Handlers
{
    public class IntervalCheckingHandler : IIntervalCheckingHandler
    {
        private readonly ICheckStrategy _httpStrategy;
        private readonly ICheckStrategy _pingStrategy;
        private readonly ILogger<IntervalCheckingHandler> _logger;

        public HashSet<CheckResult> FailedChecks { get; private set; }
            = new HashSet<CheckResult>();

        private readonly object _lock = new object();

        public IntervalCheckingHandler([FromKeyedServices("httpStrategy")]ICheckStrategy httpStrategy, 
            [FromKeyedServices("pingStrategy")]ICheckStrategy pingStrategy, ILogger<IntervalCheckingHandler> logger)
        {
            _httpStrategy = httpStrategy;
            _pingStrategy = pingStrategy;
            _logger = logger;
        }

        /// <summary>
        /// Executes check on given target.
        /// </summary>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        public async Task CheckAvailability(Target toCheck)
        {
            ICheckStrategy checkType = GetStrategyBasedOnCheckType(toCheck);

            var result = await checkType.ExecuteAsync(toCheck);
            var isAlreadyFailed = FailedChecks.Any(x => x.Target.URL == toCheck.URL);

            lock (_lock)
            {
           
                if(!result.IsSuccess && !isAlreadyFailed) 
                    FailedChecks.Add(result);

                if(result.IsSuccess &&  isAlreadyFailed)
                {
                    var targetToRemove = FailedChecks.First(x => x.Target.URL == toCheck.URL);
                    FailedChecks.Remove(targetToRemove);
                }      
            }
            _logger.LogInformation($"[CHECK-HANDLER] - TEST: {toCheck.URL}, RESULT: {result}");
        }

        /// <summary>
        /// Get strategy based on check type.
        /// </summary>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private ICheckStrategy GetStrategyBasedOnCheckType(Target toCheck)
        {
            return toCheck.Type switch
            {
                CheckTypes.Ping => _pingStrategy,
                CheckTypes.Http => _httpStrategy,
                _ => throw new ArgumentException("Invalid check type", nameof(toCheck.Type))
            };
        }
    }
}
