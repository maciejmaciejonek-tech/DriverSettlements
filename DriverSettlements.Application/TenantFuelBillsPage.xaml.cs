using DriverSettlements.App.Controllers;
using DriverSettlements.App.Models;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class TenantFuelBillsPage : ContentPage
    {
        public TenantFuelBillsWeek TenantFuelBillsWeek { get; private set; }
        private readonly ApplicationController _applicationController;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Dictionary<DriverFuelBillsInfo, Grid> _grids = new Dictionary<DriverFuelBillsInfo, Grid>();
        private Dictionary<DriverFuelBillsInfo, List<Label>> _driverNameLabels = new Dictionary<DriverFuelBillsInfo, List<Label>>();
        private Dictionary<FuelBill, List<Label>> _fuelBillLabels = new Dictionary<FuelBill, List<Label>>();
        private bool _filterRequested = true;
        private bool _comeBackFromAcceptancePage = false;

        public TenantFuelBillsPage()
        {
            InitializeComponent();
            FuelBillsMasterView.Children.Clear();
            FuelBillsMasterView.RowDefinitions.Clear();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Appearing += TenantFuelBillsPage_Appearing;
            BindingContext = this;
            UpdateButtonsAndTitleGrid();
            RenderTenantFuelBillsWeek();
        }

        private async void TenantFuelBillsPage_Appearing(object sender, EventArgs e)
        {
            if (_comeBackFromAcceptancePage)
            {
                var driverFuelBillsInfo = VerifyFuelBillButton.CommandParameter as DriverFuelBillsInfo;
                var fuelBill = VerifyFuelBillButton.CommandParameter as FuelBill;
                var skipReloading = true;
                if (fuelBill!=null)
                {
                    var lastLabel = _fuelBillLabels[fuelBill].Last();
                    lastLabel.Text = Helpers.FuelBill.StatusToText(fuelBill.Status);
                    lastLabel.BackgroundColor = Helpers.FuelBill.StatusToColor(fuelBill.Status);
                    lastLabel.Handler.UpdateValue("Background");
                    var driverNameLabelsKvp = _driverNameLabels.FirstOrDefault(d => d.Key.DriverId.Equals(fuelBill.DriverId));
                    driverNameLabelsKvp.Value[1].Text = driverNameLabelsKvp.Key.FuelBils.Count(fb => fb.Status == FuelBillStatus.Requested).ToString();
                    driverNameLabelsKvp.Value[2].Text = driverNameLabelsKvp.Key.FuelBils.Count(fb => fb.Status == FuelBillStatus.Acccepted).ToString();
                    driverNameLabelsKvp.Value[3].Text = driverNameLabelsKvp.Key.FuelBils.Count(fb => fb.Status == FuelBillStatus.Rejected).ToString();
                    if (_filterRequested && driverNameLabelsKvp.Key.FuelBils.Count(fb => fb.Status == FuelBillStatus.Requested) < 1)
                    {
                        skipReloading = false;
                        _applicationController.TenantController.FuelBillsController.ForceDataRefresh = true;
                    }
                }
                else if (driverFuelBillsInfo!=null)
                {
                    var driverNameLabels = _driverNameLabels[driverFuelBillsInfo];
                    driverNameLabels[1].Text = driverFuelBillsInfo.FuelBils.Count(fb => fb.Status == FuelBillStatus.Requested).ToString();
                    driverNameLabels[2].Text = driverFuelBillsInfo.FuelBils.Count(fb => fb.Status == FuelBillStatus.Acccepted).ToString();
                    driverNameLabels[3].Text = driverFuelBillsInfo.FuelBils.Count(fb => fb.Status == FuelBillStatus.Rejected).ToString();
                    foreach(var tempFuelBill in driverFuelBillsInfo.FuelBils)
                    {
                        var lastLabel = _fuelBillLabels[tempFuelBill].Last();
                        lastLabel.Text = Helpers.FuelBill.StatusToText(tempFuelBill.Status);
                        lastLabel.BackgroundColor = Helpers.FuelBill.StatusToColor(tempFuelBill.Status);
                        lastLabel.Handler.UpdateValue("Background");
                    }
                    if (_filterRequested && driverFuelBillsInfo.FuelBils.Count(fb => fb.Status == FuelBillStatus.Requested) < 1)
                    {
                        skipReloading = false;
                        _applicationController.TenantController.FuelBillsController.ForceDataRefresh = true;
                    }
                }
                if (skipReloading)
                    return;
            }
            await SetTenantFuelBillsWeek(_applicationController.TenantController.WeekController.DeltaWeek);
            UpdateButtonsAndTitleGrid();
            RenderTenantFuelBillsWeek();
        }

        private async void PrevButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                await SetTenantFuelBillsWeek(_applicationController.TenantController.WeekController.PreviousDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantFuelBillsWeek();
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
                await SetTenantFuelBillsWeek(_applicationController.TenantController.WeekController.NextDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantFuelBillsWeek();
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async Task<bool> SetTenantFuelBillsWeek(int deltaWeek)
        {
            try
            {
                TenantFuelBillsWeek = await GetTenantFuelBillsWeek(deltaWeek);
                OnPropertyChanged("TenantFuelBillsWeek");
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
                return false;
            }
        }

        private void RenderTenantFuelBillsWeek()
        {
            _grids.Clear();
            _driverNameLabels.Clear();
            FuelBillsMasterView.Children.Clear();
            FuelBillsMasterView.RowDefinitions.Clear();
            if (TenantFuelBillsWeek == null)
                return;
            if (TenantFuelBillsWeek.DriverFuelBillsInfos != null && TenantFuelBillsWeek.DriverFuelBillsInfos.Count > 0  && (!_filterRequested || TenantFuelBillsWeek.DriverFuelBillsInfos.Exists(fbi=>fbi.Requested>0)))
            {
                FuelBillsMasterView.RowDefinitions.Add(new RowDefinition(new GridLength(28)));
                FuelBillsMasterView.Add(new Label() { Text = "Kierowca", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 14 }, 0, 0);
                var label = new Label() { Text = "Rachunki", FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 14 };
                FuelBillsMasterView.SetColumnSpan(label, 3);
                FuelBillsMasterView.SetRow(label, 0);
                FuelBillsMasterView.SetColumn(label, 1);
                FuelBillsMasterView.Add(label);
                var lp = -1;
                foreach (var driverFuelBillsInfo in TenantFuelBillsWeek.DriverFuelBillsInfos)
                {
                    if (!string.IsNullOrWhiteSpace(SearchBar.Text) && !driverFuelBillsInfo.DriverName.Contains(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    if (_filterRequested && driverFuelBillsInfo.Requested < 1)
                        continue;
                    lp +=2;
                    FuelBillsMasterView.RowDefinitions.Add(new RowDefinition(new GridLength(28)));
                    var driverNameLabel = new Label() { Text = driverFuelBillsInfo.DriverName, BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 14 };
                    FuelBillsMasterView.Add(driverNameLabel, 0, lp);
                    var requestedLabel = new Label() { Text = driverFuelBillsInfo.Requested.ToString(), BackgroundColor = Helpers.FuelBill.StatusToColor(FuelBillStatus.Requested), TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    FuelBillsMasterView.Add(requestedLabel, 1, lp);
                    var acceptedLabel = new Label() { Text = driverFuelBillsInfo.Accepted.ToString(), BackgroundColor = Helpers.FuelBill.StatusToColor(FuelBillStatus.Acccepted), TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    FuelBillsMasterView.Add(acceptedLabel, 2, lp);
                    var rejectedLabel = new Label() { Text = driverFuelBillsInfo.Rejected.ToString(), BackgroundColor = Helpers.FuelBill.StatusToColor(FuelBillStatus.Rejected), TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    FuelBillsMasterView.Add(rejectedLabel, 3, lp);
                    _driverNameLabels.Add(driverFuelBillsInfo,new List<Label>() { driverNameLabel, requestedLabel, acceptedLabel, rejectedLabel });
                    var tapGestureRecognizer = new TapGestureRecognizer() { CommandParameter = driverFuelBillsInfo };
                    tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
                    var contentView = new ContentView() { BackgroundColor = Colors.Transparent };
                    contentView.GestureRecognizers.Add(tapGestureRecognizer);
                    FuelBillsMasterView.SetColumnSpan(contentView, 5);
                    FuelBillsMasterView.SetRow(contentView, lp);
                    FuelBillsMasterView.SetColumn(contentView, 0);
                    FuelBillsMasterView.Add(contentView);
                    //details
                    var rowDefinition = new RowDefinition(new GridLength(1,GridUnitType.Auto));
                    FuelBillsMasterView.RowDefinitions.Add(rowDefinition);
                    var grid = new Grid();
                    FuelBillsMasterView.SetColumnSpan(grid, 5);
                    FuelBillsMasterView.SetRow(grid, lp + 1);
                    FuelBillsMasterView.SetColumn(grid, 0);
                    FuelBillsMasterView.Add(grid);
                    _grids.Add(driverFuelBillsInfo, grid);
                }
                FuelBillsMasterView.RowDefinitions.Add(new RowDefinition(new GridLength(1,GridUnitType.Auto)));
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var driverFuelBillsInfo = e.Parameter as DriverFuelBillsInfo;
            if (driverFuelBillsInfo == null)
                return;
            var grid = _grids[driverFuelBillsInfo];
            var driverNameLabels = _driverNameLabels[driverFuelBillsInfo];
            if (grid.Children.Count>0)
            {
                driverNameLabels.First().BackgroundColor = Colors.White;
                driverNameLabels.First().Handler.UpdateValue("Background");
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
                grid.ColumnDefinitions.Clear();
                VerifyFuelBillButton.IsVisible = false;
                VerifyFuelBillButton.CommandParameter = null;
                return;
            }
            foreach(var driverNameLabelsKvp in _driverNameLabels)
            {
                if (driverNameLabelsKvp.Value.Equals(driverNameLabels))
                {
                    driverNameLabels.First().BackgroundColor = Colors.OrangeRed;
                    driverNameLabels.First().Handler.UpdateValue("Background");
                    if (TenantFuelBillsWeek != null && TenantFuelBillsWeek.BillingWeekStatus != BillingWeekStatus.Closed)
                        VerifyFuelBillButton.IsVisible = true;
                     VerifyFuelBillButton.CommandParameter = driverFuelBillsInfo;
                }
                else
                {
                    driverNameLabelsKvp.Value.First().BackgroundColor = Colors.White;
                    driverNameLabelsKvp.Value.First().Handler.UpdateValue("Background");
                }
            }
            foreach (var gridKvp in _grids)
            {
                if (gridKvp.Value.Children.Count>0)
                {
                    gridKvp.Value.Children.Clear();
                    gridKvp.Value.RowDefinitions.Clear();
                    gridKvp.Value.ColumnDefinitions.Clear();
                }
            }
            grid.RowSpacing = 1;
            grid.ColumnSpacing = 1;
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(25)));
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(70)));
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1,GridUnitType.Star)));
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(60)));
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(86)));
            grid.HorizontalOptions = LayoutOptions.Fill;
            grid.BackgroundColor = Colors.Black;
            if (driverFuelBillsInfo.FuelBils == null)
            {
                _applicationController.TenantController.FuelBillsController.SetYearAndWeekAndDriver(TenantFuelBillsWeek.YearNumber, TenantFuelBillsWeek.WeekNumber, driverFuelBillsInfo.DriverId);
                driverFuelBillsInfo.FuelBils = await _applicationController.TenantController.FuelBillsController.GetObjects();
            }
            if (driverFuelBillsInfo.FuelBils != null && driverFuelBillsInfo.FuelBils.Count > 0)
            {
                grid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                grid.Add(new Label() { Text = "Lp", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 0, 0);
                grid.Add(new Label() { Text = "Data", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 1, 0);
                grid.Add(new Label() { Text = "Numer rachunku", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 2, 0);
                grid.Add(new Label() { Text = "Kwota", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 3, 0);
                grid.Add(new Label() { Text = "Status", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 4, 0);
                var lp = 0;
                _fuelBillLabels.Clear();
                foreach (var fuelBill in driverFuelBillsInfo.FuelBils)
                {
                    lp++;
                    grid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                    var label1 = new Label() { Text = lp.ToString(), HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 };
                    grid.Add(label1, 0, lp);
                    var label2 = new Label() { Text = fuelBill.Date.ToString("yyyy-MM-dd"), HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 };
                    grid.Add(label2, 1, lp);
                    var label3 = new Label() { Text = fuelBill.ReferenceNumber, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment=TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 10 };
                    grid.Add(label3, 2, lp);
                    var label4 = new Label() { Text = $"{fuelBill.Cost.ToString("#00.00")} zł", HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 };
                    grid.Add(label4, 3, lp);
                    var label5 = new Label() { Text = Helpers.FuelBill.StatusToText(fuelBill.Status), HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Helpers.FuelBill.StatusToColor(fuelBill.Status), TextColor = Colors.Black, FontSize = 12 };
                    grid.Add(label5 , 4, lp);
                    _fuelBillLabels.Add(fuelBill, new List<Label> { label1, label2, label3, label4, label5 });
                    var tapGestureRecognizer = new TapGestureRecognizer() { CommandParameter = fuelBill };
                    tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped2;
                    var contentView = new ContentView() { BackgroundColor = Colors.Transparent };
                    contentView.GestureRecognizers.Add(tapGestureRecognizer);
                    grid.SetColumnSpan(contentView, 5);
                    grid.SetRow(contentView, lp);
                    grid.SetColumn(contentView, 0);
                    grid.Add(contentView);
                }
            }
        }

        private void TapGestureRecognizer_Tapped2(object sender, TappedEventArgs e)
        {
            var fuelBill = e.Parameter as FuelBill;
            if (fuelBill == null)
                return;
            var driverFuelBillsInfo = VerifyFuelBillButton.CommandParameter as DriverFuelBillsInfo;
            if (driverFuelBillsInfo!=null)
            {
                var driverNameLabels = _driverNameLabels[driverFuelBillsInfo];
                driverNameLabels.First().BackgroundColor = Colors.White;
                driverNameLabels.First().Handler.UpdateValue("Background");
                VerifyFuelBillButton.IsVisible = false; //added line ...
                VerifyFuelBillButton.CommandParameter = null;
            }
            var fuelBillLabels = _fuelBillLabels[fuelBill];
            if (fuelBillLabels.First().BackgroundColor.Equals(Colors.OrangeRed))
            {
                VerifyFuelBillButton.IsVisible = false;
                VerifyFuelBillButton.CommandParameter = null;
                foreach (var label in fuelBillLabels.SkipLast(1))
                {
                    label.BackgroundColor = Colors.White;
                    label.Handler.UpdateValue("Background");
                }
            }
            else
            {
                if (TenantFuelBillsWeek != null && TenantFuelBillsWeek.BillingWeekStatus != BillingWeekStatus.Closed)
                    VerifyFuelBillButton.IsVisible = true;
                VerifyFuelBillButton.CommandParameter = fuelBill;
                foreach (var fuelBillLabelsKvp in _fuelBillLabels)
                {
                    if (fuelBillLabelsKvp.Value.Equals(fuelBillLabels))
                    {
                        foreach (var label in fuelBillLabelsKvp.Value.SkipLast(1))
                        {
                            label.BackgroundColor = Colors.OrangeRed;
                            label.Handler.UpdateValue("Background");
                        }
                    }
                    else if (fuelBillLabelsKvp.Value.First().BackgroundColor.Equals(Colors.OrangeRed))
                    {
                        foreach (var label in fuelBillLabelsKvp.Value.SkipLast(1))
                        {
                            label.BackgroundColor = Colors.White;
                            label.Handler.UpdateValue("Background");
                        }
                    }
                }
            }
        }

        private async void VerifyFuelBill_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            var navigationParameter = new Dictionary<string, object>
            {
                {"FilterRequested", _filterRequested},
                {"FuelBillsScope", VerifyFuelBillButton.CommandParameter}
            };
            await Shell.Current.GoToAsync(nameof(TenantFuelBillVerificationPage), navigationParameter);
            _comeBackFromAcceptancePage = true;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {            
            if (e.NewTextValue.Length==0 && e.OldTextValue.Length>1)
                Helpers.Keyboard.HideKeyboard();
            RenderTenantFuelBillsWeek();
            UpdateButtonsAndTitleGrid();            
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Helpers.Keyboard.HideKeyboard();
        }

        private void FilterAllFuelBills_Clicked(object sender, EventArgs e)
        {
            _filterRequested = !_filterRequested;
            RefreshFilterButtons();
            RenderTenantFuelBillsWeek();
        }

        private void FilterRequestedFuelBills_Clicked(object sender, EventArgs e)
        {
            _filterRequested = !_filterRequested;
            RefreshFilterButtons();
            RenderTenantFuelBillsWeek();
        }
        
        private async Task<TenantFuelBillsWeek> GetTenantFuelBillsWeek(int deltaWeek)
        {
            var tenantFuelBillsWeek = _applicationController.TenantController.WeekController.GetWeek<TenantFuelBillsWeek>(deltaWeek);
            _applicationController.TenantController.FuelBillsController.SetYearAndWeekAndDriver(tenantFuelBillsWeek.YearNumber, tenantFuelBillsWeek.WeekNumber);
            var response = await _applicationController.TenantController.FuelBillsController.GetDriverFuelBillsInfos();
            tenantFuelBillsWeek.DriverFuelBillsInfos = response.DriverFuelBillsInfos;
            tenantFuelBillsWeek.BillingWeekStatus = response.BillingWeekStatus;
            return tenantFuelBillsWeek;
        }

        private void UpdateButtonsAndTitleGrid()
        {
            TitleGrid.BackgroundColor = Helpers.ColorHelper.GetTitleGridBacgroundColor(TenantFuelBillsWeek);
            if (_applicationController.TenantController.WeekController.DeltaWeek>0)
            {
//                NextButton.IsEnabled = true;
                NextButton.BackgroundColor = Application.Current.Resources.MergedDictionaries.First()["Primary"] as Color;
            }
            else
            {
//                NextButton.IsEnabled = false;
                NextButton.BackgroundColor = Colors.Grey;
            }
            VerifyFuelBillButton.IsVisible = false;
            RefreshFilterButtons();
        }

        private void RefreshFilterButtons()
        {
            if (_filterRequested)
            {
                FilterRequestedFuelBillsButton.IsVisible = true;
                FilterAllFuelBillsButton.IsVisible = false;
            }
            else
            {
                FilterRequestedFuelBillsButton.IsVisible = false;
                FilterAllFuelBillsButton.IsVisible = true;
            }
        }
    }
}