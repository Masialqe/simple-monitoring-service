using Microsoft.Extensions.Logging;
using SMS.App.Models.Entities;
using SMS.App.Models.Results;
using System.Net;

namespace SMS.App.Services.Strategies
{
    public class HttpCheckStrategy : ICheckStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpCheckStrategy> _logger;

        /// <summary>
        /// Alowed HTTP status codes.
        /// </summary>
        private readonly HashSet<HttpStatusCode> _allowedStatusCodes = new() 
        { 
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.Forbidden
        };

        public HttpCheckStrategy(HttpClient httpClient, ILogger<HttpCheckStrategy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CheckResult> ExecuteAsync(Target target)
        {
            var result = new CheckResult()
            { Target = target};

            try
            {
                var response = await _httpClient.GetAsync(target.URL);

                if (_allowedStatusCodes.Contains(response.StatusCode))
                    result.SetSuccess();
            }
            catch (HttpRequestException ex)
            {
                result.SetResultMessage(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[HTTP-STRATEGY]: Error sending HTTP reqest: [ex]: {ex}");
                result.SetResultMessage(ex.Message);
            }

            return result;
        }
    }
}
