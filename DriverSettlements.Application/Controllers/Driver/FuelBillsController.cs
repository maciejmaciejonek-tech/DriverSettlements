using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App.Controllers.Driver
{
    public class FuelBillsController : BaseListController<FuelBill>
    {
        public BillingWeekStatus BillingWeekStatus { get; private set; } = BillingWeekStatus.NotExists;
        private int _yearNumber;
        private int _weekNumber;
        private readonly ApplicationController _applicationController;
        private readonly IDriverAsync _driverService;

        public FuelBillsController(ApplicationController applicationController) : base()
        {
            _applicationController = applicationController;
            _driverService = ServiceHelper.GetService<IDriverAsync>();
        }

        public void SetYearAndWeekNumbers(int yearNumber, int weekNumber)
        {
            bool changed = false;
            if (_yearNumber != yearNumber)
            {
                _yearNumber = yearNumber;
                changed = true;
            }
            if (_weekNumber != weekNumber)
            {
                _weekNumber = weekNumber;
                changed = true;
            }
            if (changed)
                this.ForceDataRefresh = true;
        }

        protected async override Task<List<FuelBill>> GetDataFromService()
        {
            var response = await _applicationController.DriverController.GetFuelBills(_yearNumber, _weekNumber);
            BillingWeekStatus = response.BillingWeekStatus;
            if (IncludeDeleted)
                return response.FuelBills;
            else
                return response.FuelBills.Where(b => b.Status != FuelBillStatus.Deleted).ToList();
        }

        protected override async Task<FuelBill> AddObjectToService(FuelBill obj)
        {
            var response = await _driverService.AddDriverFuelBillAsync
           (
                new AddDriverFuelBillRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    FuelBill = obj
                }
            ); 
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            obj.Id = response.FuelBillId.Value;
            return obj;
        }

        protected override async Task<FuelBill> UpdateObjectInService(FuelBill obj)
        {
            var response = await _driverService.UpdateDriverFuelBillAsync(
                new UpdateDriverFuelBillRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    FuelBill = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;

        }

        protected override async Task<FuelBill> DeleteObjectInService(FuelBill obj)
        {
            obj.Status = FuelBillStatus.Deleted;
            var response = await _driverService.DeleteDriverFuelBillAsync
            (
                new DeleteDriverFuelBillRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    FuelBillId = obj.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override List<FuelBill> FilterData(string filerString)
        {
            if (string.IsNullOrWhiteSpace(filerString))
                return Objects.Where(t => IncludeDeleted || t.Status!=FuelBillStatus.Deleted).OrderBy(t => t.Date).ThenBy(t=>t.ReferenceNumber) .ToList();
            else
                return Objects.Where(t => IncludeDeleted || t.Status != FuelBillStatus.Deleted).Where(t =>
                                    !string.IsNullOrWhiteSpace(t.ReferenceNumber) && (t.ReferenceNumber.StartsWith(filerString, StringComparison.InvariantCultureIgnoreCase)
                                    || t.ReferenceNumber.Contains($" {filerString}", StringComparison.InvariantCultureIgnoreCase))).OrderBy(t => t.Date).ThenBy(t=>t.ReferenceNumber) .ToList();
        }

        public async Task<bool> UploadDriverFuelBillPhoto(int fuelBillId, byte[] photoFileContent )
        {
            var response = await _driverService.UploadDriverFuelBillPhotoAsync(
                new UploadDriverFuelBillPhotoRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    FuelBillId = fuelBillId,
                    PhotoContent = photoFileContent
                });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return true;
        }
    }
}
