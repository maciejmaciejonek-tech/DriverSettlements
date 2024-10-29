using DriverSettlements.App.Controllers;
using DriverSettlements.App.Controllers.Tenant;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Helpers;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Helpers;
using DriverSettlements.Common.Models;
using System.Collections.ObjectModel;

namespace DriverSettlements.App
{
    public partial class DriversConfirmationAndMappingsPage : ContentPage, IQueryAttributable
    {
        public Driver Driver { get; private set; }
        private List<Driver> _drivers;
        private int _driverIdx = 0;
        private readonly ApplicationController _applicationController;
        private readonly DriversController _driversListController;
        private Color? _originalBackgroundColor;

        public DriversConfirmationAndMappingsPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            _driversListController = _applicationController.TenantController.DriversController;
            Appearing += DriverDetailsPage_Appearing;
            DriverNameInfo.BindingContext = this;
            BoltInfo.BindingContext = this;
            UberInfo.BindingContext = this;
            FreenowInfo.BindingContext = this;
        }

        private async void DriverDetailsPage_Appearing(object sender, EventArgs e)
        {            
            if (_originalBackgroundColor == null)
                _originalBackgroundColor = PrevButton.BackgroundColor;
            if (Driver == null)
                return;
            UpdatePrevNextButtonsAndInfo();
            if (!string.IsNullOrWhiteSpace(SearchBar.Text))
                SearchBar.Text = string.Empty;
            await Task.Delay(200);
            SearchBar.Focus();
            try
            {
                _driversListController.EnsureListViewIsRegistered(driversView);
                if (!_driversListController.DataLoaded)
                {
                    if (await _driversListController.LoadData())
                        _driversListController.RefreshListView(FilterDrivers);
                }
                else
                    _driversListController.RefreshListView(FilterDrivers);
            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.Message, "OK");
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var driverIds = query["DriverIds"] as HashSet<int>;
            if (driverIds == null) return;
            _drivers =  (await _applicationController.TenantController.DriversController.GetObjects()).Where(d => !d.Confirmed && driverIds.Contains(d.Id)).OrderBy(d=>d.CompanyName).ToList();
            if (_drivers == null)
                return;
            if (_drivers.Count == 0)
                Driver = null;
            else
                Driver = _drivers[_driverIdx];
            OnPropertyChanged("Driver");
            DriverDetailsPage_Appearing(null, null);
        }

        private async void PrevButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            buttonControl.IsInProgress = true;
            try
            {
                _driverIdx--;
                if (_driverIdx < 0)
                    _driverIdx = 0;
                var newDriver = _drivers[_driverIdx];
                if (!Driver.Equals(newDriver))
                {
                    Driver = newDriver;
                    UpdatePrevNextButtonsAndInfo();
                    OnPropertyChanged("Driver");
                    SearchBar.Text = String.Empty;
                    await Task.Delay(200);
                    SearchBar.Focus();
                }
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert(ApplicationStrings.TenantWeeksPage_RevisingDriversList, ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            _driversListController.RefreshListView(FilterDrivers);
            driversView.SelectedItem = null;
            UpdatePrevNextButtonsAndInfo();
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            buttonControl.IsInProgress = true;
            try
            {
                _driverIdx++;
                if (_driverIdx >= _drivers.Count)
                    _driverIdx = _drivers.Count - 1;
                var newDriver = _drivers[_driverIdx];
                if (!Driver.Equals(newDriver))
                {
                    Driver = newDriver;
                    UpdatePrevNextButtonsAndInfo();
                    OnPropertyChanged("Driver");
                    SearchBar.Text = String.Empty;
                    _driversListController.RefreshListView(FilterDrivers);
                    await Task.Delay(200);
                    SearchBar.Focus();
                }
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert(ApplicationStrings.TenantWeeksPage_RevisingDriversList, ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private void UpdatePrevNextButtonsAndInfo()
        {
            if (_drivers == null) return;
            if (_driverIdx == 0)
                PrevButton.BackgroundColor = Colors.LightGrey;
            else
                PrevButton.BackgroundColor = _originalBackgroundColor;
            if (_driverIdx+1==_drivers.Count)
                NextButton.BackgroundColor = Colors.LightGrey;
            else
                NextButton.BackgroundColor = _originalBackgroundColor;
            NumberInfo.Text = string.Format(ApplicationStrings.DriversConfirmationAndMappingsPage_NumberInfo, _driverIdx + 1, _drivers.Count);
            var selectedItem = driversView.SelectedItem as ISelectable;
             if (selectedItem==null)
            {
                ConfirmButton.IsEnabled = true;
                ConfirmButton.BackgroundColor = _originalBackgroundColor;
                MapWithDriverButton.IsEnabled = false;
                MapWithDriverButton.BackgroundColor = Colors.LightGray;
            }
            else
            {
                ConfirmButton.IsEnabled = false;
                ConfirmButton.BackgroundColor = Colors.LightGray;
                MapWithDriverButton.IsEnabled = true;
                MapWithDriverButton.BackgroundColor = _originalBackgroundColor;
            }
        }

        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            buttonControl.IsInProgress = true;
            try
            {
                Driver.Confirmed = true;
                if (await _applicationController.TenantController.DriversController.UpdateObject(Driver))
                {
//                    _applicationController.TenantController.WeeksController.ForceDataRefresh = true;
                    _drivers.Remove(Driver);
                    if (_drivers.Count < 1)
                    {
                        await Shell.Current.GoToAsync("..");
                        return;
                    }
                    if (_driverIdx == _drivers.Count)
                        _driverIdx--;
                    if (_driverIdx < 0)
                        _driverIdx = 0;
                    Driver = _drivers[_driverIdx];
                    UpdatePrevNextButtonsAndInfo();
                    OnPropertyChanged("Driver");
                    SearchBar.Text = String.Empty;
                    await Task.Delay(200);
                    SearchBar.Focus();
                }
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert(ApplicationStrings.TenantWeeksPage_RevisingDriversList, ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async void MapWithDriverButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            buttonControl.IsInProgress = true;
            try
            {
                var selectedItem = driversView.SelectedItem as SelectableDriverWrapper;
                if (selectedItem == null) 
                    return;
                var drivers = await _applicationController.TenantController.DriversController.GetObjects();
                var selectedDriver = drivers.FirstOrDefault(d => d.Id.Equals(selectedItem.Id));
                if (selectedDriver == null) 
                    return;
                if (!string.IsNullOrEmpty(Driver.BoltName))
                    selectedDriver.BoltName = Strings.StringsToString(selectedDriver.BoltName, Driver.BoltName);
                if (!string.IsNullOrEmpty(Driver.UberName))
                    selectedDriver.UberName = Strings.StringsToString(selectedDriver.UberName, Driver.UberName);
                if (!string.IsNullOrEmpty(Driver.FreenowName))
                    selectedDriver.FreenowName = Strings.StringsToString(selectedDriver.FreenowName, Driver.FreenowName);
                var confirmedSelectedDriver = false;
                if (!selectedDriver.Confirmed && selectedDriver.GetNumberOfExternalServices() == 3)
                {
                    selectedDriver.Confirmed = true;
                    confirmedSelectedDriver = true;
                }
                if (await _applicationController.TenantController.DriversController.UpdateObject(selectedDriver))
                {
//                    _applicationController.TenantController.WeeksController.ForceDataRefresh = true;
                    _drivers.Remove(Driver);
                    if (await _applicationController.TenantController.DriversController.DeleteObject(Driver))
                    {
                        if (_drivers.Count < 1)
                        {
                            await Shell.Current.GoToAsync("..");
                            return;
                        }
                        if (_driverIdx == _drivers.Count)
                            _driverIdx--;
                        if (_driverIdx < 0)
                            _driverIdx = 0;
                        if (confirmedSelectedDriver)
                        {
                            var idx = _drivers.FindIndex(d => d.Id.Equals(selectedDriver.Id));
                            if (idx >= 0)
                            {
                                _drivers.RemoveAt(idx);
                                if (_drivers.Count < 1)
                                {
                                    await Shell.Current.GoToAsync("..");
                                    return;
                                }
                                if (idx < _driverIdx)
                                    _driverIdx--;
                                if (_driverIdx == _drivers.Count)
                                    _driverIdx--;
                                if (_driverIdx < 0)
                                    _driverIdx = 0;
                            }
                        }
                        Driver = _drivers[_driverIdx];
                        UpdatePrevNextButtonsAndInfo();
                        OnPropertyChanged("Driver");
                        SearchBar.Text = String.Empty;
                        await Task.Delay(200);
                        SearchBar.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert(ApplicationStrings.TenantWeeksPage_RevisingDriversList, ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private List<Driver> FilterDrivers(List<Driver> drivers)
        {
            if (!_driversListController.IncludeDeleted)
                drivers = drivers.Where(d => !d.Deleted).ToList();
            if (!string.IsNullOrWhiteSpace(SearchBar.Text))
                drivers = drivers.Where(d => d.CompanyName.StartsWith(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase)
                                    || d.CompanyName.Contains($" {SearchBar.Text}", StringComparison.InvariantCultureIgnoreCase)).ToList();
            return drivers.Where(d=>!d.Id.Equals(Driver?.Id)).Select(d=>new SelectableDriverWrapper(d)).Cast<Driver>().OrderBy(d => d.CompanyName).ToList();
//            return drivers.Where(d=>!d.Equals(Driver)).OrderBy(d => d.CompanyName).ToList();
        }

        private void driversView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedItem = e.Item as ISelectable;
            if (selectedItem == null)
                return;
            var items = driversView.ItemsSource as ObservableCollection<Driver>;
            if (items == null)
                return;
            foreach(ISelectable item in items) 
            {
                if (item.Equals(selectedItem))
                    continue;
                item.Selected = false;
            };
            selectedItem.Selected = !selectedItem.Selected;
            if (!selectedItem.Selected)
                driversView.SelectedItem = null;
            UpdatePrevNextButtonsAndInfo();
        }
    }
}