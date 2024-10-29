using DriverSettlements.App.Controllers;
using DriverSettlements.App.Models;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class TenantTariffPlanAssignmentsPage : ContentPage
    {
        public TenantTariffPlanAssignmentsWeek TenantTariffPlanAssignmentsWeek { get; private set; }
        private readonly ApplicationController _applicationController;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Dictionary<TariffPlanAssignment, List<View>> _tariffPlanAssignmentViewElements;

        public TenantTariffPlanAssignmentsPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            Appearing += TenantTariffPlanAssignmentsPage_Appearing;
            BindingContext = this;
            UpdateButtonsAndTitleGrid();
            RenderTenantTariffPlanAssignmentsWeek();
        }

        private async void TenantTariffPlanAssignmentsPage_Appearing(object sender, EventArgs e)
        {
            await SetTenantTariffPlanAssignmentsWeek(_applicationController.TenantController.WeekController.DeltaWeek);
            UpdateButtonsAndTitleGrid();
            RenderTenantTariffPlanAssignmentsWeek();
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
                await SetTenantTariffPlanAssignmentsWeek(_applicationController.TenantController.WeekController.PreviousDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantTariffPlanAssignmentsWeek();
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
                await SetTenantTariffPlanAssignmentsWeek(_applicationController.TenantController.WeekController.NextDeltaWeek);
                UpdateButtonsAndTitleGrid();
                RenderTenantTariffPlanAssignmentsWeek();
                TryToMakePeriodicAction(true);
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async Task<bool> SetTenantTariffPlanAssignmentsWeek(int deltaWeek)
        {
            try
            {
                TenantTariffPlanAssignmentsWeek = await GetTenantTariffPlanAssignmentsWeek(deltaWeek);
                OnPropertyChanged("TenantTariffPlanAssignmentsWeek");
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
                return false;
            }
        }

        private void RenderTenantTariffPlanAssignmentsWeek()
        {
            while (MasterView.Children.Count > 4)
                MasterView.Children.RemoveAt(4);
            while (MasterView.RowDefinitions.Count > 1)
                MasterView.RowDefinitions.RemoveAt(1);
            MasterView.IsVisible = TenantTariffPlanAssignmentsWeek !=null && TenantTariffPlanAssignmentsWeek.TariffPlanAssignments != null && TenantTariffPlanAssignmentsWeek.TariffPlanAssignments.Count > 0;
            _tariffPlanAssignmentViewElements = new Dictionary<TariffPlanAssignment, List<View>>();
            if (TenantTariffPlanAssignmentsWeek != null && TenantTariffPlanAssignmentsWeek.TariffPlanAssignments != null && TenantTariffPlanAssignmentsWeek.TariffPlanAssignments.Count > 0)
            {
                var lp = 0;
                foreach (var tariffPlanAssignment in TenantTariffPlanAssignmentsWeek.TariffPlanAssignments)
                {
                    if (!string.IsNullOrWhiteSpace(SearchBar.Text)
                        && !tariffPlanAssignment.Driver.CompanyName.StartsWith(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase)
                        && !tariffPlanAssignment.Driver.CompanyName.Contains($" {SearchBar.Text}", StringComparison.InvariantCultureIgnoreCase)
                        && !tariffPlanAssignment.TariffPlan.Name.StartsWith(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase)
                        && !tariffPlanAssignment.TariffPlan.Name.Contains($" {SearchBar.Text}", StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    lp++;
                    //MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(30)));
                    MasterView.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Auto)));
                    var lpLabel = new Label() { Text = (lp).ToString(), BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(lpLabel, 0, lp);
                    var driverNameLabel = new Label() { Text = tariffPlanAssignment.Driver.CompanyName, BackgroundColor = Colors.White, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontSize = 12 };
                    MasterView.Add(driverNameLabel, 1, lp);
                    var tariffPlanStackLayout = CreateTariffPlanLayout(tariffPlanAssignment);
                    MasterView.Add(tariffPlanStackLayout, 2, lp);
                    var weekStackLayout = CreateWeekStackLayout(tariffPlanAssignment);
                    MasterView.Add(weekStackLayout, 3, lp);
                    _tariffPlanAssignmentViewElements.Add(tariffPlanAssignment, new List<View>() {lpLabel, driverNameLabel, tariffPlanStackLayout, weekStackLayout });
                    var tapGestureRecognizer = new TapGestureRecognizer() { CommandParameter = tariffPlanAssignment };
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

        private async void TryToMakePeriodicAction(bool withQuestion = true)
        {
            if (TenantTariffPlanAssignmentsWeek.TariffPlanAssignments != null && TenantTariffPlanAssignmentsWeek.TariffPlanAssignments.Count != 0)
                return;
            if (!string.IsNullOrWhiteSpace(SearchBar.Text))
                return;
            if (!withQuestion || await DisplayAlert(Title.Text, "Czy chcesz powielić najmy samochodu z poprzedniego tygodnia?", ApplicationStrings.Actions_Yes, ApplicationStrings.Actions_No))
            {
                if (await _applicationController.TenantController.TariffPlanAssignmentsController.CopyTariffPlanAssignmentsFromPreviousWeek())
                {
                    _applicationController.TenantController.TariffPlanAssignmentsController.ForceDataRefresh = true;
                    await SetTenantTariffPlanAssignmentsWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                    UpdateButtonsAndTitleGrid();
                    RenderTenantTariffPlanAssignmentsWeek();
                }
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var tariffPlanAssignment = e.Parameter as TariffPlanAssignment;
            if (tariffPlanAssignment == null)
                return;
            var viewElements = _tariffPlanAssignmentViewElements[tariffPlanAssignment];
            if (EditButton.CommandParameter != null && EditButton.CommandParameter.Equals(tariffPlanAssignment))
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
                foreach (var viewElementKvp in _tariffPlanAssignmentViewElements)
                {
                    if (viewElementKvp.Key.Equals(tariffPlanAssignment))
                    {
                        if (TenantTariffPlanAssignmentsWeek != null && TenantTariffPlanAssignmentsWeek.BillingWeekStatus != BillingWeekStatus.Closed)
                        {
                            EditButton.IsVisible = true;
                            DeleteButton.IsVisible = true;
                        }
                        EditButton.CommandParameter = tariffPlanAssignment;
                        DeleteButton.CommandParameter = tariffPlanAssignment;
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

        private static StackLayout CreateTariffPlanLayout(TariffPlanAssignment tariffPlanAssignment)
        {
            var tariffPlanStackLayout = new StackLayout() { BackgroundColor = Colors.White };
            var tariffPlanGrid = new Grid() { Margin = 2, HorizontalOptions = LayoutOptions.Center, BackgroundColor = PeriodicTypeToColor(tariffPlanAssignment.PeriodicType) };
            tariffPlanGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(3)));
            tariffPlanGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Auto)));
            tariffPlanGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(3)));
            tariffPlanGrid.Add(new Label() { Text = tariffPlanAssignment.TariffPlan.Name, TextColor = Colors.Black, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Center, FontSize = 12 }, 1, 0);
            tariffPlanStackLayout.Add(tariffPlanGrid);
            return tariffPlanStackLayout;
        }

        private static StackLayout CreateWeekStackLayout(TariffPlanAssignment tariffPlanAssignment)
        {
            var weekStackLayout = new StackLayout() { BackgroundColor = Colors.White };
            var horizontalStackLayout = new HorizontalStackLayout() { HorizontalOptions = LayoutOptions.Center, Margin=2};
            horizontalStackLayout.Add(CreateDayLabel(tariffPlanAssignment.Monday));
            horizontalStackLayout.Add(CreateDayLabel(tariffPlanAssignment.Tuesday));
            horizontalStackLayout.Add(CreateDayLabel(tariffPlanAssignment.Wednesday));
            horizontalStackLayout.Add(CreateDayLabel(tariffPlanAssignment.Thursday));
            horizontalStackLayout.Add(CreateDayLabel(tariffPlanAssignment.Friday));
            horizontalStackLayout.Add(CreateDayLabel(tariffPlanAssignment.Saturday));
            horizontalStackLayout.Add(CreateDayLabel(tariffPlanAssignment.Sunday));
            weekStackLayout.Add(horizontalStackLayout);
            return weekStackLayout;
        }

        private static Label CreateDayLabel(bool occuupied)
        {
            var label = new Label()
            {
                BackgroundColor = occuupied ? Colors.LightGreen : Colors.LightGrey,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 12,
                Text = occuupied ? "X" : "O",
                WidthRequest=12
            };
            label.TextColor = Colors.Black;
            label.HorizontalTextAlignment = TextAlignment.Center;
            return label;
        }

        private async Task<TenantTariffPlanAssignmentsWeek> GetTenantTariffPlanAssignmentsWeek(int deltaWeek)
        {
            var tariffPlanAssignmentsWeek = _applicationController.TenantController.WeekController.GetWeek<TenantTariffPlanAssignmentsWeek>(deltaWeek);
            _applicationController.TenantController.TariffPlanAssignmentsController.SetYearAndWeek(tariffPlanAssignmentsWeek.YearNumber, tariffPlanAssignmentsWeek.WeekNumber);
            tariffPlanAssignmentsWeek.TariffPlanAssignments = await _applicationController.TenantController.TariffPlanAssignmentsController.GetObjects();
            tariffPlanAssignmentsWeek.BillingWeekStatus = _applicationController.TenantController.TariffPlanAssignmentsController.BillingWeekStatus;
            return tariffPlanAssignmentsWeek;
        }

        private void UpdateButtonsAndTitleGrid()
        {
            TitleGrid.BackgroundColor = Helpers.ColorHelper.GetTitleGridBacgroundColor(TenantTariffPlanAssignmentsWeek);
            EditButton.IsVisible = false;
            DeleteButton.IsVisible = false;
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
            AddTariffPlanAssignmentButton.IsVisible = TenantTariffPlanAssignmentsWeek != null && TenantTariffPlanAssignmentsWeek.BillingWeekStatus != BillingWeekStatus.Closed;
        }

        private async void AddTariffPlanAssignmentButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var buttonControl = sender as CustomControls.ButtonControl;
                if (buttonControl == null)
                    return;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Mode", TenantTariffPlanAssignmentPage.Mode.AddTariffPlanAssignment},
                    { "TariffPlanAssignment", new TariffPlanAssignment(){ YearNumber=TenantTariffPlanAssignmentsWeek.YearNumber, WeekNumber=TenantTariffPlanAssignmentsWeek.WeekNumber}}
                };
                await Shell.Current.GoToAsync(nameof(TenantTariffPlanAssignmentPage), navigationParameter);
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
            var tariffPlanAssignment = buttonControl.CommandParameter as TariffPlanAssignment;
            if (tariffPlanAssignment == null)
                return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "Mode", TenantTariffPlanAssignmentPage.Mode.EdittariffPlanAssignment},
                { "TariffPlanAssignment" , tariffPlanAssignment },
            };
            await Shell.Current.GoToAsync(nameof(TenantTariffPlanAssignmentPage), navigationParameter);
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as CustomControls.ButtonControl;
            if (buttonControl == null)
                return;
            var tariffPlanAssignment = buttonControl.CommandParameter as TariffPlanAssignment;
            if (tariffPlanAssignment == null)
                return;
            buttonControl.IsInProgress = true;
            try
            {
                if (await DisplayAlert(Title.Text, "Czy napewno chcesz usunąć najem samochodu?", ApplicationStrings.Actions_Yes, ApplicationStrings.Actions_No))
                {
                    if (await (_applicationController.TenantController.TariffPlanAssignmentsController.DeleteObject(tariffPlanAssignment)))
                    {
                        _applicationController.TenantController.TariffPlanAssignmentsController.ForceDataRefresh = true;
                        await SetTenantTariffPlanAssignmentsWeek(_applicationController.TenantController.WeekController.DeltaWeek);
                        UpdateButtonsAndTitleGrid();
                        RenderTenantTariffPlanAssignmentsWeek();
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
            RenderTenantTariffPlanAssignmentsWeek();
            UpdateButtonsAndTitleGrid();
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Helpers.Keyboard.HideKeyboard();
        }

        public static Color PeriodicTypeToColor(PeriodicType periodicType)
        {
            switch (periodicType)
            {
                case PeriodicType.TariffPlanWithoutDetails:
                    return Colors.LightGreen;
                case PeriodicType.TariffPlanWithDetails:
                    return Colors.LightBlue;
                case PeriodicType.None:
                default:
                    return Colors.LightGray;
            }
        }
    }
}