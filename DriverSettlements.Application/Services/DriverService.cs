using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using System.Diagnostics;

namespace DriverSettlements.App.Services
{
    public class DriverService : BaseService, IDriverAsync
    {
        public DriverService(IHttpsClientHandlerService service) : base(service, "Driver")
        {
        }

        public async Task<GetBillingWeekResponse> GetBillingWeekAsync(GetBillingWeekRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetBillingWeekRequest, GetBillingWeekResponse>("GetBillingWeek", request);
        }

        public async Task<GetDriverFuelBillsResponse> GetDriverFuelBillsAsync(GetDriverFuelBillsRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<GetDriverFuelBillsRequest, GetDriverFuelBillsResponse>("GetDriverFuelBills", request);
        }

        public async Task<AddDriverFuelBillResponse> AddDriverFuelBillAsync(AddDriverFuelBillRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<AddDriverFuelBillRequest, AddDriverFuelBillResponse>("AddDriverFuelBill", request);
        }

        public async Task<UpdateDriverFuelBillResponse> UpdateDriverFuelBillAsync(UpdateDriverFuelBillRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UpdateDriverFuelBillRequest, UpdateDriverFuelBillResponse>("UpdateDriverFuelBill", request);
        }

        public async Task<DeleteDriverFuelBillResponse> DeleteDriverFuelBillAsync(DeleteDriverFuelBillRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DeleteDriverFuelBillRequest, DeleteDriverFuelBillResponse>("DeleteDriverFuelBill", request);
        }

        public async Task<UploadDriverFuelBillPhotoResponse> UploadDriverFuelBillPhotoAsync(UploadDriverFuelBillPhotoRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<UploadDriverFuelBillPhotoRequest, UploadDriverFuelBillPhotoResponse>("UploadDriverFuelBillPhoto", request);
        }
        public async Task<DownloadDriverFuelBillPhotoResponse> DownloadDriverFuelBillPhotoAsync(DownloadDriverFuelBillPhotoRequest request)
        {
            return await ExecuteMethodAsyncWithErrorsHandling<DownloadDriverFuelBillPhotoRequest, DownloadDriverFuelBillPhotoResponse>("DownloadDriverFuelBillPhoto", request);
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
