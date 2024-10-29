using DriverSettlements.App.Controllers;
using DriverSettlements.App.Models;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class DriverFuelBillsPage : ContentPage
    {
        public DriverFuelBillsWeek DriverFuelBillsWeek { get; private set; }
        private readonly ApplicationController _applicationController;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Dictionary<FuelBill, List<Label>> _fuelBillsLabels = new Dictionary<FuelBill, List<Label>>();
        public DriverFuelBillsPage()
        {
            InitializeComponent();
            FuelBillsView.Children.Clear();
            FuelBillsView.RowDefinitions.Clear();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Appearing += DriverFuelBillsPage_Appearing;
            BindingContext = this;
            UpdateButtonsAndTitleGrid();
            RenderDriverFuelBillsWeek();
        }

        private async void DriverFuelBillsPage_Appearing(object sender, EventArgs e)
        {
            await SetDriverFuelBillsWeek(_applicationController.DriverController.WeekController.DeltaWeek);
            UpdateButtonsAndTitleGrid();
            RenderDriverFuelBillsWeek();
        }

        private async void PrevButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                await SetDriverFuelBillsWeek(_applicationController.DriverController.WeekController.PreviousDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderDriverFuelBillsWeek();
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            if (!_applicationController.DriverController.WeekController.CanGoToNextWeek)
                return;
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                await SetDriverFuelBillsWeek(_applicationController.DriverController.WeekController.NextDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderDriverFuelBillsWeek();
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async Task<bool> SetDriverFuelBillsWeek(int deltaWeek)
        {
            try
            {
                DriverFuelBillsWeek = await GetDriverFuelBillsWeek(deltaWeek);
                OnPropertyChanged("DriverFuelBillsWeek");
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ShellContent_FuelBills, ex.Message, "OK");
                return false;
            }
        }

        private void RenderDriverFuelBillsWeek()
        {
            FuelBillsView.Children.Clear();
            FuelBillsView.RowDefinitions.Clear();
            if (DriverFuelBillsWeek!=null && DriverFuelBillsWeek.FuelBills!=null && DriverFuelBillsWeek.FuelBills.Count>0)
            {
                FuelBillsView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                FuelBillsView.Add(new Label() { Text = "Lp", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 0, 0);
                FuelBillsView.Add(new Label() { Text = "Data", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 1, 0);
                FuelBillsView.Add(new Label() { Text = "Numer rachunku", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 2, 0);
                FuelBillsView.Add(new Label() { Text = "Kwota", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 3, 0);
                FuelBillsView.Add(new Label() { Text = "Status", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 4, 0);
                var lp = 0;
                _fuelBillsLabels.Clear();
                foreach(var fuelBill in DriverFuelBillsWeek.FuelBills)
                { 
                    lp++;
                    FuelBillsView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                    var label1 = new Label() { Text = lp.ToString(), HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 };
                    FuelBillsView.Add(label1, 0, lp);
                    var label2 = new Label() { Text = fuelBill.Date.ToString("yyyy-MM-dd"), HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 };
                    FuelBillsView.Add(label2, 1, lp);
                    var label3 = new Label() { Text = fuelBill.ReferenceNumber, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 10 };
                    FuelBillsView.Add(label3, 2, lp);
                    var label4 = new Label() { Text = $"{fuelBill.Cost.ToString("#00.00")} zł", HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 };
                    FuelBillsView.Add(label4, 3, lp);                
                    FuelBillsView.Add(new Label() { Text = Helpers.FuelBill.StatusToText(fuelBill.Status), HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Helpers. FuelBill.StatusToColor(fuelBill.Status), TextColor = Colors.Black, FontSize = 12 }, 4, lp);
                    _fuelBillsLabels.Add(fuelBill, new List<Label> { label1, label2, label3, label4 });
                    var tapGestureRecognizer = new TapGestureRecognizer() { CommandParameter=fuelBill };
                    tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
                    var contentView = new ContentView() { BackgroundColor = Colors.Transparent};
                    contentView.GestureRecognizers.Add(tapGestureRecognizer);
                    FuelBillsView.SetColumnSpan(contentView, 5);
                    FuelBillsView.SetRow(contentView, lp);
                    FuelBillsView.SetColumn(contentView, 0);
                    FuelBillsView.Add(contentView);
                }
                FuelBillsView.RowDefinitions.Add(new RowDefinition(new GridLength(1,GridUnitType.Auto)));
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var fuelBill = e.Parameter as FuelBill;
            if (fuelBill == null)
                return;
            var fuelBillLabels = _fuelBillsLabels[fuelBill];
            if (fuelBillLabels.First().BackgroundColor==Colors.OrangeRed)
            {
                EditFuelBill.IsVisible = false;
                EditFuelBill.CommandParameter = null;
                foreach (var label in fuelBillLabels)
                {
                    label.BackgroundColor = Colors.White;
                    label.Handler.UpdateValue("Background");
                }
            }
            else
            {
                foreach(var fuelBillLabelsKvp in _fuelBillsLabels)
                {
                    if (fuelBillLabelsKvp.Value.Equals(fuelBillLabels))
                    {
                        if (DriverFuelBillsWeek != null && DriverFuelBillsWeek.BillingWeekStatus != BillingWeekStatus.Closed)
                            EditFuelBill.IsVisible = true;
                        EditFuelBill.CommandParameter = fuelBill;
                        foreach (var label in fuelBillLabels)
                        {
                            label.BackgroundColor = Colors.OrangeRed;
                            label.Handler.UpdateValue("Background");
                        }
                    }
                    else
                    {
                        foreach (var label in fuelBillLabelsKvp.Value)
                        {
                            label.BackgroundColor = Colors.White;
                            label.Handler.UpdateValue("Background");
                        }
                    }
                }
            }
        }

        private async void EditFuelBill_Clicked(object sender, EventArgs e)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                {"Mode", DriverFuelBillPage.Mode.Edit},
                {"FuelBill",EditFuelBill.CommandParameter as FuelBill}
            };
            await Shell.Current.GoToAsync(nameof(DriverFuelBillPage), navigationParameter);
        }

        private async void AddFuelBillButton_Clicked(object sender, EventArgs e)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                {"Mode", DriverFuelBillPage.Mode.AddNew},
                {"FuelBill",new FuelBill(){ YearNumber = DriverFuelBillsWeek.YearNumber, WeekNumber=DriverFuelBillsWeek.WeekNumber }}
            };
            await Shell.Current.GoToAsync(nameof(DriverFuelBillPage), navigationParameter);
        }


        private async Task<DriverFuelBillsWeek> GetDriverFuelBillsWeek(int deltaWeek)
        {
            var driverFuelBillsWeek = _applicationController.DriverController.WeekController.GetWeek<DriverFuelBillsWeek>(deltaWeek);
            _applicationController.DriverController.FuelBillsController.SetYearAndWeekNumbers(driverFuelBillsWeek.YearNumber, driverFuelBillsWeek.WeekNumber);
            driverFuelBillsWeek.FuelBills = await _applicationController.DriverController.FuelBillsController.GetObjects();
            driverFuelBillsWeek.BillingWeekStatus = _applicationController.DriverController.FuelBillsController.BillingWeekStatus;
            return driverFuelBillsWeek;
        }

        private void UpdateButtonsAndTitleGrid()
        {
            TitleGrid.BackgroundColor = Helpers.ColorHelper.GetTitleGridBacgroundColor(DriverFuelBillsWeek);
            NextButton.IsEnabled = true;
            NextButton.BackgroundColor = Application.Current.Resources.MergedDictionaries.First()["Primary"] as Color;
            if (_applicationController.DriverController.WeekController.DeltaWeek>0)
            {
//                NextButton.IsEnabled = true;
                NextButton.BackgroundColor = Application.Current.Resources.MergedDictionaries.First()["Primary"] as Color;
            }
            else
            {
//                NextButton.IsEnabled = false;
                NextButton.BackgroundColor = Colors.Grey;
            }
            EditFuelBill.IsVisible = false;
            AddFuelBillButton.IsVisible = DriverFuelBillsWeek != null && DriverFuelBillsWeek.BillingWeekStatus!=BillingWeekStatus.Closed;
        }
    }
}