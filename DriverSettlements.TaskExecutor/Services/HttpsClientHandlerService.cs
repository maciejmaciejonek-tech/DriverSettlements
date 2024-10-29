using System.Net.Security;

namespace DriverSettlements.TaskExecutor.Services
{
    public class HttpsClientHandlerService : IHttpsClientHandlerService
    {
        public HttpMessageHandler GetPlatformMessageHandler()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => 
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return sslPolicyErrors == SslPolicyErrors.None;
            };
            return handler;
        }
    }
}
