using CommunityToolkit.Maui.Storage;
using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Helpers;
using DriverSettlements.App.Models;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Interfaces;
using DriverSettlements.Common.Models;
using System.Text;
#if ANDROID
using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
#endif

namespace DriverSettlements.App
{
    public partial class TenantWeekPage : ContentPage
    {
        public TenantWeek TenantWeek { get; private set; }
        public List<PickerWeek> PickerWeeks { get; private set; }
        private readonly ApplicationController _applicationController;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public List<Driver> Drivers { get; private set; }
        private Driver _selectedDriver = null;
        public TenantWeekPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Appearing += TenantWeekPage_Appearing;
            BindingContext = this;
            BasicRaportView.Children.Clear();
            BasicRaportView.RowDefinitions.Clear();
            UpdateButtonsAndTitleGrid();
            PickerWeeks = new List<PickerWeek>()
            {
                new PickerWeek() { Name = "2 tygodnie", Number = 2},
                new PickerWeek() { Name = "3 tygodnie", Number = 3},
                new PickerWeek() { Name = "4 tygodnie", Number = 4},
                new PickerWeek() { Name = "5 tygodni", Number = 5},
                new PickerWeek() { Name = "6 tygodni", Number = 6},
                new PickerWeek() { Name = "7 tygodni", Number = 7},
                new PickerWeek() { Name = "8 tygodni", Number = 8},
                new PickerWeek() { Name = "9 tygodni", Number = 9}
            };
            OnPropertyChanged("PickerWeeks");
            WeekPicker.SelectedIndex = 3;
            RenderTenantWeek();
        }

        private async void TenantWeekPage_Appearing(object sender, EventArgs e)
        {
            WeekPicker.HeightRequest = DownloadCustomReportButton.Height;
            Drivers = (await _applicationController.TenantController.DriversController.GetObjects())
                .Where(d => d.Confirmed && !d.Deleted).OrderBy(d => d.CompanyName).ToList();
            OnPropertyChanged("Drivers");
            await SetTenantWeek(_applicationController.TenantController.WeekController.DeltaWeek);
            UpdateButtonsAndTitleGrid();
            RenderTenantWeek();
        }

        private async void PrevButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                await SetTenantWeek(_applicationController.TenantController.WeekController.PreviousDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantWeek();
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
                await SetTenantWeek(_applicationController.TenantController.WeekController.NextDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantWeek();
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async Task<bool> SetTenantWeek(int deltaWeek)
        {
            try
            {
                TenantWeek = await GetTenantWeek(deltaWeek);
                OnPropertyChanged("TenantWeek");
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ShellContent_Weeks, ex.Message, "OK");
                return false;
            }
        }

        private void RenderTenantWeek()
        {
            BasicRaportView.Children.Clear();
            BasicRaportView.RowDefinitions.Clear();
            WeeklySettlementGrid.Children.Clear();
            WeeklySettlementGrid.RowDefinitions.Clear();
            if (TenantWeek == null)
            {
                BoltActions.IsVisible = false;
                BoltMessage.IsVisible = false;
                UberActions.IsVisible = false;
                UberMessage.IsVisible = false;
                FreenowActions.IsVisible = false;
                FreenowMessage.IsVisible = false;
                MasterWeeklySettlementGrid.IsVisible = false;
                return;
            }
            for (int i = 0; i < TenantWeek.TenantReportsValueNames.Count + 1; i++)
            {
                BasicRaportView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                if (i == 0)
                {
                    BasicRaportView.Add(new Label() { Text = "Wartość", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12}, 0, i);
                    BasicRaportView.Add(new Label() { Text = "Bolt", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 1, i);
                    BasicRaportView.Add(new Label() { Text = "Uber", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 2, i);
                    BasicRaportView.Add(new Label() { Text = "FreeNow", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize = 12 }, 3, i);
                }
                else
                {
                    BasicRaportView.Add(new Label() { 
                        Text = TenantWeek.TenantReportsValueNames[i - 1], 
                        HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 0, i);
                    if (TenantWeek.TenantReportsValues.ContainsKey(ExternalReportType.Bolt))
                        BasicRaportView.Add(new Label() { 
                            Text = TenantWeek.TenantReportsValues[ExternalReportType.Bolt][i - 1].ToString("#0.00 zł"), 
                            HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 1, i);
                    else
                        BasicRaportView.Add(new Label() { Text = "-", 
                            HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 1, i);
                    if (TenantWeek.TenantReportsValues.ContainsKey(ExternalReportType.Uber))
                        BasicRaportView.Add(new Label() { Text = TenantWeek.TenantReportsValues[ExternalReportType.Uber][i - 1].ToString("#0.00 zł"), 
                            HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 2, i);
                    else
                        BasicRaportView.Add(new Label() { Text = "-", 
                            HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 2, i);
                    if (TenantWeek.TenantReportsValues.ContainsKey(ExternalReportType.Freenow))
                        BasicRaportView.Add(new Label() { Text = TenantWeek.TenantReportsValues[ExternalReportType.Freenow][i - 1].ToString("#0.00 zł"), 
                            HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 3, i);
                    else
                        BasicRaportView.Add(new Label() { Text = "-", 
                            HorizontalTextAlignment = TextAlignment.Center, BackgroundColor = Colors.White, TextColor = Colors.Black, FontSize=12}, 3, i);
                }
            }
            BasicRaportView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
            var isCurrentWeek = _applicationController.TenantController.WeekController.DeltaWeek == 0;
            var boltReport = TenantWeek.ExternalReports.FirstOrDefault(r => r.ReportType == ExternalReportType.Bolt);
            BoltActions.IsVisible = !isCurrentWeek && (boltReport == null || boltReport.Status != ExternalReportStatus.Generated);
            BoltMessage.Text = boltReport?.ErrorMessage?.Replace("ERR:Exception: ", "");
            BoltMessage.IsVisible = !isCurrentWeek && !string.IsNullOrWhiteSpace(BoltMessage.Text);
            var uberReport = TenantWeek.ExternalReports.FirstOrDefault(r => r.ReportType == ExternalReportType.Uber);
            UberActions.IsVisible = !isCurrentWeek && (uberReport == null || uberReport.Status != ExternalReportStatus.Generated);
            UberMessage.Text = uberReport?.ErrorMessage?.Replace("ERR:Exception: ", "");
            UberMessage.IsVisible = !isCurrentWeek && !string.IsNullOrWhiteSpace(UberMessage.Text);
            var freenowReport = TenantWeek.ExternalReports.FirstOrDefault(r => r.ReportType == ExternalReportType.Freenow);
            FreenowActions.IsVisible = !isCurrentWeek && (freenowReport == null || freenowReport.Status != ExternalReportStatus.Generated);
            FreenowMessage.Text = freenowReport?.ErrorMessage?.Replace("ERR:Exception: ", "");
            FreenowMessage.IsVisible = !isCurrentWeek && !string.IsNullOrWhiteSpace(FreenowMessage.Text);

            if (TenantWeek.WeeklySettlementComponents != null && TenantWeek.WeeklySettlementComponents.Count > 0)
            {
                MasterWeeklySettlementGrid.IsVisible = true;
                int rowIdx = 0;
                foreach (var weeklySettlementComponentKvp in TenantWeek.WeeklySettlementComponents)
                {
                    var settlementTitleTab = weeklySettlementComponentKvp.Key.Split('|');
                    var settlementTitle = Translate.TranslateSettlementType(settlementTitleTab[0]);
                    if (settlementTitleTab.Length > 1)
                        settlementTitle += $" {Translate.TranslateSettlementType(settlementTitleTab[1])}";
                    WeeklySettlementGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                    WeeklySettlementGrid.Add(new Label() { Text = $"{settlementTitle}:", HorizontalTextAlignment = TextAlignment.End }, 0, rowIdx);
                    WeeklySettlementGrid.Add(new Label() { Text = weeklySettlementComponentKvp.Value.ToString("+#0.00 zł;-#0.00 zł;0.00 zł"), HorizontalTextAlignment = TextAlignment.End }, 1, rowIdx++);
                }
                WeeklySettlementGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                WeeklySettlementGrid.Add(new Label() { Text = "RAZEM:", FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End }, 0, rowIdx);
                WeeklySettlementGrid.Add(new Label() { Text = TenantWeek.WeeklySettlementComponents.Sum(c => c.Value).ToString("+#0.00 zł;-#0.00 zł;0.00 zł"), FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.End }, 1, rowIdx++);
            }
            else
                MasterWeeklySettlementGrid.IsVisible = false;            
        }

        private async Task<TenantWeek> GetTenantWeek(int deltaWeek)
        {
            var tenantWeek = _applicationController.TenantController.WeekController.GetWeek<TenantWeek>(deltaWeek);
            await _applicationController.TenantController.LoadWeeklyExternalReportsMetadata(tenantWeek, _selectedDriver);
            _applicationController.TenantController.DriversController.ForceDataRefresh = true;
            return tenantWeek;
        }

        private void UpdateButtonsAndTitleGrid()
        {
            TitleGrid.BackgroundColor = Helpers.ColorHelper.GetTitleGridBacgroundColor(TenantWeek);
            if (_applicationController.TenantController.WeekController.DeltaWeek> 0)
            {
//                NextButton.IsEnabled = true;
                NextButton.BackgroundColor = Application.Current.Resources.MergedDictionaries.First()["Primary"] as Color;
            }
            else
            {
//                NextButton.IsEnabled = false;
                NextButton.BackgroundColor = Colors.Grey;
            }
            var mappingDrivers = TenantWeek != null && TenantWeek.ExternalReports.Count(r => r.Status == ExternalReportStatus.Generated) == 3 && TenantWeek.ExternalReports.Sum(r => r.NotComfirmedDriversIdentifiers.Count) > 0;
            var mappedDrivers = TenantWeek != null && TenantWeek.ExternalReports.Count(r => r.Status == ExternalReportStatus.Generated) == 3 && TenantWeek.ExternalReports.Sum(r => r.NotComfirmedDriversIdentifiers.Count) == 0;
            ReportsGrid.IsVisible = TenantWeek != null && TenantWeek.BillingWeekStatus == BillingWeekStatus.Closed;
            WeekFunctionalities.IsVisible = TenantWeek != null && TenantWeek.BillingWeekStatus != BillingWeekStatus.Closed;
            if (WeekFunctionalities.IsVisible)
            {
                if (TenantWeek.WeeklySettlementsInfo == null)
                {
                    TariffPlanAssignemtsButton.Text = ApplicationStrings.Tenant_CarRentals;
                    FuelBillsButton.Text = ApplicationStrings.ShellContent_FuelBills;
                    SettlementItemsButton.Text = ApplicationStrings.ShellContent_ExtrasAndDeducts;
                }
                else
                {
                    TariffPlanAssignemtsButton.Text = $"{ApplicationStrings.Tenant_CarRentals} ({TenantWeek.WeeklySettlementsInfo.NumberOfTariffPlanAssignments})";
                    FuelBillsButton.Text = $"{ApplicationStrings.ShellContent_FuelBills} ({TenantWeek.WeeklySettlementsInfo.NumberOfRequestedFuelBills}/{TenantWeek.WeeklySettlementsInfo.NumberOfAcceptedFuelBills}/{TenantWeek.WeeklySettlementsInfo.NumberOfRejectedFuelBills})";
                    SettlementItemsButton.Text = $"{ApplicationStrings.ShellContent_ExtrasAndDeducts} ({TenantWeek.WeeklySettlementsInfo.NumberOfSettlementItems })";
                }
                TariffPlanAssignemtsButton.LoadingText = TariffPlanAssignemtsButton.Text;
                FuelBillsButton.LoadingText = FuelBillsButton.Text;
                SettlementItemsButton.LoadingText = SettlementItemsButton.Text;
            }
            OpenDriversMappingsButton.IsVisible = WeekFunctionalities.IsVisible && mappingDrivers;
            ConfirmWeekButton.IsVisible = WeekFunctionalities.IsVisible && mappedDrivers;
            RenderCustomReportDateRangeInfo();
        }

        private async Task<bool> DownloadExternalReport(ButtonControl buttonControl,ExternalReportType reportType, bool overwrite = true)
        {
            if (buttonControl == null) return false;
            buttonControl.IsInProgress = true;
            try
            {
                var externalReportMetadata = await _applicationController.TenantController.DownloadExternalReport(TenantWeek.YearNumber, TenantWeek.WeekNumber, reportType, overwrite);
                return true;
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert(ApplicationStrings.ExternalReports_DownloadingWeeklyReportUber, ex.Message, "OK");
                return false;
            }
            finally
            {
                await SetTenantWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                RenderTenantWeek();
                buttonControl.IsInProgress = false;
            }
        }

        private async Task<bool> UploadExternalReport(ButtonControl buttonControl, ExternalReportType reportType)
        {
            if (buttonControl == null) return false;
            buttonControl.IsInProgress = true;
            var startsWith = new string[0];
            var dialogTitle = string.Empty;
            var errorMessage = string.Empty;
            var errorDialogTitle = string.Empty;
            switch(reportType)
            {
                case ExternalReportType.Bolt:
                    startsWith = new[] {
                            $"Bolt Weekly Report - {TenantWeek.YearNumber:#0000}W{TenantWeek.WeekNumber:#00}",
                            $"Tygodniowy raport Bolt - {TenantWeek.YearNumber:#0000}W{TenantWeek.WeekNumber:#00}" };
                    dialogTitle = "Please select Bolt report";
                    errorMessage = ApplicationStrings.TenantWeekDetailsPage_WrongBoltReportName;
                    errorDialogTitle = ApplicationStrings.ExternalReports_UploadingWeeklyReportBolt;
                    break;
                case ExternalReportType.Uber:
                    startsWith = new[]
                        {$"{TenantWeek.DateFrom:yyyyMMdd}-{TenantWeek.DateTo.AddDays(1):yyyyMMdd}-payments_driver"};
                    dialogTitle = "Please select Uber report";
                    errorMessage = ApplicationStrings.TenantWeekDetailsPage_WrongUberReportName;
                    errorDialogTitle = ApplicationStrings.ExternalReports_UploadingWeeklyReportUber;
                    break;
                case ExternalReportType.Freenow:
                    startsWith = new[]
                        {$"earnings_{TenantWeek.DateFrom:yyyy-MM-dd}_{TenantWeek.DateTo:yyyy-MM-dd}.zip"};
                    dialogTitle = "Please select Freenow report";
                    errorMessage = ApplicationStrings.TenantWeekDetailsPage_WrongFreenowReportName;
                    errorDialogTitle = ApplicationStrings.ExternalReports_UploadingWeeklyReportFreenow;
                    break;
            }
            try
            {
                await UploadFile(reportType, dialogTitle, errorMessage, startsWith);
                return true;
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert(errorDialogTitle, ex.Message, "OK");
                return false;
            }
            finally
            {
                await SetTenantWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                RenderTenantWeek();
                buttonControl.IsInProgress = false;
            }
        }

        private async Task UploadFile(ExternalReportType reportType, string dialogTitle, string errorMessage, string[] startsWith)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = dialogTitle
            });
            if (result == null || result.FullPath == null)
                return;

            if (!startsWith.Any(sw => result.FileName.StartsWith(sw, StringComparison.InvariantCultureIgnoreCase)))
                throw new Exception(string.Format(errorMessage, string.Join("", startsWith.Select(sw => $"\r\n{sw}"))));

            byte[] byteArray;
            using (var stream = await result.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                byteArray = memoryStream.ToArray();
            }
            await _applicationController.TenantController.UploadExternalReport(TenantWeek, reportType, byteArray);
        }

        private async void BoltDownload_Clicked(object sender, EventArgs e)
        {
            await DownloadExternalReport(sender as ButtonControl, ExternalReportType.Bolt);
        }

        private async void UberDownload_Clicked(object sender, EventArgs e)
        {
            await DownloadExternalReport(sender as ButtonControl, ExternalReportType.Uber);
        }

        private async void FreenowDownload_Clicked(object sender, EventArgs e)
        {
            await DownloadExternalReport(sender as ButtonControl, ExternalReportType.Freenow);
        }

        private async void BoltUpload_Clicked(object sender, EventArgs e)
        {
            await UploadExternalReport(sender as ButtonControl, ExternalReportType.Bolt);
        }

        private async void UberUpload_Clicked(object sender, EventArgs e)
        {
            await UploadExternalReport(sender as ButtonControl, ExternalReportType.Uber);
        }

        private async void FreenowUpload_Clicked(object sender, EventArgs e)
        {
            await UploadExternalReport(sender as ButtonControl, ExternalReportType.Freenow);
        }

        private async void OpenDriversMappingsButton_Clicked(object sender, EventArgs e)
        {
            var driverIds = new HashSet<int>();
            foreach (var externalReport in TenantWeek.ExternalReports)
            {
                if (externalReport.NotComfirmedDriversIdentifiers == null)
                    continue;
                foreach (var driverId in externalReport.NotComfirmedDriversIdentifiers)
                    driverIds.Add(driverId);
            }
            if (driverIds.Count>0)
                await GoToDriversConfirmationAndMappingsPage(driverIds);
        }

        private async void DownloadWeeklyReportButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            try
            {
                buttonControl.IsInProgress = true;
                var report = await _applicationController.TenantController.GetBasicReport(TenantWeek.YearNumber, TenantWeek.WeekNumber);
                report.Header = report.Header.Select(Translate.TranslateSettlementType).ToList();
                buttonControl.IsInProgress = false;
                using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(report.GetString()));
                // this will run for Android 33 and greater
                if (DeviceInfo.Platform == DevicePlatform.Android && OperatingSystem.IsAndroidVersionAtLeast(33))
                {
#if ANDROID
                var activity = Platform.CurrentActivity ?? throw new NullReferenceException("Current activity is null");
                if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(activity, new[] { Manifest.Permission.ReadExternalStorage }, 1);
                }
#endif
                }
                var path = await FileSaver.SaveAsync(
                    $"{ApplicationStrings.Reports_BasicReport} - {ApplicationStrings.Calendar_Week.ToLower()} {TenantWeek.YearNumber:#0000}.{TenantWeek.WeekNumber:#00} {ApplicationStrings.Actions_From} {TenantWeek.DateFrom:yyyy-MM-dd} {ApplicationStrings.Actions_To} {TenantWeek.DateTo:yyyy-MM-dd}.csv",
                    memoryStream, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert(Title.Text , ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async void DownloadCustomReportButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            var pickerWeek = WeekPicker.SelectedItem as PickerWeek;
            if (pickerWeek == null || TenantWeek == null)
                return;
            try
            {
                buttonControl.IsInProgress = true;
                var weeks = new List<Week>();
                for(int i = _applicationController.TenantController.WeekController.DeltaWeek; i<_applicationController.TenantController.WeekController.DeltaWeek+pickerWeek.Number;i++)
                    weeks.Add(_applicationController.TenantController.WeekController.GetWeek<Week>(i, false));
                weeks.Reverse();
                var report = await _applicationController.TenantController.GetCustomReport(weeks);
                report.Header = report.Header.Select(Translate.TranslateSettlementType).ToList();
                buttonControl.IsInProgress = false;
                using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(report.GetString()));
                // this will run for Android 33 and greater
                if (DeviceInfo.Platform == DevicePlatform.Android && OperatingSystem.IsAndroidVersionAtLeast(33))
                {
#if ANDROID
                var activity = Platform.CurrentActivity ?? throw new NullReferenceException("Current activity is null");
                if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(activity, new[] { Manifest.Permission.ReadExternalStorage }, 1);
                }
#endif
                }
                var path = await FileSaver.SaveAsync(
                    $"{ApplicationStrings.Reports_OtherReport} - {ApplicationStrings.Actions_From} { weeks.First().DateFrom:yyyy-MM-dd} {ApplicationStrings.Actions_To} {weeks.Last().DateTo:yyyy-MM-dd}.csv",
                    memoryStream, _cancellationTokenSource.Token);
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

        private async void FuelBillsButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TenantFuelBillsPage));
        }

        private async void ExtrasAndDeductsButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TenantSettlementItemsPage));
        }

        private async void CarRentalsButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TenantTariffPlanAssignmentsPage));
        }

        private async void ConfirmWeekButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            try
            {
                if (TenantWeek.WeeklySettlementsInfo==null)
                {
                    await DisplayAlert(Title.Text, "Wystąpił nieoczekiwany błąd z brakiem inforamcji o rozliczeniach.", "OK");
                    return;
                }
                if (TenantWeek.WeeklySettlementsInfo.NumberOfRequestedFuelBills > 0)
                {
                    await DisplayAlert(Title.Text, "Nie można zatwierdzić tygodnia bez wcześniejszej akceptacji rachunków za paliwo.", "OK");
                    return;
                }
                if (TenantWeek.WeeklySettlementsInfo.NumberOfTariffPlanAssignments == 0)
                {
                    if (!await DisplayAlert(Title.Text,"Czy napewno chcesz zatwierdzić tydzień bez przypisania planów taryfowych?", ApplicationStrings.Actions_Yes, ApplicationStrings.Actions_No))
                        return;
                }
                if (TenantWeek.WeeklySettlementsInfo.NumberOfSettlementItems == 0)
                {
                    if (!await DisplayAlert(Title.Text, "Czy napewno chcesz zatwierdzić tydzień bez przypisania rozliczeń?", ApplicationStrings.Actions_Yes, ApplicationStrings.Actions_No))
                        return;
                }

                buttonControl.IsInProgress = true;
                if (await _applicationController.TenantController.ConfirmBillingWeek(TenantWeek.YearNumber, TenantWeek.WeekNumber))
                {
                    TenantWeek.BillingWeekStatus = BillingWeekStatus.Closed;
                    await SetTenantWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                    UpdateButtonsAndTitleGrid();
                    RenderTenantWeek();
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

        private void FilterButton_Clicked(object sender, EventArgs e)
        {
            FilterDriverPanel.IsVisible = true;
            FilterButton.IsVisible = false;
            RemoveFilterButton.IsVisible = true;
        }

        private async void RemoveFilterButton_Clicked(object sender, EventArgs e)
        {
            RemoveFilterButton.IsVisible = false;
            FilterButton.IsVisible = true;
            FilterDriverPanel.IsVisible = false;
            DriverName.Text = "";
            if (_selectedDriver != null)
            {
                _selectedDriver = null;
                await SetTenantWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantWeek();
            }
            Helpers.Keyboard.HideKeyboard();
        }

        private async void DriverName_TextChanged(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangedEventArgs e)
        {
            if (e.Reason != zoft.MauiExtensions.Controls.AutoCompleteEntryTextChangeReason.UserInput)
                return;
            var autocompleteEntry = sender as zoft.MauiExtensions.Controls.AutoCompleteEntry;
            if (autocompleteEntry == null)
                return;
            if (_selectedDriver != null)
            {
                _selectedDriver = null;
                await SetTenantWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantWeek();
            }
            var drivers = (await _applicationController.TenantController.DriversController.GetObjects())
                .Where(d => d.Confirmed && !d.Deleted);
            if (string.IsNullOrWhiteSpace(autocompleteEntry.Text))
                Drivers = drivers.OrderBy(d => d.CompanyName).ToList();
            else
                Drivers = drivers.Where(d => d.CompanyName.StartsWith(autocompleteEntry.Text, StringComparison.InvariantCultureIgnoreCase)
                    || d.CompanyName.Contains($" {autocompleteEntry.Text}", StringComparison.InvariantCultureIgnoreCase)).OrderBy(d => d.CompanyName).ToList();
            OnPropertyChanged("Drivers");
        }

        private async void DriverName_SuggestionChosen(object sender, zoft.MauiExtensions.Controls.AutoCompleteEntrySuggestionChosenEventArgs e)
        {
            var driver = e.SelectedItem as Driver;
            if (_selectedDriver != driver)
            {
                _selectedDriver = driver;
                await SetTenantWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantWeek();
                Helpers.Keyboard.HideKeyboard();
            }
        }

        private async Task GoToDriversConfirmationAndMappingsPage(HashSet<int> driverIds)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "DriverIds", driverIds}
            };
            await Shell.Current.GoToAsync(nameof(DriversConfirmationAndMappingsPage), navigationParameter);
        }

        private void RenderCustomReportDateRangeInfo()
        {
            var pickerWeek = WeekPicker.SelectedItem as PickerWeek;
            if (pickerWeek == null || TenantWeek==null)
            {
                DateTo.Text = DateFrom.Text = "YYYY-MM-DD";
                return;
            }
            DateTo.Text = TenantWeek.DateTo.ToString("yyyy-MM-dd");
            var firstWeek = _applicationController.TenantController.WeekController.GetWeek<Week>(_applicationController.TenantController.WeekController.DeltaWeek + pickerWeek.Number-1,false);
            DateFrom.Text = firstWeek.DateFrom.ToString("yyyy-MM-dd");
        }

        private void WeekPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderCustomReportDateRangeInfo();
        }
    }

    public class PickerWeek
    {
        public string Name { get; set; }
        public int Number { get; set; }
    }
}