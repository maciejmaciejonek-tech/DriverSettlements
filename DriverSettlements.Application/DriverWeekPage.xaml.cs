using DriverSettlements.App.Controllers;
using DriverSettlements.App.Helpers;
using DriverSettlements.App.Models;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class DriverWeekPage : ContentPage
    {
        public DriverWeek DriverWeek { get; private set; }
        private readonly ApplicationController _applicationController;

        public DriverWeekPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Appearing += DriverWeekPage_Appearing;
            BindingContext = this;
            BasicRaportView.Children.Clear();
            BasicRaportView.RowDefinitions.Clear();
            UpdateButtonsAndTitleGrid();
            RenderDriverWeek();
        }

        private async void DriverWeekPage_Appearing(object sender, EventArgs e)
        {
            await SetDriverWeek(_applicationController.DriverController.WeekController.DeltaWeek);
            UpdateButtonsAndTitleGrid();
            RenderDriverWeek();
        }

        private async void PrevButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                await SetDriverWeek(_applicationController.DriverController.WeekController.PreviousDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderDriverWeek();
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
                await SetDriverWeek(_applicationController.DriverController.WeekController.NextDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderDriverWeek();
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async Task<bool> SetDriverWeek(int deltaWeek)
        {
            try
            {
                DriverWeek = await GetDriverWeek(deltaWeek);
                OnPropertyChanged("DriverWeek");
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ShellContent_Weeks, ex.Message, "OK");
                return false;
            }
        }

        private void RenderDriverWeek()
        {
            BasicRaportView.Children.Clear();
            BasicRaportView.RowDefinitions.Clear();
            WeeklySettlementGrid.Children.Clear();
            WeeklySettlementGrid.RowDefinitions.Clear();
            if (DriverWeek!=null)
            { 
                for (int i = 0; i < DriverWeek.BillingWeek.DriverReportsValueNames.Count + 1; i++)
                {
                    BasicRaportView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                    if (i == 0)
                    {
                        BasicRaportView.Add(new Label() { Text = "Wartość", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12}, 0, i);
                        BasicRaportView.Add(new Label() { Text = "Bolt", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 1, i);
                        BasicRaportView.Add(new Label() { Text = "Uber", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 2, i);
                        BasicRaportView.Add(new Label() { Text = "FreeNow", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 3, i);
                        BasicRaportView.Add(new Label() { Text = "Razem", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 4, i);
                    }
                    else
                    {
                        BasicRaportView.Add(new Label() { 
                            Text = DriverWeek.BillingWeek.DriverReportsValueNames[i - 1], 
                            HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 0, i);
                        if (DriverWeek.BillingWeek.DriverReportsValues.ContainsKey(ExternalReportType.Bolt))
                            BasicRaportView.Add(new Label() { 
                                Text = DriverWeek.BillingWeek.DriverReportsValues[ExternalReportType.Bolt][i - 1].ToString("#0.00 zł"), 
                                HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 1, i);
                        else
                            BasicRaportView.Add(new Label() { Text = "-", 
                                HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 1, i);
                        if (DriverWeek.BillingWeek.DriverReportsValues.ContainsKey(ExternalReportType.Uber))
                            BasicRaportView.Add(new Label() { Text = DriverWeek.BillingWeek.DriverReportsValues[ExternalReportType.Uber][i - 1].ToString("#0.00 zł"), 
                                HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 2, i);
                        else
                            BasicRaportView.Add(new Label() { Text = "-", 
                                HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 2, i);
                        if (DriverWeek.BillingWeek.DriverReportsValues.ContainsKey(ExternalReportType.Freenow))
                            BasicRaportView.Add(new Label() { Text = DriverWeek.BillingWeek.DriverReportsValues[ExternalReportType.Freenow][i - 1].ToString("#0.00 zł"), 
                                HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 3, i);
                        else
                            BasicRaportView.Add(new Label() { Text = "-", 
                                HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 3, i);
                        if (DriverWeek.BillingWeek.DriverReportsValues.Count > 0)
                            BasicRaportView.Add(new Label() { Text = DriverWeek.BillingWeek.DriverReportsValues.Sum(v => v.Value[i - 1]).ToString("#0.00 zł"), 
                                FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 4, i);
                        else
                            BasicRaportView.Add(new Label() { Text = "-", 
                                FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 4, i);
                    }
                }
                BasicRaportView.RowDefinitions.Add(new RowDefinition(new GridLength(1,GridUnitType.Auto)));
                if (DriverWeek.BillingWeek.WeeklySettlementComponents != null && DriverWeek.BillingWeek.WeeklySettlementComponents.Count > 0)
                {
                    MasterWeeklySettlementGrid.IsVisible = true;
                    int rowIdx = 0;
                    foreach(var weeklySettlementComponentKvp in DriverWeek.BillingWeek.WeeklySettlementComponents)
                    {
                        var settlementTitleTab = weeklySettlementComponentKvp.Key.Split('|');
                        var settlementTitle = Translate.TranslateSettlementType(settlementTitleTab[0]);
                        if (settlementTitleTab.Length > 1)
                            settlementTitle += $" {Translate.TranslateSettlementType(settlementTitleTab[1])}";
                        WeeklySettlementGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                        WeeklySettlementGrid.Add(new Label() {Text=$"{settlementTitle}:",HorizontalTextAlignment=TextAlignment.End },0, rowIdx);
                        WeeklySettlementGrid.Add(new Label() { Text = weeklySettlementComponentKvp.Value.ToString("+#0.00 zł;-#0.00 zł;0.00 zł"), HorizontalTextAlignment = TextAlignment.End }, 1, rowIdx++);
                    }
                    WeeklySettlementGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                    WeeklySettlementGrid.Add(new Label() { Text = "RAZEM:", FontAttributes=FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End }, 0, rowIdx);
                    WeeklySettlementGrid.Add(new Label() { Text = DriverWeek.BillingWeek.WeeklySettlementComponents.Sum(c=>c.Value).ToString("+#0.00 zł;-#0.00 zł;0.00 zł"), FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End }, 1, rowIdx++);
                }
                else
                    MasterWeeklySettlementGrid.IsVisible = false;
            }
        }

        private async Task<DriverWeek> GetDriverWeek(int deltaWeek)
        {
            var driverWeek = _applicationController.DriverController.WeekController.GetWeek<DriverWeek>(deltaWeek); 
            driverWeek.BillingWeek = await _applicationController.DriverController.GetBillingWeek(driverWeek.YearNumber, driverWeek.WeekNumber);
            driverWeek.BillingWeekStatus = driverWeek.BillingWeek.Status;
            return driverWeek;
        }

        private void UpdateButtonsAndTitleGrid()
        {
            TitleGrid.BackgroundColor = ColorHelper.GetTitleGridBacgroundColor(DriverWeek);
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
        }
    }
}