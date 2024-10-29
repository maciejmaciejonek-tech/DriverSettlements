using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Interfaces;

namespace DriverSettlements.App.Controllers.Tenant
{
    public class DriversController : BaseListController<Common.Models.Driver>
    {
        private readonly ApplicationController _applicationController;
        private readonly ITenantAsync _tenantService;
        public DriversController(ApplicationController applicationController) : base()
        {
            _applicationController = applicationController;
            _tenantService = ServiceHelper.GetService<ITenantAsync>();
        }

        public async Task<string> SentInvitationKey(Common.Models.Driver driver)
        {
            var response = await _tenantService.SetDriverInvitationKeyAsync
            (
                new SetDriverInvitationKeyRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    DriverId = driver.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            driver.InvitationKey = response.InvitationKey;
            return response.InvitationKey;
        }

        protected async override Task<List<Common.Models.Driver>> GetDataFromService()
        {
            var response = await _tenantService.GetDriversAsync
            (
                new GetDriversRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    IncludeDeleted = IncludeDeleted
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return response.Drivers;
        }

        protected override async Task<Common.Models.Driver> AddObjectToService(Common.Models.Driver obj)
        {
            var response = await _tenantService.AddDriverAsync
            (
                 new AddDriverRequest()
                 {
                     ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                     Driver = obj
                 }
             );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            obj.Id = response.DriverId.Value;
            return obj;
        }

        protected override async Task<Common.Models.Driver> UpdateObjectInService(Common.Models.Driver obj)
        {
            var response = await _tenantService.UpdateDriverAsync(
                new UpdateDriverRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    Driver = obj
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override async Task<Common.Models.Driver> DeleteObjectInService(Common.Models.Driver obj)
        {
            obj.Deleted = true;
            var response = await _tenantService.DeleteDriverAsync
            (
                new DeleteDriverRequest()
                {
                    ApplicationInstanceId = _applicationController.ApplicationProfile.Id,
                    DriverId = obj.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            return obj;
        }

        protected override List<Common.Models.Driver> FilterData(string filerString)
        {
            if (string.IsNullOrWhiteSpace(filerString))
                return Objects.Where(t => IncludeDeleted || !t.Deleted).OrderBy(t => t.CompanyName).ToList();
            else
                return Objects.Where(t => IncludeDeleted || !t.Deleted).Where(t =>
                                    !string.IsNullOrWhiteSpace(t.CompanyName) && (t.CompanyName.StartsWith(filerString, StringComparison.InvariantCultureIgnoreCase)
                                    || t.CompanyName.Contains($" {filerString}", StringComparison.InvariantCultureIgnoreCase))).OrderBy(t => t.FriendlyName).ToList();
        }
    }
}
