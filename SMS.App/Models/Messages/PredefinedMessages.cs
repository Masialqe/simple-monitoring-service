using SMS.App.Models.Entities;
using SMS.App.Models.Results;
using System.Text;

namespace SMS.App.Models.Messages
{
    public static class PredefinedMessages
    {
        /// <summary>
        /// Message to notify failed check, providing unavailable service's details.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static Notification ServiceUnavailableMessage(IEnumerable<CheckResult> services)
        {
            var baseMessage = "Unavailable service detected. Adresses: \n";
            var message = baseMessage + GetTargetSummaryAsString(services);

            return new Notification()
                .SetSubject("[MONITORING]: Unavailable service detected.")
                .SetMessage(message);
        }

        /// <summary>
        /// Creates target summary from checking result.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private static string GetTargetSummaryAsString(IEnumerable<CheckResult> results)
        {
            var builder = new StringBuilder();

            foreach (var result in results)
            {
                builder.AppendLine(@$"<p>Service: {result.Target.Name}<br/>
                                      Address: {result.Target.URL}<br/>
                                      Message: {result.ResultMessage} <br/>
                                      Check: {DateTime.Now}<br/>
                </p>" + "</br>");
            }

            return builder.ToString();
        }
        /// <summary>
        /// Message to notify stopping the service.
        /// </summary>
        /// <returns></returns>
        public static Notification ServiceStoppedMessage()
        {
            return new Notification()
                .SetSubject("[MONITORING]: Monitoring stopped.")
                .SetMessage($"Monitoring service has been stopped at {DateTime.Now}");
        }
 
    }
}
