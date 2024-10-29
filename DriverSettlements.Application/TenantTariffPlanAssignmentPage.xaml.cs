using DriverSettlements.App.Controllers;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class TenantTariffPlanAssignmentPage : ContentPage, IQueryAttributable
    {
        private readonly ApplicationController _applicationController;
        public List<Driver> Drivers { get; private set; }
        public List<TariffPlan> TariffPlans { get; private set; }
        public TariffPlanAssignment TariffPlanAssignment { get; private set; }

        private Mode _mode;

        public TenantTariffPlanAssignmentPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            this.Appearing += TenantTariffPlanAssignmentPage_Appearing;
            BindingContext = this;
            SetPeriodicTypeColors();
        }

        private async void TenantTariffPlanAssignmentPage_Appearing(object sender, EventArgs e)
        {
            Drivers = (await _applicationController.TenantController.DriversController.GetObjects())
                .Where(d => d.Confirmed && !d.Deleted).OrderBy(d=>d.CompanyName).ToList();
            OnPropertyChanged("Drivers");
            TariffPlans = (await _applicationController.TenantController.TariffPlansController.GetObjects())
                .Where(p => p.Enabled).OrderBy(p => p.Name).ToList();
            OnPropertyChanged("TariffPlans");
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                var mode = query["Mode"] as Mode?;
                if (!mode.HasValue)
                    await Shell.Current.GoToAsync("..");
                _mode = mode.Value;
                TariffPlanAssignment = query["TariffPlanAssignment"] as TariffPlanAssignment;
                if (TariffPlanAssignment == null)
                    await Shell.Current.GoToAsync("..");
                SetDriverNameAndTarfiffNameAndRadioButtons();                
                OnPropertyChanged("TariffPlanAssignment");
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private void SetDriverNameAndTarfiffNameAndRadioButtons()
        {
            if (TariffPlanAssignment != null && TariffPlanAssignment.Driver != null)
                DriverName.SelectedSuggestion = TariffPlanAssignment.Driver;

            if (TariffPlanAssignment != null && TariffPlanAssignment.TariffPlan != null)
                TariffPlanName.SelectedSuggestion = TariffPlanAssignment.TariffPlan;
            PeriodicType0.IsChecked = TariffPlanAssignment != null && TariffPlanAssignment.PeriodicType == PeriodicType.None;
            PeriodicType1.IsChecked = TariffPlanAssignment != null && TariffPlanAssignment.PeriodicType == PeriodicType.TariffPlanWithoutDetails;
            PeriodicType2.IsChecked = TariffPlanAssignment != null && TariffPlanAssignment.PeriodicType == PeriodicType.TariffPlanWithDetails;
        }

        private void SetPeriodicTypeColors()
        {
            ((View)(PeriodicType0.Parent)).BackgroundColor = TenantTariffPlanAssignmentsPage.PeriodicTypeToColor(PeriodicType.None);
            ((View)(PeriodicType1.Parent)).BackgroundColor = TenantTariffPlanAssignmentsPage.PeriodicTypeToColor(PeriodicType.TariffPlanWithoutDetails);
            ((View)(PeriodicType2.Parent)).BackgroundColor = TenantTariffPlanAssignmentsPage.PeriodicTypeToColor(PeriodicType.TariffPlanWithDetails);
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (TariffPlanAssignment.Driver==null)
                {
                    await DisplayAlert(Title.Text, "Pole 'Nazwa kierowcy' jest wymagane", "OK");
                    return;
                }
                if (TariffPlanAssignment.TariffPlan == null)
                {
                    await DisplayAlert(Title.Text, "Pole 'Nazwa planu taryfowego' jest wymagane", "OK");
                    return;
                }
                if (PeriodicType0.IsChecked)
                    TariffPlanAssignment.PeriodicType = PeriodicType.None;
                if (PeriodicType1.IsChecked)
                    TariffPlanAssignment.PeriodicType = PeriodicType.TariffPlanWithoutDetails;
                if (PeriodicType2.IsChecked)
                    TariffPlanAssignment.PeriodicType = PeriodicType.TariffPlanWithDetails;
                var driver = TariffPlanAssignment.Driver;
                TariffPlanAssignment.Driver = null;
                var tariffPlan = TariffPlanAssignment.TariffPlan;
                TariffPlanAssignment.TariffPlan = null;
                if (_mode == Mode.AddTariffPlanAssignment)
                    await _applicationController.TenantController.TariffPlanAssignmentsController.AddObject(TariffPlanAssignment);
                else if (_mode == Mode.EdittariffPlanAssignment)
                    await _applicationController.TenantController.TariffPlanAssignmentsController.UpdateObject(TariffPlanAssignment);
                TariffPlanAssignment.Driver = driver;
                TariffPlanAssignment.TariffPlan = tariffPlan;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
            await Shell.Current.GoToAsync("..");
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        public enum Mode
        {
            Unknown = 0,
            AddTariffPlanAssignment = 1,
            EdittariffPlanAssignment = 2
        }

        private async void Drivers_TextChanged(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangedEventArgs e)
        {
            if (e.Reason != zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangeReason.UserInput)
                return;
            var autocompleteEntry = sender as zoft.MauiExtensions.Controls.AutoCompleteEntry;
            if (autocompleteEntry == null)
                return;
            TariffPlanAssignment.DriverId = 0;
            TariffPlanAssignment.Driver = null;
            var drivers = (await _applicationController.TenantController.DriversController.GetObjects())
                .Where(d => d.Confirmed && !d.Deleted);
            if (string.IsNullOrWhiteSpace(autocompleteEntry.Text))
                Drivers = drivers.OrderBy(d=>d.CompanyName).ToList();
            else
                Drivers = drivers.Where(d => d.CompanyName.StartsWith(autocompleteEntry.Text, StringComparison.InvariantCultureIgnoreCase)
                    || d.CompanyName.Contains($" {autocompleteEntry.Text}", StringComparison.InvariantCultureIgnoreCase)).OrderBy(d=>d.CompanyName).ToList();
            OnPropertyChanged("Drivers");
        }

        private async void TariffPlans_TextChanged(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangedEventArgs e)
        {
            if (e.Reason != zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangeReason.UserInput)
                return;
            var autocompleteEntry = sender as zoft.MauiExtensions.Controls.AutoCompleteEntry;
            if (autocompleteEntry == null)
                return;
            TariffPlanAssignment.TariffPlanId = 0;
            TariffPlanAssignment.TariffPlan = null;
            var tariffPlans = (await _applicationController.TenantController.TariffPlansController.GetObjects())
                .Where(d => d.Enabled);
            if (string.IsNullOrWhiteSpace(autocompleteEntry.Text))
                TariffPlans = tariffPlans.OrderBy(d => d.Name).ToList();
            else
                TariffPlans = tariffPlans.Where(d => d.Name.StartsWith(autocompleteEntry.Text, StringComparison.InvariantCultureIgnoreCase)
                    || d.Name.Contains($" {autocompleteEntry.Text}", StringComparison.InvariantCultureIgnoreCase)).OrderBy(d => d.Name).ToList();
            OnPropertyChanged("TariffPlans");
        }

        private void Drivers_SuggestionChosen(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntrySuggestionChosenEventArgs e)
        {
            var driver = e.SelectedItem as Driver;
            if (driver == null)
            {
                TariffPlanAssignment.DriverId = 0;
                TariffPlanAssignment.Driver = null;
                return;
            }; 
            TariffPlanAssignment.DriverId= driver.Id;
            TariffPlanAssignment.Driver = driver;
            Helpers.Keyboard.HideKeyboard();
        }

        private void TariffPlans_SuggestionChosen(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntrySuggestionChosenEventArgs e)
        {
            var tariffPlan = e.SelectedItem as TariffPlan;
            if (tariffPlan == null)
            {
                TariffPlanAssignment.TariffPlanId = 0;
                TariffPlanAssignment.TariffPlan = null;
                TariffPlanAssignment.Cost = 0;
                return; 
            }
            TariffPlanAssignment.TariffPlanId = tariffPlan.Id;
            TariffPlanAssignment.TariffPlan = tariffPlan;
            TariffPlanAssignment.Cost = tariffPlan.WeeklyRate;
            Helpers.Keyboard.HideKeyboard();
        }
    }
}