using DriverSettlements.App.Controllers;
using DriverSettlements.App.Controllers.MasterAdmin;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Resources.Localization;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class TenantsPage : ContentPage
    {
        private readonly ApplicationController _applicationController;
        private readonly TenantsListController _tenantsListController;
        public TenantsPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            _tenantsListController = _applicationController.MasterAdminController.TenantsListController;
            Appearing += TenantsPage_Appearing;
        }

        private async void TenantsPage_Appearing(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchBar.Text))
                SearchBar.Text = string.Empty;
            try
            {
                _tenantsListController.EnsureListViewIsRegistered(tenantsView);
                if (!_tenantsListController.DataLoaded)
                {
                    if (await _tenantsListController.LoadData())
                        _tenantsListController.RefreshListView();
                }
                else
                    _tenantsListController.RefreshListView();
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.ShellContent_Tenants, ex.Message, "OK");
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0 && e.OldTextValue.Length > 1)
                Helpers.Keyboard.HideKeyboard();
            _tenantsListController.RefreshListView(FilterTenants);
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Helpers.Keyboard.HideKeyboard();
        }

        private async void tenantsView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (tenantsView.SelectedItem == null)
                return;
            await GoToTenantDetaisPageInEditMode(e.SelectedItem as LegalEntity);
        }

        private void tenantsView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            tenantsView.SelectedItem = null;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var button = sender as ButtonControl;
            if (button == null) return;
            button.IsInProgress = true;
            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Mode", TenantDetailsPage.Mode.AddNew }
                };
                await Shell.Current.GoToAsync(nameof(TenantDetailsPage), navigationParameter);
            }
            finally
            {
                button.IsInProgress = false;
            }
        }

        private async void EditMenuItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
                return;
            await GoToTenantDetaisPageInEditMode(menuItem.CommandParameter as LegalEntity);
        }

        private async Task GoToTenantDetaisPageInEditMode(LegalEntity legalEntity)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "Mode", TenantDetailsPage.Mode.Edit },
                { "Tenant", legalEntity }
            };
            await Shell.Current.GoToAsync(nameof(TenantDetailsPage), navigationParameter);
            return;
        }

        private async void DeleteMenuItem_Clicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
                return;
            var tenant = menuItem.CommandParameter as LegalEntity;
            if (tenant == null) return;
            try
            {
                if (await DisplayAlert(ApplicationStrings.TenantsPage_DeletingTenant, string.Format(ApplicationStrings.TenantsPage_DeletingTenantQuestion, tenant.FriendlyName), ApplicationStrings.Actions_Yes, ApplicationStrings.Actions_No))
                {
                    if (await _tenantsListController.DeleteObject(menuItem.CommandParameter as Tenant))
                        _tenantsListController.RefreshListView();
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(ApplicationStrings.TenantsPage_DeletingTenant, ex.Message, "OK");
            }
        }

        private List<LegalEntity> FilterTenants(List<LegalEntity> tenants)
        {
            if (!_tenantsListController.IncludeDeleted)
                tenants = tenants.Where(t => !t.Deleted).ToList();
            if (!string.IsNullOrWhiteSpace(SearchBar.Text))
                tenants = tenants.Where(t => t.CompanyName.StartsWith(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase)
                                    || t.FriendlyName.StartsWith(SearchBar.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return tenants.OrderBy(t => t.FriendlyName).ToList();
        }
    }
}