using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.DataContracts.Tenant;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DriverSettlements.TaskExecutor.Services
{
    public abstract class BaseService
    {
        private string _interfaceName;
        private HttpClient _client;
        private JsonSerializerOptions _serializerOptions;
        private IHttpsClientHandlerService _httpsClientHandlerService;

        protected BaseService(IHttpsClientHandlerService service, string interfaceName)
        {
            _interfaceName = interfaceName;
//#if DEBUG
            _httpsClientHandlerService = service;
            HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();
            if (handler != null)
                _client = new HttpClient(handler);
            else
                _client = new HttpClient();
//#else
//            _client = new HttpClient();
//#endif
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        protected async Task<TResponse> ExecuteMethodAsyncWithErrorsHandling<TRequest, TResponse>(string methodName, TRequest request)
        {
            try
            {
                var response = await PostAsync<TResponse>(methodName, request);
                var baseResponse = response as BaseResponse;
                if (baseResponse != null && !baseResponse.Result)
                    throw new Exception(baseResponse.ErrorMessage);
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        protected async Task<T> PostAsync<T>(string methodName, object request)
        {
            Uri uri = UriBuilder.GetUri(_interfaceName, methodName);
            var json = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(uri, stringContent);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, _serializerOptions);
            }
            else
                throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }

    }
}
