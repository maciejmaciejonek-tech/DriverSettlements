using DriverSettlements.App.Controllers;
using DriverSettlements.App.Resources.Localization;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace DriverSettlements.App
{
    public partial class ErrorPage : ContentPage
    {
        private readonly ApplicationController _applicationController;
        public ErrorPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            ErrorMessageBody.Text = _applicationController.ExceptionMessage;
        }

        private void OnClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }
    }
}