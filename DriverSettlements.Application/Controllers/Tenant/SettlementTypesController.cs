using DriverSettlements.App.Helpers;
using DriverSettlements.Common.DataContracts.DriverInterface;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;
using System.Resources;

namespace DriverSettlements.App.Controllers.Tenant
{
    public class SettlementTypesController : BaseListController<SettlementType>
    {
        private int _yearNumber;
        private int _weekNumber;
        private int? _driverId;
        private readonly ApplicationController _applicationController;
        private readonly ITenantAsync _tenantService;

        public SettlementTypesController(ApplicationController applicationController) : base()
        {
            _applicationController = applicationController;
            _tenantService = ServiceHelper.GetService<ITenantAsync>();
        }

        protected async override Task<List<SettlementType>> GetDataFromService()
        {
            var response = await _tenantService.GetSettlementTypesAsync
            (
                new GetSettlementTypesRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.SettlementTypes.Select(Translate.TranslateSettlementType).OrderBy(st=>st.TypeName).ToList();
        }

        protected override async Task<SettlementType> AddObjectToService(SettlementType obj)
        {
            var response = await _tenantService.AddSettlementTypeAsync
            (
                new AddSettlementTypeRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    SettlementType = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            obj.Id = response.SettlementTypeId.Value;
            return obj;
        }

        protected override async Task<SettlementType> UpdateObjectInService(SettlementType obj)
        {
            var response = await _tenantService.UpdateSettlementTypeAsync(
                new UpdateSettlementTypeRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    SettlementType = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override async Task<SettlementType> DeleteObjectInService(SettlementType obj)
        {
            obj.Enabled = false;
            var response = await _tenantService.DeleteSettlementTypeAsync
            (
                new DeleteSettlementTypeRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    SettlementTypeId = obj.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override List<SettlementType> FilterData(string filerString)
        {
            if (string.IsNullOrWhiteSpace(filerString))
                return Objects.Where(t => IncludeDeleted || t.Enabled == true).OrderBy(t => t.TypeName).ToList();
            else
                return Objects.Where(t => IncludeDeleted || t.Enabled == true).Where(t =>
                                    !string.IsNullOrWhiteSpace(t.TypeName) && (t.TypeName.StartsWith(filerString, StringComparison.InvariantCultureIgnoreCase)
                                    || t.TypeName.Contains($" {filerString}", StringComparison.InvariantCultureIgnoreCase))).OrderBy(t => t.TypeName).ToList();
        }
    }
}
