using DriverSettlements.Common.DataContracts.Tenant;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.TaskExecutor.Services
{
    public class TenantService: BaseService
    {
        public TenantService(IHttpsClientHandlerService service) : base(service, "Tenant")
        {
        }

        public async Task<DownloadExternalReportResponse> DownloadExternalReportAsync(DownloadExternalReportRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DownloadExternalReportRequest, DownloadExternalReportResponse>("DownloadExternalReport", request);
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
