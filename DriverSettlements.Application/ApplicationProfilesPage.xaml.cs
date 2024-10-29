using DriverSettlements.App.Controllers;
using DriverSettlements.App.Controllers.Application;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace DriverSettlements.App
{
    public partial class ApplicationProfilesPage : ContentPage
    {
        private readonly ApplicationController _applicationController;
        private readonly ApplicationProfilesListController _applicationProfilesListController;
        public ApplicationProfilesPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            _applicationProfilesListController = _applicationController.ApplicationProfilesListController;
            Appearing += ProfilesPage_Appearing;
        }

        private async void ProfilesPage_Appearing(object sender, EventArgs e)
        {
            try
            {
                _applicationProfilesListController.EnsureListViewIsRegistered(profilesView);
                if (!_applicationProfilesListController.DataLoaded)
                {
                    if (await _applicationProfilesListController.LoadData())
                    {
                        _applicationProfilesListController.RefreshListView();
                        _applicationProfilesListController.SelectObject(_applicationController.ApplicationProfile);
                    }
                }
                else
                {
                    _applicationProfilesListController.RefreshListView();
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ShellContent_Tenants, ex.Message, "OK");
            }
        }

        private async void profilesView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (profilesView.SelectedItem == null)
                return;
            await GoToProfileDetaisPageInEditMode(e.SelectedItem as ApplicationProfile);
        }

        private void profilesView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            profilesView.SelectedItem = null;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var button = sender as ButtonControl;
            if (button == null) return;
            button.IsInProgress = true;
            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Mode", ApplicationProfileDetailsPage.Mode.AddNew }
                };
                await Shell.Current.GoToAsync(nameof(ApplicationProfileDetailsPage), navigationParameter);
            }
            finally
            {
                button.IsInProgress = false;
            }
        }

        private async void EditMenuItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
                return;
            await GoToProfileDetaisPageInEditMode(menuItem.CommandParameter as ApplicationProfile);
        }

        private async Task GoToProfileDetaisPageInEditMode(ApplicationProfile profile)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "Mode", ApplicationProfileDetailsPage.Mode.Edit },
                { "Profile", profile }
            };
            await Shell.Current.GoToAsync(nameof(ApplicationProfileDetailsPage), navigationParameter);
            return;
        }

        private async void DeleteMenuItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
                return;
            var profile = menuItem.CommandParameter as ApplicationProfile;
            if (profile == null) return;
            try
            {
                if (await DisplayAlert(ApplicationStrings.ApplicationProfiles_DeletingProfile, string.Format(ApplicationStrings.ApplicationProfiles_DeletingProfileQuestion, profile.Name), ApplicationStrings.Actions_Yes, ApplicationStrings.Actions_No))
                {
                    if (await (_applicationProfilesListController.DeleteObject(menuItem.CommandParameter as ApplicationProfile)))
                        _applicationProfilesListController.RefreshListView();
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ApplicationProfiles_DeletingProfile, ex.Message, "OK");
            }
        }
    }
}