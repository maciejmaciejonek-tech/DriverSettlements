using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App.Controllers.Tenant
{
    public class FuelBillsController : BaseListController<FuelBill>
    {
        private int _yearNumber;
        private int _weekNumber;
        private int? _driverId;
        private readonly ApplicationController _applicationController;
        private readonly ITenantAsync _tenantService;

        public FuelBillsController(ApplicationController applicationController) : base()
        {
            _applicationController = applicationController;
            _tenantService = ServiceHelper.GetService<ITenantAsync>();
        }

        public void SetYearAndWeekAndDriver(int yearNumber, int weekNumber, int? driverId=null)
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
            if (driverId.HasValue!=_driverId.HasValue) 
            {
                _driverId = driverId;
                changed = true;
            }
            else if (driverId.HasValue && !driverId.Value.Equals(_driverId.Value))
            {
                _driverId = driverId;
                changed = true;
            }
            if (changed)
                ForceDataRefresh = true;
        }

        public async Task<GetDriverFuelBillsInfosResponse> GetDriverFuelBillsInfos()
        {
            return await _applicationController.TenantController.GetDriverFuelBillsInfos(_yearNumber,_weekNumber);
        }

        protected async override Task<List<FuelBill>> GetDataFromService()
        {
            var fuelBills = await _applicationController.TenantController.GetTenantFuelBills(_yearNumber, _weekNumber, _driverId);
            if (IncludeDeleted)
                return fuelBills;
            else
                return fuelBills.Where(b => b.Status != FuelBillStatus.Deleted).ToList();
        }

        protected override async Task<FuelBill> AddObjectToService(FuelBill obj)
        {
            throw new Exception("Not implemented method");
        }

        protected override async Task<FuelBill> UpdateObjectInService(FuelBill obj)
        {
            var response = await _tenantService.UpdateTenantFuelBillAsync(
                new UpdateTenantFuelBillRequest()
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
            throw new Exception("Not implemented method");
        }

        protected override List<FuelBill> FilterData(string filerString)
        {
            if (string.IsNullOrWhiteSpace(filerString))
                return Objects.Where(t => IncludeDeleted || t.Status != FuelBillStatus.Deleted).OrderBy(t => t.Date).ThenBy(t => t.ReferenceNumber).ToList();
            else
                return Objects.Where(t => IncludeDeleted || t.Status != FuelBillStatus.Deleted).Where(t =>
                                    !string.IsNullOrWhiteSpace(t.ReferenceNumber) && (t.ReferenceNumber.StartsWith(filerString, StringComparison.InvariantCultureIgnoreCase)
                                    || t.ReferenceNumber.Contains($" {filerString}", StringComparison.InvariantCultureIgnoreCase))).OrderBy(t => t.Date).ThenBy(t => t.ReferenceNumber).ToList();
        }

    }
}
