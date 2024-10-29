using DriverSettlements.App.Services;
using DriverSettlements.Common.DataContracts.MasterAdmin;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App.Controllers.MasterAdmin
{
    public class TenantsListController : BaseListController<LegalEntity>
    {
        private readonly ApplicationController _applicationController;
        private readonly IMasterAdminAsync _masterAdminService;
        public TenantsListController(ApplicationController applicationController) :base()
        {
            _applicationController = applicationController;
            _masterAdminService = ServiceHelper.GetService<IMasterAdminAsync>();
        }

        public async Task<string> SentInvitationKey(LegalEntity tenant)
        {
            var response = await _masterAdminService.SetTenantInvitationKeyAsync
            (
                new SetTenantInvitationKeyRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    TenantId = tenant.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            tenant.InvitationKey = response.InvitationKey;
            return response.InvitationKey;
        }

        protected async override Task<List<LegalEntity>> GetDataFromService()
        {
            var response = await _masterAdminService.GetTenantsAsync(
                new GetTenantsRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    IncludeDeleted = IncludeDeleted
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Tenants;
        }

        protected override async Task<LegalEntity> AddObjectToService(LegalEntity obj)
        {
            var response = await _masterAdminService.AddTenantAsync(
                new AddTenantRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    Tenant = obj
                });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            obj.Id = response.TenantId.Value;
            return obj;
        }

        protected override async Task<LegalEntity> UpdateObjectInService(LegalEntity obj)
        {
            var response = await _masterAdminService.UpdateTenantAsync
            (
                new UpdateTenantRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    Tenant = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override async Task<LegalEntity> DeleteObjectInService(LegalEntity obj)
        {
            obj.Deleted = true;
            var response = await _masterAdminService.DeleteTenantAsync(
                new DeleteTenantRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    TenantId = obj.Id
                });
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override List<LegalEntity> FilterData(string filerString)
        {
            if (string.IsNullOrWhiteSpace(filerString))
                return Objects.Where(t => IncludeDeleted || !t.Deleted).OrderBy(t => t.FriendlyName).ToList();
            else
                return Objects.Where(t => IncludeDeleted || !t.Deleted).Where(t =>
                                    !string.IsNullOrWhiteSpace(t.FriendlyName) && t.FriendlyName.StartsWith(filerString, StringComparison.InvariantCultureIgnoreCase)
                                    || !string.IsNullOrWhiteSpace(t.CompanyName) && t.CompanyName.StartsWith(filerString, StringComparison.InvariantCultureIgnoreCase)).OrderBy(t => t.FriendlyName).ToList();
        }

    }
}
