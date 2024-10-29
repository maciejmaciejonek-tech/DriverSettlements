using DriverSettlements.Common.DataContracts.DriverInterface;

namespace DriverSettlements.Common.Interfaces
{
    public interface IDriverAsync
    {
        Task<GetBillingWeekResponse> GetBillingWeekAsync (GetBillingWeekRequest request);
        Task<GetDriverFuelBillsResponse> GetDriverFuelBillsAsync(GetDriverFuelBillsRequest request);
        Task<AddDriverFuelBillResponse> AddDriverFuelBillAsync(AddDriverFuelBillRequest request);
        Task<UpdateDriverFuelBillResponse> UpdateDriverFuelBillAsync(UpdateDriverFuelBillRequest request);
        Task<DeleteDriverFuelBillResponse> DeleteDriverFuelBillAsync(DeleteDriverFuelBillRequest request);
        Task<UploadDriverFuelBillPhotoResponse> UploadDriverFuelBillPhotoAsync(UploadDriverFuelBillPhotoRequest request);
        Task<DownloadDriverFuelBillPhotoResponse> DownloadDriverFuelBillPhotoAsync(DownloadDriverFuelBillPhotoRequest request);

        Task<string> PingAsync(string ping);
    }
}
