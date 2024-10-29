using DriverSettlements.App.Controllers;
using DriverSettlements.App.Controllers.Application;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DriverSettlements.App
{
    public partial class ApplicationProfileDetailsPage : ContentPage, IQueryAttributable
    {
        public ApplicationProfile Profile { get; private set; }
        private readonly ApplicationController _applicationController;
        private readonly ApplicationProfilesListController _applicationProfilesListController;
        private Mode? _mode;
        private string _profileSignature;

        public ApplicationProfileDetailsPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            _applicationProfilesListController = _applicationController.ApplicationProfilesListController;
            BindingContext = this;
            Appearing += ApplicationProfileDetailsPage_Appearing;
            Disappearing += ApplicationProfileDetailsPage_Disappearing;            
        }

        public enum Mode
        {
            Edit,
            AddNew
        }

        private void ApplicationProfileDetailsPage_Appearing(object sender, EventArgs e)
        {
            if (_mode == Mode.AddNew)
            {
                ProfileName.Focus();
            }
        }

        private async void ApplicationProfileDetailsPage_Disappearing(object sender, EventArgs e)
        {
            try
            {
                switch (_mode)
                {
                    case Mode.Edit:
                        if (string.IsNullOrWhiteSpace(_profileSignature) || _profileSignature.Equals(GetSignature(Profile)))
                            return;
                        //update action
                        if (await _applicationProfilesListController.UpdateObject(Profile))
                            _applicationProfilesListController.RefreshListView();
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
                    Profile = query["Profile"] as ApplicationProfile;
                    _profileSignature = GetSignature(Profile);
                    OnPropertyChanged("Profile");
                    break;
                case Mode.AddNew:
                    Profile = new ApplicationProfile();
                    break;
            }
            SetButtonsVisibility();
        }

        private void SetButtonsVisibility()
        {
            CancelButton.IsVisible = _mode == Mode.AddNew;
            SaveButton.IsVisible = _mode == Mode.AddNew;
            SwitchToProfile.IsVisible = _mode == Mode.Edit && !Profile.Equals(_applicationController.ApplicationProfile);
        }

        private static string GetSignature(ApplicationProfile profile)
        {
            return $"{profile.Name}|{profile.Default}";
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

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Profile.Name))
            {
                await DisplayAlert(ApplicationStrings.ApplicationProfiles_AddingProfile , string.Format(ApplicationStrings.Actions_FieldIsRequired,ApplicationStrings.LegalEntity_FriendlyName),"OK");
                ProfileName.Focus();
                return;
            }
            SaveButton.IsInProgress = true;
            try
            {
                if (await _applicationProfilesListController.AddObject(Profile))
                {
                    await Shell.Current.GoToAsync("..");
                    _applicationProfilesListController.RefreshListView();
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ApplicationProfiles_AddingProfile, ex.Message, "OK");
            }
            finally
            {
                SaveButton.IsInProgress = false;
            }
        }

        private async void SwitchToProfile_Clicked(object sender, EventArgs e)
        {
            SwitchToProfile.IsInProgress = true;
            try
            {
                if (!await _applicationController.SwitchToProfile(Profile))
                { 
                    _applicationController.ExceptionMessage = string.Empty;
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ApplicationProfiles_MarkingProfileAsDefault, ex.Message, "OK");
            }
            finally
            {
                SwitchToProfile.IsInProgress = false;
            }
        }
    }
}