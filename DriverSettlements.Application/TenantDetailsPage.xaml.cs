using DriverSettlements.App.Controllers;
using DriverSettlements.App.Controllers.MasterAdmin;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class TenantDetailsPage : ContentPage, IQueryAttributable
    {
        public LegalEntity Tenant { get; private set; }
        private ApplicationController _applicationController;
        private TenantsListController _tenantsListController;
        private Mode? _mode;
        private string _tenantSignature;

        public TenantDetailsPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            _tenantsListController = _applicationController.MasterAdminController.TenantsListController;
            BindingContext = this;
            Appearing += TenantDetailsPage_Appearing;
            Disappearing += TenantDetailsPage_Disappearing;   
        }

        public enum Mode
        {
            Edit,
            AddNew
        }

        private void TenantDetailsPage_Appearing(object sender, EventArgs e)
        {
            if (_mode == Mode.AddNew)
            {
                FriendlyName.Focus();
            }
        }

        private async void TenantDetailsPage_Disappearing(object sender, EventArgs e)
        {
            try
            {
                switch (_mode)
                {
                    case Mode.Edit:
                        if (string.IsNullOrWhiteSpace(_tenantSignature) || _tenantSignature.Equals(GetSignature(Tenant)))
                            return;
                        //update action
                        if (await _tenantsListController.UpdateObject(Tenant))
                            _tenantsListController.RefreshListView();
                        break;
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ExceptionMessageTitle, ex.Message, "OK");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _mode = query["Mode"] as Mode?;
            switch(_mode)
            {
                case Mode.Edit:
                    Tenant = query["Tenant"] as LegalEntity;
                    _tenantSignature = GetSignature(Tenant);
                    OnPropertyChanged("Tenant");
                    break;
                case Mode.AddNew:
                    Tenant = new LegalEntity();
                    break;
            }
            SetButtonsVisibility();
        }

        private void SetButtonsVisibility()
        {
            SendInvitationKeyButton.IsVisible = _mode==Mode.Edit && string.IsNullOrWhiteSpace(Tenant.InvitationKey);
            CancelButton.IsVisible = _mode == Mode.AddNew;
            SaveButton.IsVisible = _mode == Mode.AddNew;
        }

        private static string GetSignature(LegalEntity tenant)
        {
            return $"{tenant.FriendlyName}|{tenant.CompanyName}|{tenant.EmailAddress}|{tenant.ApplicationFeeBaseValue}|{tenant.ApplicationFeeChargeDriver}|{tenant.Blocked}|{tenant.Deleted}";
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            CancelButton.IsInProgress = true;
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                CancelButton.IsInProgress= false;   
            }
        }

        private async void SendInvitationKeyButton_Clicked(object sender, EventArgs e)
        {
            SendInvitationKeyButton.IsInProgress = true;
            try
            {
                var invitationKey = await _tenantsListController.SentInvitationKey(Tenant);
                InvitationKey.Text = invitationKey;
                SetButtonsVisibility();
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.InvitationKey_SendingInvitationKey, ex.Message, "OK");
            }
            finally
            {
                SendInvitationKeyButton.IsInProgress = false;
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Tenant.FriendlyName))
            {
                await DisplayAlert(ApplicationStrings.TenantDetailsPage_AddingTenant , string.Format(ApplicationStrings.Actions_FieldIsRequired,ApplicationStrings.LegalEntity_FriendlyName),"OK");
                FriendlyName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Tenant.CompanyName))
            {
                await DisplayAlert(ApplicationStrings.TenantDetailsPage_AddingTenant, string.Format(ApplicationStrings.Actions_FieldIsRequired, ApplicationStrings.LegalEntity_CompanyName), "OK");
                CompanyName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Tenant.EmailAddress))
            {
                await DisplayAlert(ApplicationStrings.TenantDetailsPage_AddingTenant, string.Format(ApplicationStrings.Actions_FieldIsRequired, ApplicationStrings.LegalEntity_EmailAddress), "OK");
                EmailAddress.Focus();
                return;
            }
            SaveButton.IsInProgress = true;
            try
            {
                if (await _tenantsListController.AddObject(Tenant))
                {
                    await Shell.Current.GoToAsync("..");
                    _tenantsListController.RefreshListView();
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.TenantDetailsPage_AddingTenant, ex.Message, "OK");
            }
            finally
            {
                SaveButton.IsInProgress = false;
            }
        }
    }
}