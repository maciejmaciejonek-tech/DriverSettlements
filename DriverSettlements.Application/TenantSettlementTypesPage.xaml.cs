using DriverSettlements.App.Controllers;
using DriverSettlements.App.Models;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class TenantSettlementTypesPage : ContentPage, IQueryAttributable
    {
        private List<SettlementType> SettlementTypes;
        private readonly ApplicationController _applicationController;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _filterOnlyActive = true;
        private Dictionary<SettlementType, List<Label>> _settlementTypesLabels;
        public SettlementItem SettlementItem;
        private SettlementType _settlementTypeFromSettlementItem;

        public TenantSettlementTypesPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Appearing += TenantSettlementTypesPage_Appearing;
            BindingContext = this;
            UpdateFilterButtons();
            RenderSettlementTypes();
        }

        private async void TenantSettlementTypesPage_Appearing(object sender, EventArgs e)
        {
            UpdateFilterButtons();
            await SetSettlementTypes();
            RenderSettlementTypes();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {

                SettlementItem = query["SettlementItem"] as SettlementItem;
                if (SettlementItem != null)
                    _settlementTypeFromSettlementItem = SettlementItem.SettlementType;
                OnPropertyChanged("SettlementItem");
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private async Task<bool> SetSettlementTypes()
        {
            try
            {
                _applicationController.TenantController.TariffPlansController.IncludeDeleted = !_filterOnlyActive;
                _applicationController.TenantController.TariffPlansController.ForceDataRefresh = true;
                SettlementTypes = await _applicationController.TenantController.SettlementTypesController.GetObjects();
                OnPropertyChanged("SettlementTypes");
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
                return false;
            }
        }

        private void RenderSettlementTypes()
        {
            while (MasterView.Children.Count > 4)
                MasterView.Children.RemoveAt(4);
            while (MasterView.RowDefinitions.Count > 1)
                MasterView.RowDefinitions.RemoveAt(1);
            MasterView.IsVisible = SettlementTypes != null && SettlementTypes.Count > 0;
            if (MasterView.IsVisible)
            {
                var lp = 0;
                _settlementTypesLabels = new Dictionary<SettlementType, List<Label>>();
                foreach (var settlementType in SettlementTypes)
                {
                    if (!string.IsNullOrWhiteSpace(SearchBar.Text) && !settlementType.TypeName.Contains(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    if (_filterOnlyActive && !settlementType.Enabled)
                        continue;
                    lp ++;
                    //MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(30)));
                    MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                    var lpLabel = new Label() { Text = (lp).ToString(), BackgroundColor = Colors.White, TextColor=Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 14 };
                    MasterView.Add(lpLabel, 0, lp);
                    var registrationNumberLabel = new Label() { Text = settlementType.TypeName , BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(registrationNumberLabel, 1, lp);
                    var weeklyRateLabel = new Label() { Text = TenantSettlementTypePage.GetSettlementTypeString(settlementType), BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(weeklyRateLabel, 2, lp);
                    var statusLabel = new Label() { Text = CarEnabledToText(settlementType.Enabled), BackgroundColor = TariffPlanEnabledToColor(settlementType.Enabled), TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(statusLabel, 3, lp);
                    _settlementTypesLabels.Add(settlementType, new List<Label>() { lpLabel, registrationNumberLabel, weeklyRateLabel, statusLabel});
                    var tapGestureRecognizer = new TapGestureRecognizer() { CommandParameter = settlementType };
                    tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
                    var contentView = new ContentView() { BackgroundColor = Colors.Transparent };
                    contentView.GestureRecognizers.Add(tapGestureRecognizer);
                    MasterView.SetColumnSpan(contentView, 5);
                    MasterView.SetRow(contentView, lp);
                    MasterView.SetColumn(contentView, 0);
                    MasterView.Add(contentView);
                }
                MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var settlementType = e.Parameter as SettlementType;
            if (settlementType == null)
                return;
            var settlementTypeLabels = _settlementTypesLabels[settlementType];
            if (EditSettlementTypeButton.CommandParameter!=null && EditSettlementTypeButton.CommandParameter.Equals(settlementType))
            {
                if (SettlementItem != null)
                {
                    SettlementItem.SettlementType = _settlementTypeFromSettlementItem;
                    if (SettlementItem.SettlementType!=null)
                        SettlementItem.SettlementTypeId = SettlementItem.SettlementType.Id;
                }
                EditSettlementTypeButton.CommandParameter = null;
                EditSettlementTypeButton.IsVisible = false;
                foreach (var label in settlementTypeLabels.SkipLast(1))
                    label.BackgroundColor = Colors.White;
            }
            else
            {
                foreach(var settlementTypeLabelsKvp in _settlementTypesLabels)
                {
                    if (settlementTypeLabelsKvp.Key.Equals(settlementType))
                    {
                        if (SettlementItem != null)
                        {
                            SettlementItem.SettlementType = settlementType;
                            if (SettlementItem.SettlementType != null)
                                SettlementItem.SettlementTypeId = SettlementItem.SettlementType.Id;
                        }
                        EditSettlementTypeButton.CommandParameter = settlementType;
                        EditSettlementTypeButton.IsVisible = true;
                        foreach (var label in settlementTypeLabelsKvp.Value.SkipLast(1))
                            label.BackgroundColor = Colors.OrangeRed;
                    }
                    else
                    {
                        foreach (var label in settlementTypeLabelsKvp.Value.SkipLast(1))
                            label.BackgroundColor = Colors.White;
                    }
                }

            }
        }

        private static string CarEnabledToText(bool enabled)
        {
            return enabled ? "Aktywny" : "Nieaktywny";
        }

        private static Color TariffPlanEnabledToColor(bool enabled)
        {
            return enabled ? Colors.LightGreen : Colors.LightGrey;
        }

        private async void EditSettlementType_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            var settlementType = buttonControl.CommandParameter as SettlementType;
            if (settlementType == null)
                return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "Mode", TenantSettlementTypePage.Mode.EditSettlementType},
                { "SettlementType" , settlementType },
            };
            await Shell.Current.GoToAsync(nameof(TenantSettlementTypePage), navigationParameter);
        }

        private async void SelectSettlementType_Clicked(object sender, EventArgs e)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "SettlementTypeId" , 1 }
            };
            await Shell.Current.GoToAsync("..",navigationParameter);
        }

        private async void FilterAllSettlementTypes_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            _filterOnlyActive = true;
            UpdateFilterButtons();
            await SetSettlementTypes();
            RenderSettlementTypes();
        }

        private async void FilterActiveSettlementTypes_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            _filterOnlyActive = false;
            UpdateFilterButtons();
            await SetSettlementTypes();
            RenderSettlementTypes();
        }

        private async void AddSettlementType_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "Mode", TenantSettlementTypePage.Mode.AddSettlementType},
                { "SettlementType" , new SettlementType(){ Enabled=true, IncludeInReport=true, Debit=true, Factor=1, TenantId=-1}}
            };
            await Shell.Current.GoToAsync(nameof(TenantSettlementTypePage), navigationParameter);
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0 && e.OldTextValue.Length > 1)
                Helpers.Keyboard.HideKeyboard();
            UpdateFilterButtons();
            await SetSettlementTypes();
            RenderSettlementTypes();
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Helpers.Keyboard.HideKeyboard();
        }

        private void UpdateFilterButtons()
        {
            if (SettlementItem != null)
            {
                SettlementItem.SettlementType = _settlementTypeFromSettlementItem;
                if (SettlementItem.SettlementType != null)
                    SettlementItem.SettlementTypeId = SettlementItem.SettlementType.Id;
            }
            EditSettlementTypeButton.CommandParameter = null;
            EditSettlementTypeButton.IsVisible = false;
            FilterActiveSettlementTypesButton.IsVisible = _filterOnlyActive;
            FilterAllSettlementTypesButton.IsVisible = !_filterOnlyActive;
        }
    }
}