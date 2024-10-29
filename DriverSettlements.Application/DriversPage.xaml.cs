using DriverSettlements.App.Controllers;
using DriverSettlements.App.Controllers.Tenant;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Helpers;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;
using System.Collections.ObjectModel;

namespace DriverSettlements.App
{
    public partial class DriversPage : ContentPage
    {
        private readonly ApplicationController _applicationController;
        private readonly DriversController _driversListController;
        public DriversPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            _driversListController = _applicationController.TenantController.DriversController;
            Appearing += DriversPage_Appearing;
        }

        private async void DriversPage_Appearing(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchBar.Text))
                SearchBar.Text = string.Empty;
            try
            {
                _driversListController.EnsureListViewIsRegistered(driversView);
                if (!_driversListController.DataLoaded)
                {
                    if (await _driversListController.LoadData())
                    {
                        _driversListController.RefreshListView(FilterDrivers);
                        EditButton.IsVisible = false;
                        EditButton.CommandParameter = null;
                        DeleteButton.IsVisible = false;
                        DeleteButton.CommandParameter = null;
                    }
                }
                else
                {
                    _driversListController.RefreshListView(FilterDrivers);
                    EditButton.IsVisible = false;
                    EditButton.CommandParameter = null;
                    DeleteButton.IsVisible = false;
                    DeleteButton.CommandParameter = null;
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ShellContent_Tenants, ex.Message, "OK");
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0 && e.OldTextValue.Length > 1)
                Helpers.Keyboard.HideKeyboard();
            _driversListController.RefreshListView(FilterDrivers);
            EditButton.IsVisible = false;
            EditButton.CommandParameter = null;
            DeleteButton.IsVisible = false;
            DeleteButton.CommandParameter = null;
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Helpers.Keyboard.HideKeyboard();
        }

        /*
                private async void driversView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
                {
                    if (driversView.SelectedItem == null)
                        return;
                    await GoToDriverDetaisPageInEditMode(e.SelectedItem as Driver);
                }
        */
        private void driversView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedItem = e.Item as ISelectable;
            if (selectedItem == null)
                return;
            var items = driversView.ItemsSource as ObservableCollection<Driver>;
            if (items == null)
                return;
            foreach (ISelectable item in items)
            {
                if (item.Equals(selectedItem))
                {
                    selectedItem.Selected = !selectedItem.Selected;
                    if (selectedItem.Selected)
                    {
                        EditButton.CommandParameter = (selectedItem as SelectableDriverWrapper)?.Driver;
                        EditButton.IsVisible = true;
                        DeleteButton.CommandParameter = (selectedItem as SelectableDriverWrapper)?.Driver;
                        DeleteButton.IsVisible = true;
                    }
                    else
                    {
                        driversView.SelectedItem = null;
                        EditButton.CommandParameter = null;
                        EditButton.IsVisible = false;
                        DeleteButton.CommandParameter = null;
                        DeleteButton.IsVisible = false;
                    }
                }
                else
                {
                    item.Selected = false;
                }
            };
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
                    { "Mode", DriverDetailsPage.Mode.AddNew }
                };
                await Shell.Current.GoToAsync(nameof(DriverDetailsPage), navigationParameter);
            }
            finally
            {
                button.IsInProgress = false;
            }
        }

        private async void EditButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null)
                return;
            await GoToDriverDetaisPageInEditMode(buttonControl.CommandParameter as Driver);
        }

        private async Task GoToDriverDetaisPageInEditMode(Driver driver)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "Mode", DriverDetailsPage.Mode.Edit },
                { "Driver", driver }
            };
            await Shell.Current.GoToAsync(nameof(DriverDetailsPage), navigationParameter);
            return;
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null)
                return;
            var tenant = buttonControl.CommandParameter as LegalEntity;
            if (tenant == null) return;
            try
            {
                if (await DisplayAlert(ApplicationStrings.DriversPage_DeletingDriver, string.Format(ApplicationStrings.DriversPage_DeletingDriverQuestion, tenant.CompanyName), ApplicationStrings.Actions_Yes, ApplicationStrings.Actions_No))
                {
                    if (await (_driversListController.DeleteObject(buttonControl.CommandParameter as Driver)))
                    {
                        _driversListController.RefreshListView(FilterDrivers);
                        EditButton.IsVisible = false;
                        EditButton.CommandParameter = null;
                        DeleteButton.IsVisible = false;
                        DeleteButton.CommandParameter = null;
                    }
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.DriversPage_DeletingDriver, ex.Message, "OK");
            }
        }

        private async void MapWithDriverMenuItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
                return;
            await GoToDriverDetaisPageInEditMode(menuItem.CommandParameter as Driver);
        }

        private void ToggleButton_Clicked(object sender, EventArgs e)
        {
            _driversListController.RefreshListView(FilterDrivers);
            EditButton.IsVisible = false;
            EditButton.CommandParameter = null;
            DeleteButton.IsVisible = false;
            DeleteButton.CommandParameter = null;
        }

        private List<Driver> FilterDrivers(List<Driver> drivers)
        {
            if (!_driversListController.IncludeDeleted)
                drivers = drivers.Where(d => !d.Deleted).ToList();
            if (!string.IsNullOrWhiteSpace(SearchBar.Text))
                drivers = drivers.Where(d=> d.CompanyName.StartsWith(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase)
                                    || d.CompanyName.Contains($" {SearchBar.Text}", StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (NumberOfExternalServices1.Toggled || NumberOfExternalServices2.Toggled || NumberOfExternalServices3.Toggled)
                drivers = drivers.Where(d => NumberOfExternalServices1.Toggled && d.GetNumberOfExternalServices().Equals(1)
                    || NumberOfExternalServices2.Toggled && d.GetNumberOfExternalServices().Equals(2)
                    || NumberOfExternalServices3.Toggled && d.GetNumberOfExternalServices().Equals(3)).ToList();
            //            return drivers.OrderBy(d => d.CompanyName).ToList();       
            return drivers.Select(d => new SelectableDriverWrapper(d)).Cast<Driver>().OrderBy(d => d.CompanyName).ToList();
        }
    }
}