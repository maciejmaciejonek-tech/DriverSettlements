using DriverSettlements.App.Controllers;
using DriverSettlements.App.Resources.Localization;

namespace DriverSettlements.App
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Appearing += MainPage_Appearing;
        }

        private void MainPage_Appearing(object sender, EventArgs e)
        {
            Label.Text = ApplicationStrings.Messages_NoInformation;
        }
    }
}