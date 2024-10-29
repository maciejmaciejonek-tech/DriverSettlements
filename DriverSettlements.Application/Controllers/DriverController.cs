using DriverSettlements.App.Controllers.Driver;
using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App.Controllers
{
    public class DriverController
    {
        public readonly FuelBillsController FuelBillsController;
        public readonly WeekController WeekController;
        private readonly IDriverAsync _driverService;
        private readonly ApplicationController _applicationController;

        public DriverController(ApplicationController applicationController)
        {
            FuelBillsController = new FuelBillsController(applicationController);
            WeekController = new WeekController();
            _applicationController = applicationController;
            _driverService = ServiceHelper.GetService<IDriverAsync>();
        }

        public async Task<BillingWeek> GetBillingWeek(int yearNumber, int weekNumber)
        {
            var response = await _driverService.GetBillingWeekAsync(new GetBillingWeekRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = yearNumber,
                WeekNumber = weekNumber
            });                
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.BillingWeek;
        }

        public async Task<GetDriverFuelBillsResponse> GetFuelBills(int yearNumber, int weekNumber)
        {
            var response = await _driverService.GetDriverFuelBillsAsync(new GetDriverFuelBillsRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                YearNumber = yearNumber,
                WeekNumber = weekNumber
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response;
        }

        public async Task<bool> UpdateDriverFuelBills(FuelBill fuelBill)
        {
            var response = await _driverService.UpdateDriverFuelBillAsync(new UpdateDriverFuelBillRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                FuelBill = fuelBill
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Result;
        }

        public async Task<byte[]> DownloadDriverFuelBillPhoto(int fuelBillId, double requestedImageWidth, double requestedImageHeight)
        {
            var response = await _driverService.DownloadDriverFuelBillPhotoAsync(new DownloadDriverFuelBillPhotoRequest()
            {
                ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                FuelBillId = fuelBillId,
                RequestedImageWidth = requestedImageWidth,
                RequestedImageHeight = requestedImageHeight
            });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.PhotoContent;
        }

        public void ClearControllers()
        {
            FuelBillsController.ClearController();
            WeekController.ClearController();
        }
    }
}
