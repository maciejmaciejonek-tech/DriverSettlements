using DriverSettlements.Common.DataContracts.ApplicationInstance;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App.Controllers
{
    public class ApplicationController
    {
        public ApplicationProfile ApplicationProfile;
        public ApplicationRole ApplicationRole;
        public string ExceptionMessage;

        public readonly Application.ApplicationProfilesListController ApplicationProfilesListController;
        public readonly MasterAdminController MasterAdminController;
        public readonly TenantController TenantController;
        public readonly DriverController DriverController;

        public ApplicationController()
        {
            ApplicationProfilesListController = new Application.ApplicationProfilesListController(this);
            MasterAdminController = new MasterAdminController(this);
            TenantController = new TenantController(this);
            DriverController = new DriverController(this);
        }

        public async Task Initialize(bool applicationInitialize = false)
        {
            try
            {
                if (ApplicationProfile == null)
                {
                    ApplicationProfile = await ApplicationProfilesListController.GetApplicationProfile();
                    ApplicationProfilesListController.SetProfileAsActive(ApplicationProfile);
                }
                var applicationInstanceService = ServiceHelper.GetService<IApplicationInstanceAsync>();
                var response = await applicationInstanceService.GetApplicationInstanceRoleAsync(
                    new GetApplicationInstanceRoleRequest()
                    {
                        ApplicationInstanceId = ApplicationProfile.Id
                    });
                ApplicationRole = response.ApplicationRole;
            }
            catch (Exception exception)
            {
                ExceptionMessage = exception.Message;
            }
            RefreshUserInterface(applicationInitialize);
        }
        public async Task<bool> SwitchToProfile(ApplicationProfile applicationProfile)
        {
            TenantController.ClearControllers();
            MasterAdminController.ClearControllers();
            DriverController.ClearControllers();
            ClearController();
            ApplicationProfile = applicationProfile;
            ApplicationProfilesListController.SetProfileAsActive(ApplicationProfile);
            await Initialize();
            return true;
        }

        public async Task<bool> RegisterApplicationInstance(string invitationCode)
        {
            var applicationInstanceService = ServiceHelper.GetService<IApplicationInstanceAsync>();
            var response = await applicationInstanceService.RegisterApplicationInstanceAsync
            (
                new RegisterApplicationInstanceRequest()
                {
                    InvitationKey = invitationCode,
                    ApplicationInstanceId = ApplicationProfile.Id
                }
            );
            if (!response.Result)
                throw new Exception(response.ErrorMessage);
            ApplicationRole = response.ApplicationRole;
            return true;
        }

        public void RefreshUserInterface(bool applicationInitialize = false)
        {
            var appShell = Microsoft.Maui.Controls.Application.Current.MainPage as AppShell;
            if (appShell != null)
                appShell.RefreshUserInterface(applicationInitialize);
        }

        public void ClearController()
        {
            ApplicationProfile = null;
            ApplicationRole = ApplicationRole.None;
            ExceptionMessage = string.Empty;
        }
    }
}
