using DriverSettlements.App.Controllers;
using DriverSettlements.App.Models;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class TenantTariffPlansPage : ContentPage
    {
        private List<TariffPlan> TariffPlans;
        private readonly ApplicationController _applicationController;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _filterOnlyActive = true;
        private Dictionary<TariffPlan, List<Label>> _tariffPlansLabels;

        public TenantTariffPlansPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Appearing += TenantTariffPlansPage_Appearing;
            BindingContext = this;
            UpdateFilterButtons();
            RenderTariffPlans();
        }

        private async void TenantTariffPlansPage_Appearing(object sender, EventArgs e)
        {
            UpdateFilterButtons();
            await SetTariffPlan();
            RenderTariffPlans();
        }

        private async Task<bool> SetTariffPlan()
        {
            try
            {
                _applicationController.TenantController.TariffPlansController.IncludeDeleted = !_filterOnlyActive;
                _applicationController.TenantController.TariffPlansController.ForceDataRefresh = true;
                TariffPlans = await _applicationController.TenantController.TariffPlansController.GetObjects();
                OnPropertyChanged("TariffPlans");
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
                return false;
            }
        }

        private void RenderTariffPlans()
        {
            while (MasterView.Children.Count > 4)
                MasterView.Children.RemoveAt(4);
            while (MasterView.RowDefinitions.Count > 1)
                MasterView.RowDefinitions.RemoveAt(1);
            MasterView.IsVisible = TariffPlans != null && TariffPlans.Count > 0;
            if (TariffPlans!=null && TariffPlans.Count>0)
            {
                var lp = 0;
                _tariffPlansLabels = new Dictionary<TariffPlan, List<Label>>();
                foreach (var tariffPlan in TariffPlans)
                {
                    if (!string.IsNullOrWhiteSpace(SearchBar.Text) && !tariffPlan.Name.Contains(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    if (_filterOnlyActive && !tariffPlan.Enabled)
                        continue;
                    lp ++;
                    //MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(30)));
                    MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                    var lpLabel = new Label() { Text = (lp).ToString(), BackgroundColor = Colors.White, TextColor=Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 14 };
                    MasterView.Add(lpLabel, 0, lp);
                    var registrationNumberLabel = new Label() { Text = tariffPlan.Name , BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(registrationNumberLabel, 1, lp);
                    var weeklyRateLabel = new Label() { Text = tariffPlan.WeeklyRate.ToString("#0.00"), BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(weeklyRateLabel, 2, lp);
                    var statusLabel = new Label() { Text = CarEnabledToText(tariffPlan.Enabled), BackgroundColor = TariffPlanEnabledToColor(tariffPlan.Enabled), TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(statusLabel, 3, lp);

                    _tariffPlansLabels.Add(tariffPlan, new List<Label>() { lpLabel, registrationNumberLabel, weeklyRateLabel, statusLabel});
                    var tapGestureRecognizer = new TapGestureRecognizer() { CommandParameter = tariffPlan };
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
            var tariffPlan = e.Parameter as TariffPlan;
            if (tariffPlan == null)
                return;
            var tariffPlanLabels = _tariffPlansLabels[tariffPlan];
            if (EditTariffPlanButton.CommandParameter!=null && EditTariffPlanButton.CommandParameter.Equals(tariffPlan))
            {
                EditTariffPlanButton.CommandParameter = null;
                EditTariffPlanButton.IsVisible = false;
                foreach(var label in tariffPlanLabels.SkipLast(1))
                    label.BackgroundColor = Colors.White;
            }
            else
            {
                foreach(var carLabelsKvp in _tariffPlansLabels)
                {
                    if (carLabelsKvp.Key.Equals(tariffPlan))
                    {
                        EditTariffPlanButton.CommandParameter = tariffPlan;
                        EditTariffPlanButton.IsVisible = true;
                        foreach (var label in carLabelsKvp.Value.SkipLast(1))
                            label.BackgroundColor = Colors.OrangeRed;
                    }
                    else
                    {
                        foreach (var label in carLabelsKvp.Value.SkipLast(1))
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

        private async void EditTariffPlan_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            var tariffPlan = buttonControl.CommandParameter as TariffPlan;
            if (tariffPlan == null)
                return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "Mode", TenantTariffPlanPage.Mode.EdittariffPlan},
                { "TariffPlan" , tariffPlan },
            };
            await Shell.Current.GoToAsync(nameof(TenantTariffPlanPage), navigationParameter);
        }

        private async void FilterAllTariffPlans_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            _filterOnlyActive = true;
            UpdateFilterButtons();
            await SetTariffPlan();
            RenderTariffPlans();
        }

        private async void FilterActiveTariffPlans_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            _filterOnlyActive = false;
            UpdateFilterButtons();
            await SetTariffPlan();
            RenderTariffPlans();
        }

        private async void AddTariffPlan_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "Mode", TenantTariffPlanPage.Mode.AddTariffPlan},
                { "TariffPlan" , new TariffPlan(){ Enabled = true}}
            };
            await Shell.Current.GoToAsync(nameof(TenantTariffPlanPage), navigationParameter);
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0 && e.OldTextValue.Length > 1)
                Helpers.Keyboard.HideKeyboard();
            UpdateFilterButtons();
            await SetTariffPlan();
            RenderTariffPlans();
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Helpers.Keyboard.HideKeyboard();
        }

        private void UpdateFilterButtons()
        {
            EditTariffPlanButton.CommandParameter = null;
            EditTariffPlanButton.IsVisible = false;
            FilterActiveTariffPlansButton.IsVisible = _filterOnlyActive;
            FilterAllTariffPlansButton.IsVisible = !_filterOnlyActive;
        }
    }
}