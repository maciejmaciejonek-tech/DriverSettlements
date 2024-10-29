using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Globalization;

namespace DriverSettlements.App
{
    public partial class TenantExternalServicesSettingsPage : ContentPage
    {
        public Tenant Tenant { get; private set; }
        private string _tenantSignature;
        private readonly ApplicationController _applicationController;

        public TenantExternalServicesSettingsPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Appearing += TenantExternalServicesSettingsPage_Appearing;
            Disappearing += TenantExternalServicesSettingsPage_Disappearing;
        }

        private async void TenantExternalServicesSettingsPage_Appearing(object sender, EventArgs e)
        {
            try
            {
                if (await _applicationController.TenantController.EnsureTenant())
                {
                    Tenant = _applicationController.TenantController.Tenant;
                    BindingContext = this;
                    _tenantSignature = GetSignature(Tenant);                 
                }
                SetButtonsAccesability();
            }
            catch (Exception ex)
            {
                SetButtonsAccesability();
                await DisplayAlert(ApplicationStrings.ExceptionMessageTitle, ex.Message, "OK");
            }

        }

        private async void TenantExternalServicesSettingsPage_Disappearing(object sender, EventArgs e)
        {
            var signature = GetSignature(Tenant);
            if (!signature.Equals(_tenantSignature))
            {
                try
                {
                    await _applicationController.TenantController.SaveTenant();
                }
                catch (Exception ex)
                {
                    await DisplayAlert(ApplicationStrings.Actions_SavingData, ex.Message, "OK");
                }
            }
        }

        private void SetButtonsAccesability()
        {
            ButtonBoltTest.IsEnabled = Tenant!=null;
            ButtonUberTest.IsEnabled = Tenant != null;
            ButtonFreenowTest.IsEnabled = Tenant != null;
        }

        private async void ButtonBoltTest_Clicked(object sender, EventArgs e)
        {
            if (!await ValidateValueRequired(BoltCompanyId, "ID"))
                return;
            //id 5 digits
            if (!await ValidateValueLength(BoltCompanyId, "ID", 5))
                return;
            if (!await ValidateValueRequired(BoltUserName, ApplicationStrings.LegalEntity_EmailAddress))
                return;
            if (!await ValidateValueRequired(BoltPassword, ApplicationStrings.Actions_Password))
                return;
            ButtonBoltTest.IsInProgress = true;
            try
            {
                var signature = GetSignature(Tenant);
                if (!signature.Equals(_tenantSignature))
                {
                    if (await _applicationController.TenantController.SaveTenant())
                        _tenantSignature = GetSignature(Tenant);
                }
                if (await _applicationController.TenantController.CheckDownloadingReport(ExternalReportType.Bolt))
                {
                    ButtonBoltTest.IsInProgress = false;
                    await DisplayAlert(ApplicationStrings.Actions_Testing, string.Format(ApplicationStrings.ExternalServicesSettings_TestReportDownloadingCompletedSuccessfully, "BOLT"), "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(ApplicationStrings.Actions_Testing, ex.Message, "OK");
            }
            finally
            {
                ButtonBoltTest.IsInProgress = false;
            }
        }

        private async void ButtonUberTest_Clicked(object sender, EventArgs e)
        {
            if (!await ValidateValueRequired(UberCompanyId, "ID"))
                return;
            //id guid f2497213-8a49-4795-a2cc-ace1e9953879 36letters
            if (!await ValidateValueLength(UberCompanyId, "ID", 36))
                return;
            if (!await ValidateValueRequired(UberUserName, ApplicationStrings.LegalEntity_EmailAddress))
                return;
            if (!await ValidateValueRequired(UberPassword, ApplicationStrings.Actions_Password))
                return;
            ButtonUberTest.IsInProgress = true;
            try
            {
                var signature = GetSignature(Tenant);
                if (!signature.Equals(_tenantSignature))
                {
                    if (await _applicationController.TenantController.SaveTenant())
                        _tenantSignature = GetSignature(Tenant);
                }
                if (await _applicationController.TenantController.CheckDownloadingReport(ExternalReportType.Uber))
                {
                    ButtonUberTest.IsInProgress = false;
                    await DisplayAlert(ApplicationStrings.Actions_Testing, string.Format(ApplicationStrings.ExternalServicesSettings_TestReportDownloadingCompletedSuccessfully, "UBER"), "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(ApplicationStrings.Actions_Testing, ex.Message, "OK");
            }
            finally
            {
                ButtonUberTest.IsInProgress = false;
            }
        }

        private async void ButtonFreenowTest_Clicked(object sender, EventArgs e)
        {
            if (!await ValidateValueRequired(FreenowUserName, ApplicationStrings.LegalEntity_EmailAddress))
                return;
            if (!await ValidateValueRequired(FreenowPassword, ApplicationStrings.Actions_Password))
                return;
            ButtonFreenowTest.IsInProgress = true;
            try
            {
                var signature = GetSignature(Tenant);
                if (!signature.Equals(_tenantSignature))
                {
                    if (await _applicationController.TenantController.SaveTenant())
                        _tenantSignature = GetSignature(Tenant);
                }
                if (await _applicationController.TenantController.CheckDownloadingReport(ExternalReportType.Freenow))
                {
                    ButtonFreenowTest.IsInProgress = false;
                    await DisplayAlert(ApplicationStrings.Actions_Testing, string.Format(ApplicationStrings.ExternalServicesSettings_TestReportDownloadingCompletedSuccessfully, "FREENOW"), "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(ApplicationStrings.Actions_Testing, ex.Message, "OK");
            }
            finally
            {
                ButtonFreenowTest.IsInProgress = false;
            }
        }

        private async void ButtonSave_Clicked(object sender, EventArgs e)
        {
            var button = sender as ButtonControl;
            if (button == null)
                return;
            button.IsInProgress = true;
            try
            {
                await _applicationController.TenantController.SaveTenant();
                SetButtonsAccesability();
            }
            catch(Exception ex)
            {
                SetButtonsAccesability();
                await DisplayAlert(ApplicationStrings.Actions_SavingData, ex.Message, "OK");
            }
            finally
            {
                button.IsInProgress = false;
            }
        }

        private async Task<bool> ValidateValueRequired(Entry entry, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(entry.Text))
            {
                await DisplayAlert(ApplicationStrings.Actions_Testing, string.Format(ApplicationStrings.Actions_FieldIsRequired, fieldName), "OK");
                entry.Focus();
                return false;
            }
            return true;
        }

        private async Task<bool> ValidateValueLength(Entry entry, string fieldName, int expectedLength)
        {
            if (entry.Text.Length != expectedLength)
            {
                await DisplayAlert(ApplicationStrings.Actions_Testing, string.Format(ApplicationStrings.Actions_InvalidFieldValueLength, fieldName), "OK");
                entry.Focus();
                return false;
            }
            return true;
        }

        private static string GetSignature(Tenant tenant)
        {
            return $"{tenant.BoltCompanyId}|{tenant.BoltUserName}|{tenant.BoltPassword}|{tenant.UberCompanyId}|{tenant.UberUserName}|{tenant.UberPassword}|{tenant.FreenowUserName}|{tenant.FreenowPassword}";
        }
    }
}