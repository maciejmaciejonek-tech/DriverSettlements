using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Models;
using System.ComponentModel.Design;

namespace DriverSettlements.App
{
    public partial class TenantSettlementTypePage : ContentPage, IQueryAttributable
    {
        private readonly ApplicationController _applicationController;
        public SettlementType SettlementType { get; private set; }
        private Mode _mode;

        public TenantSettlementTypePage()
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
                SettlementType = query["SettlementType"] as SettlementType;
                if (SettlementType == null)
                    await Shell.Current.GoToAsync("..");
                Type.Text = GetSettlementTypeString(SettlementType);
                Type.IsEnabled = false;
                Name.IsEnabled = SettlementType.TenantId.HasValue;
                Factor.IsEnabled = SettlementType.TenantId.HasValue;
                Debit.IsEnabled = SettlementType.TenantId.HasValue;
                Enabled.IsEnabled = SettlementType.TenantId.HasValue;
                SaveButton.IsVisible = SettlementType.TenantId.HasValue;
                OnPropertyChanged("SettlementType");
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        public static string GetSettlementTypeString(SettlementType settlementType)
        {
            return settlementType.TenantId.HasValue ? "użytkownika" : "systemowy";
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SettlementType.TypeName))
                {
                    await DisplayAlert(Title.Text, "Pole 'Nazwa' jest wymagane", "OK");
                    Name.Focus();
                    return;
                }
                if (SettlementType.Factor<=0)
                {
                    await DisplayAlert(Title.Text, "Pole 'Współczynnik' jest wymagane", "OK");
                    Factor.Focus();
                    return;
                }
                if (_mode == Mode.AddSettlementType)
                {
                    SettlementType.TenantId = null;
                    await _applicationController.TenantController.SettlementTypesController.AddObject(SettlementType);
                }
                else if (_mode == Mode.EditSettlementType)
                {
                    await _applicationController.TenantController.SettlementTypesController.UpdateObject(SettlementType);
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

        public enum Mode
        {
            Unknown = 0,
            AddSettlementType = 1,
            EditSettlementType = 2
        }
    }
}