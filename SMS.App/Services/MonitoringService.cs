using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SMS.App.Handlers.Interfaces;
using SMS.App.Services.Interfaces;
using SMS.App.Models.Entities;

namespace SMS.App.Services
{
    public class MonitoringService : IHostedService, IDisposable
    {
        //dependencies
        private readonly ILogger<MonitoringService> _logger;
        private readonly IIntervalCheckingHandler _checkingHandler;
        private readonly INotificationHandler _notificationHandler;
        private readonly IConfigurationService _configurationService;

        //globals
        private Timer? _timer;
        private CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// Max amount of concurent operations.
        /// </summary>
        private const int MAX_CONCURENT_OPERATIONS = 5;
        SemaphoreSlim semaphore = new SemaphoreSlim(MAX_CONCURENT_OPERATIONS);

        public MonitoringService(ILogger<MonitoringService> logger, 
            IIntervalCheckingHandler checkingHandler,
            INotificationHandler notificationHandler,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _checkingHandler = checkingHandler;
            _notificationHandler = notificationHandler;
            _configurationService = configurationService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[SERVICE]: Service has been started.");
            _cancellationTokenSource = 
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _timer = new Timer(
                async state => await DoWork(_cancellationTokenSource.Token),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(_configurationService.IntervalSettings.CheckInterval));

            return Task.CompletedTask;
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            if(!IsWorkingTime() || cancellationToken.IsCancellationRequested)
                return;

            var targets = _configurationService.Targets;

            if(targets == null || targets.Count() < 0)  return;

            List<Task> tasks = new List<Task>();

            foreach (var test in targets)
            {
                tasks.Add(ExecuteCheck(test, cancellationToken));
            }

            await Task.WhenAll(tasks);

            await _notificationHandler.NotifyFailedCheckAsync(_checkingHandler.FailedChecks);
        }

        private async Task ExecuteCheck(Target test, CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken); 

            try
            {
               await _checkingHandler.CheckAvailability(test); 
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("[SERVICE]: Check execution was canceled.");
            }
            finally
            {
                semaphore.Release(); 
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[SERVICE]: Service has been stopped.");
            _notificationHandler.NotifyStoppingService();
            _cancellationTokenSource?.Cancel();
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _timer?.Dispose();
        }

        /// <summary>
        /// Checks if current time fits between start and stop hour.
        /// </summary>
        /// <returns></returns>
        private bool IsWorkingTime()
        {
            var currentHour = DateTime.Now.Hour;
            var startTime = _configurationService.IntervalSettings.StartTime;
            var stopTime = _configurationService.IntervalSettings.StopTime;

            return currentHour >= startTime && currentHour <= stopTime;
        }

    }
}
