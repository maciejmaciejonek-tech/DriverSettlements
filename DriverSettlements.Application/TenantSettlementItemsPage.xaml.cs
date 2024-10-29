using CommunityToolkit.Maui.Storage;
using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Models;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;
using System.Text;

namespace DriverSettlements.App
{
    public partial class TenantSettlementItemsPage : ContentPage
    {
        public TenantSettlementItemsWeek TenantSettlementItemsWeek { get; private set; }
        private readonly ApplicationController _applicationController;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Dictionary<SettlementItem, List<View>> _settlementItemViewElements;

        public TenantSettlementItemsPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Appearing += TenantSettlementItemsPage_Appearing;
            BindingContext = this;
            UpdateButtonsAndTitleGrid();
            RenderTenantSettlementItemsWeek();
        }

        private async void TenantSettlementItemsPage_Appearing(object sender, EventArgs e)
        {
            await SetTenantSettlementItemsWeek(_applicationController.TenantController.WeekController.DeltaWeek);
            UpdateButtonsAndTitleGrid();
            RenderTenantSettlementItemsWeek();
            TryToMakePeriodicAction(true);
        }

        private async void PrevButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                await SetTenantSettlementItemsWeek(_applicationController.TenantController.WeekController.PreviousDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantSettlementItemsWeek();
                TryToMakePeriodicAction(true);
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            if (!_applicationController.TenantController.WeekController.CanGoToNextWeek)
                return;
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                await SetTenantSettlementItemsWeek(_applicationController.TenantController.WeekController.NextDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantSettlementItemsWeek();
                TryToMakePeriodicAction(true);
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async Task<bool> SetTenantSettlementItemsWeek(int deltaWeek)
        {
            try
            {
                TenantSettlementItemsWeek = await GetTenantSettlementItemsWeek(deltaWeek);
                OnPropertyChanged("TenantSettlementItemsWeek");
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ShellContent_ExtrasAndDeducts, ex.Message, "OK");
                return false;
            }
        }

        private void RenderTenantSettlementItemsWeek()
        {
            while (MasterView.Children.Count > 4)
                MasterView.Children.RemoveAt(4);
            while (MasterView.RowDefinitions.Count > 1)
                MasterView.RowDefinitions.RemoveAt(1);
            MasterView.IsVisible = TenantSettlementItemsWeek != null && TenantSettlementItemsWeek.SettlementItems != null && TenantSettlementItemsWeek.SettlementItems.Count > 0;
            _settlementItemViewElements = new Dictionary<SettlementItem, List<View>>();
            if (MasterView.IsVisible)
            {
                var lp = 0;
                foreach (var settlementItem in TenantSettlementItemsWeek.SettlementItems)
                {
                    if (!string.IsNullOrWhiteSpace(SearchBar.Text)
                        && !settlementItem.Driver.CompanyName.StartsWith(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase)
                        && !settlementItem.Driver.CompanyName.Contains($" {SearchBar.Text}", StringComparison.InvariantCultureIgnoreCase)
                        && !settlementItem.SettlementType.TypeName.StartsWith(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase)
                        && !settlementItem.SettlementType.TypeName.Contains($" {SearchBar.Text}", StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    lp++;
                    //MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(30)));
                    MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                    var lpLabel = new Label() { Text = (lp).ToString(), BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(lpLabel, 0, lp);
                    var driverNameLabel = new Label() { Text = settlementItem.Driver.CompanyName, BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(driverNameLabel, 1, lp);
                    var settlementTypeStackLayout = CreateSettlementTypeLayout(settlementItem);
                    MasterView.Add(settlementTypeStackLayout, 2, lp);
                    var valueLabel = new Label() { Text = settlementItem.Value.ToString("#0.00"), BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(valueLabel, 3, lp);
                    _settlementItemViewElements .Add(settlementItem, new List<View>() { lpLabel, driverNameLabel, settlementTypeStackLayout, valueLabel });
                    var tapGestureRecognizer = new TapGestureRecognizer() { CommandParameter = settlementItem };
                    tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
                    var contentView = new ContentView() { BackgroundColor = Colors.Transparent };
                    contentView.GestureRecognizers.Add(tapGestureRecognizer);
                    MasterView.SetColumnSpan(contentView, 4);
                    MasterView.SetRow(contentView, lp);
                    MasterView.SetColumn(contentView, 0);
                    MasterView.Add(contentView);
                }
                MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var settlementItem = e.Parameter as SettlementItem;
            if (settlementItem == null)
                return;
            var viewElements = _settlementItemViewElements[settlementItem];
            if (EditButton.CommandParameter != null && EditButton.CommandParameter.Equals(settlementItem))
            {
                EditButton.CommandParameter = null;
                EditButton.IsVisible = false;
                DeleteButton.CommandParameter = null;
                DeleteButton.IsVisible = false;
                foreach (var viewElement in viewElements)
                    viewElement.BackgroundColor = Colors.White;
            }
            else
            {
                foreach (var viewElementKvp in _settlementItemViewElements)
                {
                    if (viewElementKvp.Key.Equals(settlementItem))
                    {
                        if (TenantSettlementItemsWeek != null && TenantSettlementItemsWeek.BillingWeekStatus != BillingWeekStatus.Closed)
                        {
                            EditButton.IsVisible = true;
                            DeleteButton.IsVisible = true;
                        }
                        EditButton.CommandParameter = settlementItem;
                        DeleteButton.CommandParameter = settlementItem;
                        foreach (var view in viewElementKvp.Value)
                            view.BackgroundColor = Colors.OrangeRed;
                    }
                    else
                    {
                        foreach (var view in viewElementKvp.Value)
                            view.BackgroundColor = Colors.White;
                    }
                }
            }
        }

        private static StackLayout CreateSettlementTypeLayout(SettlementItem settlementItem)
        {
            var tariffPlanStackLayout = new StackLayout() { BackgroundColor = Colors.White };
            var tariffPlanGrid = new Grid() { Margin = 2, HorizontalOptions = LayoutOptions.Center, BackgroundColor = PeriodicToColor(settlementItem.Periodic)};
            tariffPlanGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(3)));
            tariffPlanGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Auto)));
            tariffPlanGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(3)));
            tariffPlanGrid.Add(new Label() { Text = settlementItem.SettlementType.TypeName, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Center, FontSize = 12 }, 1, 0);
            tariffPlanStackLayout.Add(tariffPlanGrid);
            return tariffPlanStackLayout;
        }

        private async Task<TenantSettlementItemsWeek> GetTenantSettlementItemsWeek(int deltaWeek)
        {
            var tenantSettlementItemsWeek = _applicationController.TenantController.WeekController.GetWeek<TenantSettlementItemsWeek>(deltaWeek);
            _applicationController.TenantController.SettlementItemsController.ForceDataRefresh = true;
            _applicationController.TenantController.SettlementItemsController.SetYearAndWeek(tenantSettlementItemsWeek.YearNumber, tenantSettlementItemsWeek.WeekNumber);
            tenantSettlementItemsWeek.SettlementItems = await _applicationController.TenantController.SettlementItemsController.GetObjects();
            tenantSettlementItemsWeek.BillingWeekStatus = _applicationController.TenantController.SettlementItemsController.BillingWeekStatus;
            return tenantSettlementItemsWeek;
        }
        private void UpdateButtonsAndTitleGrid()
        {
            TitleGrid.BackgroundColor = Helpers.ColorHelper.GetTitleGridBacgroundColor(TenantSettlementItemsWeek);
            EditButton.IsVisible = false;
            DeleteButton.IsVisible = false;
            NextButton.BackgroundColor = Application.Current.Resources.MergedDictionaries.First()["Primary"] as Color;
            if (_applicationController.TenantController.WeekController.DeltaWeek > 0)
            {
//                NextButton.IsEnabled = true;
                NextButton.BackgroundColor = Application.Current.Resources.MergedDictionaries.First()["Primary"] as Color;
            }
            else
            {
//                NextButton.IsEnabled = false;
                NextButton.BackgroundColor = Colors.Grey;
            }
            AddExtraOrDeductButton.IsVisible = TenantSettlementItemsWeek != null && TenantSettlementItemsWeek.BillingWeekStatus != BillingWeekStatus.Closed;
        }

        private async void TryToMakePeriodicAction(bool withQuestion = true)
        {
            if (TenantSettlementItemsWeek.SettlementItems != null && TenantSettlementItemsWeek.SettlementItems.Count != 0)
                return;
            if (!string.IsNullOrWhiteSpace(SearchBar.Text))
                return;
            if (!withQuestion || await DisplayAlert(Title.Text, "Czy chcesz powielić pozycje rozliczeń  z poprzedniego tygodnia?", ApplicationStrings.Actions_Yes, ApplicationStrings.Actions_No))
            {
                if (await _applicationController.TenantController.SettlementItemsController.CopySettlementItemsFromPreviousWeek())
                {
                    _applicationController.TenantController.SettlementItemsController.ForceDataRefresh = true;
                    await SetTenantSettlementItemsWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                    UpdateButtonsAndTitleGrid();
                    RenderTenantSettlementItemsWeek();
                }
            }
        }

        private async void AddExtraOrDeductButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var buttonControl = sender as CustomControls.ButtonControl;
                if (buttonControl == null)
                    return;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Mode", TenantSettlementItemPage.Mode.AddSettlementItem},
                    { "SettlementItem", new SettlementItem(){ 
                        YearNumber=TenantSettlementItemsWeek.YearNumber, WeekNumber=TenantSettlementItemsWeek.WeekNumber,
                        Periodic=true}}
                };
                await Shell.Current.GoToAsync(nameof(TenantSettlementItemPage), navigationParameter);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private async void EditButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            var settlementItem = buttonControl.CommandParameter as SettlementItem;
            if (settlementItem == null)
                return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "Mode", TenantSettlementItemPage.Mode.EditSettlementItem },
                { "SettlementItem" , settlementItem }
            };
            await Shell.Current.GoToAsync(nameof(TenantSettlementItemPage), navigationParameter);
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            var settlementItem = buttonControl.CommandParameter as SettlementItem;
            if (settlementItem == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                if (await DisplayAlert(Title.Text, "Czy napewno chcesz usunąć pozycję rozliczenia?", ApplicationStrings.Actions_Yes, ApplicationStrings.Actions_No))
                {
                    if (await (_applicationController.TenantController.SettlementItemsController.DeleteObject(settlementItem)))
                    {
                        _applicationController.TenantController.SettlementItemsController.ForceDataRefresh = true;
                        await SetTenantSettlementItemsWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                        UpdateButtonsAndTitleGrid();
                        RenderTenantSettlementItemsWeek();
                        TryToMakePeriodicAction(true);
                    }
                }
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0 && e.OldTextValue.Length > 1)
                Helpers.Keyboard.HideKeyboard();
            RenderTenantSettlementItemsWeek();
            UpdateButtonsAndTitleGrid();
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Helpers.Keyboard.HideKeyboard();
        }

        public static Color PeriodicToColor(bool periodic)
        {
            return periodic ? Colors.LightGreen : Colors.LightGray;
        }

    }
}