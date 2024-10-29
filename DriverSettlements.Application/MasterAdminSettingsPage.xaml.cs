using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace DriverSettlements.App
{
    public partial class MasterAdminSettingsPage : ContentPage
    {
        private readonly ApplicationController _applicationController;
        public MasterAdminSettingsPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
        }

        private async void AddDefaultReports_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            buttonControl.IsInProgress = true;
            try
            {
                await _applicationController.MasterAdminController.AddDefaultReportDefinitions();
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert("", ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async void AddSettlementTypes_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            buttonControl.IsInProgress = true;
            try
            {
                await _applicationController.MasterAdminController.AddSettlementTypes();
            }
            catch (Exception ex)
            {
                buttonControl.IsInProgress = false;
                await DisplayAlert("", ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }
    }
}