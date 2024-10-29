using DriverSettlements.App.Controllers;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class TenantSettlementItemPage : ContentPage, IQueryAttributable
    {
        private readonly ApplicationController _applicationController;
        public List<Driver> Drivers { get; private set; }
        public List<SettlementType> SettlementTypes { get; private set; }
        public SettlementItem SettlementItem { get; private set; }

        private Mode _mode;

        public TenantSettlementItemPage()
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
            SettlementTypes = (await _applicationController.TenantController.SettlementTypesController.GetObjects())
                .Where(p => p.Enabled).OrderBy(p => p.TypeName).ToList();
            OnPropertyChanged("SettlementTypes");
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                var mode = query["Mode"] as Mode?;
                if (!mode.HasValue)
                    await Shell.Current.GoToAsync("..");
                _mode = mode.Value;
                SettlementItem = query["SettlementItem"] as SettlementItem;
                if (SettlementItem == null)
                    await Shell.Current.GoToAsync("..");
                SetDriverNameAndSettlementTypeAndRadioButtons();
                OnPropertyChanged("SettlementItem");
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private void SetDriverNameAndSettlementTypeAndRadioButtons()
        {
            if (SettlementItem != null && SettlementItem.Driver != null)
                DriverName.SelectedSuggestion = SettlementItem.Driver;
            if (SettlementItem != null && SettlementItem.SettlementType != null)
                SettlementTypeName.SelectedSuggestion = SettlementItem.SettlementType;
            PeriodicType0.IsChecked = SettlementItem == null || !SettlementItem.Periodic;
            PeriodicType1.IsChecked = SettlementItem.Periodic;
        }

        private void SetPeriodicTypeColors()
        {
            ((View)(PeriodicType0.Parent)).BackgroundColor = TenantTariffPlanAssignmentsPage.PeriodicTypeToColor(PeriodicType.None);
            ((View)(PeriodicType1.Parent)).BackgroundColor = TenantTariffPlanAssignmentsPage.PeriodicTypeToColor(PeriodicType.TariffPlanWithoutDetails);
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (SettlementItem.Driver==null)
                {
                    await DisplayAlert(Title.Text, "Pole 'Nazwa kierowcy' jest wymagane", "OK");
                    return;
                }
                if (SettlementItem.SettlementType == null)
                {
                    await DisplayAlert(Title.Text, "Pole 'Rodzaj rozliczenia' jest wymagane", "OK");
                    return;
                }
                SettlementItem.Periodic = !PeriodicType0.IsChecked;
                var driver = SettlementItem.Driver;
                SettlementItem.Driver = null;
                var settlementType = SettlementItem.SettlementType;
                SettlementItem.SettlementType = null;
                if (_mode == Mode.AddSettlementItem)
                    await _applicationController.TenantController.SettlementItemsController.AddObject(SettlementItem);
                else if (_mode == Mode.EditSettlementItem)
                    await _applicationController.TenantController.SettlementItemsController.UpdateObject(SettlementItem);
                SettlementItem.Driver = driver;
                SettlementItem.SettlementType = settlementType;
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        public enum Mode
        {
            Unknown = 0,
            AddSettlementItem = 1,
            EditSettlementItem = 2
        }

        private async void Drivers_TextChanged(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangedEventArgs e)
        {
            if (e.Reason != zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangeReason.UserInput)
                return;
            var autocompleteEntry = sender as zoft.MauiExtensions.Controls.AutoCompleteEntry;
            if (autocompleteEntry == null)
                return;
            SettlementItem.DriverId = 0;
            SettlementItem.Driver = null;
            var drivers = (await _applicationController.TenantController.DriversController.GetObjects())
                .Where(d => d.Confirmed && !d.Deleted);
            if (string.IsNullOrWhiteSpace(autocompleteEntry.Text))
                Drivers = drivers.OrderBy(d=>d.CompanyName).ToList();
            else
                Drivers = drivers.Where(d => d.CompanyName.StartsWith(autocompleteEntry.Text, StringComparison.InvariantCultureIgnoreCase)
                    || d.CompanyName.Contains($" {autocompleteEntry.Text}", StringComparison.InvariantCultureIgnoreCase)).OrderBy(d=>d.CompanyName).ToList();
            OnPropertyChanged("Drivers");
        }

        private async void SettlementTypes_TextChanged(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangedEventArgs e)
        {
            if (e.Reason != zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangeReason.UserInput)
                return;
            var autocompleteEntry = sender as zoft.MauiExtensions.Controls.AutoCompleteEntry;
            if (autocompleteEntry == null)
                return;
            SettlementItem.SettlementTypeId = 0;
            SettlementItem.SettlementType = null;
            var settlementTypes = (await _applicationController.TenantController.SettlementTypesController.GetObjects())
                .Where(d => d.Enabled);
            if (string.IsNullOrWhiteSpace(autocompleteEntry.Text))
                SettlementTypes = settlementTypes.OrderBy(d => d.TypeName).ToList();
            else
                SettlementTypes = settlementTypes.Where(d => d.TypeName.StartsWith(autocompleteEntry.Text, StringComparison.InvariantCultureIgnoreCase)
                    || d.TypeName.Contains($" {autocompleteEntry.Text}", StringComparison.InvariantCultureIgnoreCase)).OrderBy(d => d.TypeName).ToList();
            OnPropertyChanged("TariffPlans");
        }

        private void Drivers_SuggestionChosen(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntrySuggestionChosenEventArgs e)
        {
            var driver = e.SelectedItem as Driver;
            if (driver == null)
            {
                SettlementItem.DriverId = 0;
                SettlementItem.Driver = null;
                return;
            };
            SettlementItem.DriverId= driver.Id;
            SettlementItem.Driver = driver;
            Helpers.Keyboard.HideKeyboard();
        }

        private void SettlementTypes_SuggestionChosen(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntrySuggestionChosenEventArgs e)
        {
            var settlementType = e.SelectedItem as SettlementType;
            if (settlementType == null)
            {
                SettlementItem.SettlementTypeId = 0;
                SettlementItem.SettlementType = null;
                return; 
            }
            SettlementItem.SettlementTypeId = settlementType.Id;
            SettlementItem.SettlementType = settlementType;
            Helpers.Keyboard.HideKeyboard();
        }

        private async void SettlementTypes_Clicked(object sender, EventArgs e)
        {
            try
            {
                var buttonControl = sender as CustomControls.ButtonControl;
                if (buttonControl == null)
                    return;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "SettlementItem" , SettlementItem }
                };
                await Shell.Current.GoToAsync(nameof(TenantSettlementTypesPage), navigationParameter);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private void WeeklyRate_Focused(object sender, FocusEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                WeeklyRate.CursorPosition = 0;
                WeeklyRate.SelectionLength = WeeklyRate.Text == null ? 0 : WeeklyRate.Text.Length;
            });
        }
    }
}