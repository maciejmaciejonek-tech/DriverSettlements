using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Resources.Localization;

namespace DriverSettlements.App
{
    public partial class InvitationPage : ContentPage
    {
        private readonly ApplicationController _applicationController;
        public InvitationPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Loaded += InvitationPage_Loaded;
        }

        private void InvitationPage_Loaded(object sender, EventArgs e)
        {
            InvitationCode.Focus();
        }

        private async void OnClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InvitationCode.Text)
                || !InvitationCode.Text.Length.Equals(8))
            {
                await DisplayAlert(ApplicationStrings.InvitationPage_ApplicationRegistering, ApplicationStrings.InvitationPage_InputValidInvitationCode, "OK");
                InvitationCode.Focus();
                return;
            }
            var button = sender as ButtonControl;
            if (button == null)
                return;
            button.IsInProgress = true;
            try
            {
                var invitationCode = InvitationCode.Text;
                InvitationCode.Text = "";
                if (await _applicationController.RegisterApplicationInstance(invitationCode))
                    _applicationController.RefreshUserInterface();
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.InvitationPage_ApplicationRegistering, ex.Message, "OK");
            }
            finally
            {
                button.IsInProgress = false;
            }
        }
    }
}