using DriverSettlements.Common.DataContracts.ApplicationInstance;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DriverSettlements.App.Services
{
    public class ApplicationInstanceService : BaseService, IApplicationInstanceAsync
    {
        public ApplicationInstanceService(IHttpsClientHandlerService service):base(service, "ApplicationInstance")
        {
        }

        public async Task<GetApplicationInstanceRoleResponse> GetApplicationInstanceRoleAsync(GetApplicationInstanceRoleRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetApplicationInstanceRoleRequest, GetApplicationInstanceRoleResponse>("GetApplicationInstanceRole", request);
        }

        public async Task<RegisterApplicationInstanceResponse> RegisterApplicationInstanceAsync(RegisterApplicationInstanceRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<RegisterApplicationInstanceRequest, RegisterApplicationInstanceResponse>("RegisterApplicationInstance", request);
        }

        public async Task<string> PingAsync(string ping)
        {
            try
            {
                return await PostAsync<string>("Ping", ping);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
