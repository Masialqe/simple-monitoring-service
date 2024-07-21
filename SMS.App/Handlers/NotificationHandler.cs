using Microsoft.Extensions.Logging;
using SMS.App.Handlers.Interfaces;
using SMS.App.Services.Interfaces;
using SMS.App.Models.Messages;
using SMS.App.Models.Results;

namespace SMS.App.Handlers
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly INotificationService _messageService;
        private readonly ILogger<NotificationHandler> _logger;

        public NotificationHandler(INotificationService messageService, ILogger<NotificationHandler> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        /// <summary>
        /// Notify about failed check's result.
        /// </summary>
        /// <param name="unavailableServices"></param>
        /// <returns></returns>
        public async Task NotifyFailedCheckAsync(IEnumerable<CheckResult> unavailableServices)
        {
            if (unavailableServices.Any())
            {
                var notification = PredefinedMessages.ServiceUnavailableMessage(unavailableServices);
                await _messageService.SendNotificationAsync(notification);
            }
        }

        /// <summary>
        /// Notify about stopping the service.
        /// </summary>
        /// <returns></returns>
        public async Task NotifyStoppingService()
        {
            var notification = PredefinedMessages.ServiceStoppedMessage();
            await _messageService.SendNotificationAsync(notification);
        }
    }
}
