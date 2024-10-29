using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using Microsoft.AspNetCore.Mvc;

namespace DriverSettlements.WebAPI.Controllers.Interfaces
{
    public interface IDriverController
    {
        IActionResult GetBillingWeek(GetBillingWeekRequest request);
        IActionResult GetDriverFuelBills(GetDriverFuelBillsRequest request);
        IActionResult AddDriverFuelBill(AddDriverFuelBillRequest request);
        IActionResult UpdateDriverFuelBill(UpdateDriverFuelBillRequest request);
        IActionResult DeleteDriverFuelBill(DeleteDriverFuelBillRequest request);
        IActionResult UploadDriverFuelBillPhoto(UploadDriverFuelBillPhotoRequest request);
        IActionResult DownloadDriverFuelBillPhoto(DownloadDriverFuelBillPhotoRequest request);
        IActionResult Ping(string ping);
        IActionResult Test();
    }
}
