using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;
using System.Collections.Generic;

namespace DriverSettlements.App
{
    public partial class AppShell : ExtendedShell
    {
        private readonly ApplicationController _applicationController;
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ApplicationProfileDetailsPage), typeof(ApplicationProfileDetailsPage));
            Routing.RegisterRoute(nameof(DriverDetailsPage), typeof(DriverDetailsPage));
            Routing.RegisterRoute(nameof(DriverFuelBillPage), typeof(DriverFuelBillPage));
            Routing.RegisterRoute(nameof(DriversConfirmationAndMappingsPage), typeof(DriversConfirmationAndMappingsPage));
            Routing.RegisterRoute(nameof(TenantFuelBillsPage), typeof(TenantFuelBillsPage));
            Routing.RegisterRoute(nameof(TenantFuelBillVerificationPage), typeof(TenantFuelBillVerificationPage));
            Routing.RegisterRoute(nameof(TenantTariffPlanPage), typeof(TenantTariffPlanPage));
            Routing.RegisterRoute(nameof(TenantSettlementItemsPage), typeof(TenantSettlementItemsPage));
            Routing.RegisterRoute(nameof(TenantSettlementItemPage), typeof(TenantSettlementItemPage));
            Routing.RegisterRoute(nameof(TenantSettlementTypesPage), typeof(TenantSettlementTypesPage));
            Routing.RegisterRoute(nameof(TenantSettlementTypePage), typeof(TenantSettlementTypePage));
            Routing.RegisterRoute(nameof(TenantTariffPlanAssignmentsPage), typeof(TenantTariffPlanAssignmentsPage));
            Routing.RegisterRoute(nameof(TenantTariffPlanAssignmentPage), typeof(TenantTariffPlanAssignmentPage));
            Routing.RegisterRoute(nameof(TenantDetailsPage), typeof(TenantDetailsPage));
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            this.Appearing += AppShell_Appearing;
        }

        private async void AppShell_Appearing(object sender, EventArgs e)
        {
            await _applicationController.Initialize(true);
        }

        public async void RefreshUserInterface(bool applicationInitialize = false)
        {
            try
            {
                var listOfTabs = new HashSet<BaseShellItem>()
                {
                    MasterAdminTenantsShellContent, MasterAdminSettingsShellContent,
                    TenantWeeklySettlementsShellContent, TenantDriverListShellContent, TenantCarsShellContent, TenantExternalServicesShellContent,
                    DriverWeeklySetlementsShellContent, DriverFuelBillsShellContent, DriverCarInspectionsShellContent, DriverInfoShellContent,
                    ErrorShellContent,
                    InvitationShellContent,
                    SettingsShellContent,
                    ProfilesShellContent,
                    WelcomeShellContent,
                };
                var visibleTabs = new HashSet<BaseShellItem>();
                string route = string.Empty;
                if (!string.IsNullOrWhiteSpace(_applicationController.ExceptionMessage))
                {
                    visibleTabs.Add(ErrorShellContent);
                }
                else
                {
                    switch (_applicationController.ApplicationRole)
                    {
                        case ApplicationRole.None:
                            visibleTabs.Add(InvitationShellContent);
                            visibleTabs.Add(ProfilesShellContent);
                            route = "Invitation";
                            break;
                        case ApplicationRole.MasterAdmin:
                            visibleTabs.Add(MasterAdminTenantsShellContent);
                            visibleTabs.Add(MasterAdminSettingsShellContent);
                            visibleTabs.Add(ProfilesShellContent);
                            route = "MasterAdmin";
                            break;
                        case ApplicationRole.Tenant:
                            visibleTabs.Add(TenantDriverListShellContent);
                            visibleTabs.Add(TenantWeeklySettlementsShellContent);
                            visibleTabs.Add(TenantCarsShellContent);
                            visibleTabs.Add(TenantExternalServicesShellContent);
                            visibleTabs.Add(ProfilesShellContent);
                            route = "Tenant";
                            break;
                        case ApplicationRole.Driver:
                            visibleTabs.Add(DriverWeeklySetlementsShellContent);
                            visibleTabs.Add(DriverFuelBillsShellContent);
                            visibleTabs.Add(DriverCarInspectionsShellContent);
                            visibleTabs.Add(DriverInfoShellContent);
                            visibleTabs.Add(ProfilesShellContent);
                            route = "Driver";
                            break;
                        case ApplicationRole.Blocked:
                        default:
                            visibleTabs.Add(WelcomeShellContent);
                            visibleTabs.Add(ProfilesShellContent);
                            break;
                    }
                }
#if IOS
                if (!ProfilesShellContent.IsVisible)
                    ProfilesShellContent.IsVisible = true;
#endif
                foreach (var item in listOfTabs)
                {
                    if (!visibleTabs.Contains(item))
                        item.IsVisible = false;
                }
                foreach (var item in visibleTabs)
                {
                    if (!item.IsVisible)
                        item.IsVisible = true;
                }
                if (!applicationInitialize)
                    await Shell.Current.GoToAsync("..");
                if (!string.IsNullOrWhiteSpace(route))
                    await this.GoToAsync($"//{route}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("App title", ex.Message, "OK");

            }
        }
    }
}