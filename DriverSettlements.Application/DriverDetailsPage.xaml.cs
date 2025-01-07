using DriverSettlements.App.Controllers;
using DriverSettlements.App.Controllers.Tenant;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class DriverDetailsPage : ContentPage, IQueryAttributable
    {
        public Driver Driver { get; private set; }
        private readonly ApplicationController _applicationController;
        private readonly DriversController _driversListController;
        private Mode? _mode;
        private string _driverSignature;

        public DriverDetailsPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            _driversListController = _applicationController.TenantController.DriversController;
            BindingContext = this;
            Appearing += DriverDetailsPage_Appearing;
        }

        public enum Mode
        {
            Edit,
            AddNew
        }

        private void DriverDetailsPage_Appearing(object sender, EventArgs e)
        {
            if (_mode == Mode.AddNew)
            {
                CompanyName.Focus();
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _mode = query["Mode"] as Mode?;
            switch(_mode)
            {
                case Mode.Edit:
                    Driver = query["Driver"] as Driver;
                    _driverSignature = GetSignature(Driver);
                    OnPropertyChanged("Driver");
                    break;
                case Mode.AddNew:
                    Driver = new Driver();
                    break;
            }
            await UpdateDepositSummary();
            SetButtonsVisibility();
        }

        private async Task<bool> UpdateDepositSummary()
        {
            if (Driver != null && Driver.Id>0)
                DepositSummary.Text = (await _applicationController.TenantController.GetDriverDepositValue(Driver.Id)).ToString("#0.00");
            else
                DepositSummary.Text = 0.ToString("#0.00");
            return true;
        }

        private void SetButtonsVisibility()
        {
            SendInvitationKeyButton.IsVisible = _mode==Mode.Edit && string.IsNullOrWhiteSpace(Driver.InvitationKey);
        }

        private static string GetSignature(Driver driver)
        {
            return $"{driver.FriendlyName}|{driver.CompanyName}|{driver.EmailAddress}|{driver.BoltName}|{driver.UberName}|{driver.FreenowName}|{driver.Deposit}|{driver.PartnershipFee}|{driver.ApplicationFeeChargeDriver}|{driver.Confirmed}|{driver.Blocked}|{driver.Deleted}";
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                buttonControl.IsInProgress= false;   
            }
        }

        private async void SendInvitationKeyButton_Clicked(object sender, EventArgs e)
        {
            SendInvitationKeyButton.IsInProgress = true;
            try
            {
                var invitationKey = await _driversListController.SentInvitationKey(Driver);
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
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null)
                return;
            if (string.IsNullOrEmpty(Driver.CompanyName))
            {
                await DisplayAlert(ApplicationStrings.DriverDetailsPage_AddingDriver, string.Format(ApplicationStrings.Actions_FieldIsRequired, ApplicationStrings.DriverDetailsPage_DriverName), "OK");
                CompanyName.Focus();
                return;
            }
            buttonControl.IsInProgress = true;
            try
            {
                if (_mode == Mode.AddNew)
                {
                    if (string.IsNullOrWhiteSpace(PartnershipFee.Text))
                        Driver.PartnershipFee = null;
                    if (await _driversListController.AddObject(Driver))
                    {
                        //_applicationController.TenantController.WeeksController.ForceDataRefresh = true;
                        _driversListController.RefreshListView();
                        await Shell.Current.GoToAsync("..");
                    }
                }
                else if (_mode== Mode.Edit)
                {
                    if (string.IsNullOrWhiteSpace(PartnershipFee.Text))
                        Driver.PartnershipFee = null;
                    if (string.IsNullOrWhiteSpace(_driverSignature) || _driverSignature.Equals(GetSignature(Driver)))
                    {
                        await Shell.Current.GoToAsync("..");
                        return;
                    }
                    //update action
                    if (await _driversListController.UpdateObject(Driver))
                    {
                        //_applicationController.TenantController.WeeksController.ForceDataRefresh = true;
                        _driversListController.RefreshListView();
                        await Shell.Current.GoToAsync("..");
                    }
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.DriverDetailsPage_AddingDriver, ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private void Deposit_Focused(object sender, FocusEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                Deposit.CursorPosition = 0;
                Deposit.SelectionLength = Deposit.Text == null ? 0 : Deposit.Text.Length;
            });
        }

        private void PartnershipFee_Focused(object sender, FocusEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                PartnershipFee.CursorPosition = 0;
                PartnershipFee.SelectionLength = Deposit.Text == null ? 0 : PartnershipFee.Text.Length;
            });
        }
    }
}