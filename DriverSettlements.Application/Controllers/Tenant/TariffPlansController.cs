using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App.Controllers.Tenant
{
    public class TariffPlansController : BaseListController<TariffPlan>
    {
        private int _yearNumber;
        private int _weekNumber;
        private int? _driverId;
        private readonly ApplicationController _applicationController;
        private readonly ITenantAsync _tenantService;

        public TariffPlansController(ApplicationController applicationController) : base()
        {
            _applicationController = applicationController;
            _tenantService = ServiceHelper.GetService<ITenantAsync>();
        }

        protected async override Task<List<TariffPlan>> GetDataFromService()
        {
            var response = await _tenantService.GetTariffPlansAsync
            (
                new GetTariffPlansRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    IncludeDeleted = IncludeDeleted
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.TariffPlans;
        }

        protected override async Task<TariffPlan> AddObjectToService(TariffPlan obj)
        {
            var response = await _tenantService.AddTariffPlanAsync
            (
                new AddTariffPlanRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    TariffPlan = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            obj.Id = response.TariffPlanId.Value;
            return obj;
        }

        protected override async Task<TariffPlan> UpdateObjectInService(TariffPlan obj)
        {
            var response = await _tenantService.UpdateTariffPlanAsync(
                new UpdateTariffPlanRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    TariffPlan = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override async Task<TariffPlan> DeleteObjectInService(TariffPlan obj)
        {
            obj.Enabled = false;
            var response = await _tenantService.DeleteTariffPlanAsync
            (
                new DeleteTariffPlanRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    TariffPlanId = obj.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override List<TariffPlan> FilterData(string filerString)
        {
            if (string.IsNullOrWhiteSpace(filerString))
                return Objects.Where(t => IncludeDeleted || t.Enabled == true).OrderBy(t => t.Name).ToList();
            else
                return Objects.Where(t => IncludeDeleted || t.Enabled == true).Where(t =>
                                    !string.IsNullOrWhiteSpace(t.Name) && (t.Name.StartsWith(filerString, StringComparison.InvariantCultureIgnoreCase)
                                    || t.Name.Contains($" {filerString}", StringComparison.InvariantCultureIgnoreCase))).OrderBy(t => t.Name).ToList();
        }

    }
}
