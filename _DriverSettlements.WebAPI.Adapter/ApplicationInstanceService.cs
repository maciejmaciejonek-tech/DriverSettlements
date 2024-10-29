using DriverSettlements.Common.DataContracts;
using DriverSettlements.Common.Interfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DriverSettlements.WebAPI.Adapter
{
    public class ApplicationInstanceService : IApplicationInstance
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        IHttpsClientHandlerService _httpsClientHandlerService;

        public ApplicationInstanceService(IHttpsClientHandlerService service)
        {
#if DEBUG
            _httpsClientHandlerService = service;
            HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();
            if (handler != null)
                _client = new HttpClient(handler);
            else
                _client = new HttpClient();
#else
            _client = new HttpClient();
#endif
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public GetApplicationInstanceRoleResponse GetApplicationInstanceRole(GetApplicationInstanceRoleRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<GetApplicationInstanceRoleResponse> GetApplicationInstanceRoleAsync(GetApplicationInstanceRoleRequest request)
        {
            Uri uri = new Uri(string.Format(Constants.RestUrl, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<List<TodoItem>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;

        }

        public RegisterApplicationInstanceResponse RegisterApplicationInstance(RegisterApplicationInstanceRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RegisterApplicationInstanceResponse> RegisterApplicationInstanceAsync(RegisterApplicationInstanceRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
