using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;
using SMS.App.Models.Entities;
using SMS.App.Models.Results;

namespace SMS.App.Services.Strategies
{
    public class PingCheckStrategy : ICheckStrategy
    {
        private readonly ILogger<PingCheckStrategy> _logger;

        //Config values

        /// <summary>
        /// Set ping timeout.
        /// </summary>
        int PING_TIMEOUT = 1000;

        /// <summary>
        /// Set ping delay.
        /// </summary>
        int PING_DELAY_MILLISECONDS = 500;

        /// <summary>
        /// Set ping max-retries count.
        /// </summary>
        int PING_MAX_RETRIES = 5;

        public PingCheckStrategy(ILogger<PingCheckStrategy> logger)
        {
            _logger = logger;
        }
       
        public async Task<CheckResult> ExecuteAsync(Target target)
        {
            var response = new CheckResult()
            { Target = target };

            for (int attempt = 1; attempt <= PING_MAX_RETRIES; attempt++)
            {
                using (var ping = new Ping())
                {
                    try
                    {
                        PingReply reply = await ping.SendPingAsync(target.URL, PING_TIMEOUT);
                        if (reply.Status == IPStatus.Success)
                            response.SetSuccess(true);
                    }
                    catch (PingException ex)
                    {
                        _logger.LogError($"[PING-STRATEGY]: Failed to send ping request: [ex]: {ex}");
                        response.SetResultMessage(ex.Message);   
                    }
          
                    if (attempt < PING_MAX_RETRIES)
                        await Task.Delay(PING_DELAY_MILLISECONDS);
                }
            }

            return response; 
        }
    }
}
