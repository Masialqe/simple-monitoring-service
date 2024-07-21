using Microsoft.Extensions.Logging;
using SMS.App.Models.Exceptions;
using System.Net.Security;

namespace SMS.App.Services.Strategies
{
    public class HttpWithSSLCheckStrategy : HttpCheckStrategy
    {
        public HttpWithSSLCheckStrategy(ILogger<HttpWithSSLCheckStrategy> logger)
            : base(CreateHttpClientWithSslValidation(), logger) { }
    
        private static HttpClient CreateHttpClientWithSslValidation()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, sslPolicyErrors) =>
                {
                    if (sslPolicyErrors != SslPolicyErrors.None)
                    {
                        throw new SSLCertificateException("Invalid SSL Certificate for given host.");
                    }
                    return true;
                }
            };
            return new HttpClient(handler);
        }
    }

}
