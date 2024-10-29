using DriverSettlements.Common.DataContracts.DriverInterface;

namespace DriverSettlements.Common.Interfaces
{
    public interface IDriver
    {
        GetBillingWeekResponse GetBillingWeek(GetBillingWeekRequest request);
        GetDriverFuelBillsResponse GetDriverFuelBills(GetDriverFuelBillsRequest request);
        AddDriverFuelBillResponse AddDriverFuelBill(AddDriverFuelBillRequest request);
        UpdateDriverFuelBillResponse UpdateDriverFuelBill(UpdateDriverFuelBillRequest request);
        DeleteDriverFuelBillResponse DeleteDriverFuelBill(DeleteDriverFuelBillRequest request);
        UploadDriverFuelBillPhotoResponse UploadDriverFuelBillPhoto(UploadDriverFuelBillPhotoRequest request);
        DownloadDriverFuelBillPhotoResponse DownloadDriverFuelBillPhoto(DownloadDriverFuelBillPhotoRequest request);

        string Ping(string ping);
        string Test();
    }
}
