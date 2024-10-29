using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Models;
using System.ComponentModel.Design;

namespace DriverSettlements.App
{
    public partial class TenantTariffPlanPage : ContentPage, IQueryAttributable
    {
        private readonly ApplicationController _applicationController;
        public TariffPlan TariffPlan { get; private set; }
        private Mode _mode;

        public TenantTariffPlanPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            BindingContext = this;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                var mode = query["Mode"] as Mode?;
                if (!mode.HasValue)
                    await Shell.Current.GoToAsync("..");
                _mode = mode.Value;
                TariffPlan = query["TariffPlan"] as TariffPlan;
                if (TariffPlan==null)
                    await Shell.Current.GoToAsync("..");
                OnPropertyChanged("TariffPlan");
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TariffPlan.Name))
                {
                    await DisplayAlert(Title.Text, "Pole 'Nazwa planu' jest wymagane", "OK");
                    return;
                }
                if (TariffPlan.WeeklyRate<0)
                {
                    await DisplayAlert(Title.Text, "Pole 'Tygodniowa stawka najmu; jest wymagane", "OK");
                    WeeklyRate.Focus();
                    return;
                }
                if (_mode == Mode.AddTariffPlan)
                {
                    await _applicationController.TenantController.TariffPlansController.AddObject(TariffPlan);
                }
                else if (_mode == Mode.EdittariffPlan)
                {
                    await _applicationController.TenantController.TariffPlansController.UpdateObject(TariffPlan);
                }
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

        private void WeeklyRate_Focused(object sender, FocusEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                 WeeklyRate.CursorPosition = 0;
                WeeklyRate.SelectionLength = WeeklyRate.Text == null ? 0 : WeeklyRate.Text.Length;
            });
        }

        public enum Mode
        {
            Unknown = 0,
            AddTariffPlan = 1,
            EdittariffPlan = 2
        }
    }
}