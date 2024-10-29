using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.Common.DataContracts.Tenant;
using DriverSettlements.Common.Models;

namespace DriverSettlements.App
{
    public partial class TenantFuelBillVerificationPage : ContentPage, IQueryAttributable
    {
        private readonly ApplicationController _applicationController;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool? _filterRequested;
        private FuelBill? _fuelBill;
        private List<FuelBill>? _fuelBills;
        private int _fuelBillIdx = -1;
        private DriverFuelBillsInfo? _driverFuelBillsInfo;
        private ImageControlOrientation? _orientation = ImageControlOrientation.Unknown;

        public TenantFuelBillVerificationPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            BindingContext = this;
            SelectedImage.SizeChanged += SelectedImage_SizeChanged;
        }

        private async void SelectedImage_SizeChanged(object sender, EventArgs e)
        {
            await LoadPhoto();
        }

        private async Task<bool> LoadPhoto(bool force = false)
        {
            try
            {

                var orientation = ImageControlOrientation.Unknown;
                if (SelectedImage.Width > SelectedImage.Height)
                    orientation = ImageControlOrientation.East;
                else if (SelectedImage.Width < SelectedImage.Height)
                    orientation = ImageControlOrientation.North;
                if (orientation != _orientation || force)
                {
                    if (_fuelBill != null)
                    {
                        _orientation = orientation;
                        var bytes = await _applicationController.TenantController.DownloadTenantFuelBillPhoto(_fuelBill.Id, SelectedImage.Height, SelectedImage.Width);
                        SelectedImage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
                return false;
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                _orientation = ImageControlOrientation.Unknown;
                _filterRequested = query["FilterRequested"] as bool?;
                var driverFuelBillsInfo = query["FuelBillsScope"] as DriverFuelBillsInfo;
                var fuelBill = query["FuelBillsScope"] as FuelBill;
                if (driverFuelBillsInfo != null)
                {
                    List<FuelBill> fuelBills;
                    if (_filterRequested.HasValue && _filterRequested.Value)
                        fuelBills = driverFuelBillsInfo.FuelBils.Where(fb => fb.Status == FuelBillStatus.Requested).ToList();
                    else
                        fuelBills = driverFuelBillsInfo.FuelBils.Where(fb => fb.Status != FuelBillStatus.Deleted).ToList();
                    SetFuelBills(fuelBills);
                }
                else if (fuelBill != null)
                {
                    SetFuelBill(fuelBill);
                }
                RefreshNavigator();
            }
            catch(Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private void SetFuelBill(FuelBill fuelBill)
        {
            _fuelBill = fuelBill;
            Price.Text = _fuelBill.Cost.ToString("#0.00");
            Date.Text = _fuelBill.Date.ToString("yyyy-MM-dd");
            ReferenceNumber.Text = _fuelBill.ReferenceNumber.ToString();
            Status.Text = Helpers.FuelBill.StatusToText(fuelBill.Status);
            Status.BackgroundColor = Helpers.FuelBill.StatusToColor(fuelBill.Status);
            if (Status.Handler!=null)
                Status.Handler.UpdateValue("Background");
        }

        private void SetFuelBill(int fuelBillIdx)
        {
            _fuelBillIdx = fuelBillIdx;            
            SetFuelBill(_fuelBills[_fuelBillIdx]);
        }

        private void SetFuelBills(List<FuelBill> fuelBills)
        {
            _fuelBills = fuelBills;
            SetFuelBill(0);
        }

        private void RefreshNavigator()
        {
            if (_fuelBills==null)
            {
                Navigator.IsVisible = false;
            }
            else
            {
                Navigator.IsVisible = true;
//                PrevButton.IsEnabled = _fuelBillIdx != 0;
                PrevButton.BackgroundColor = (_fuelBillIdx == 0) ? Colors.Gray : Application.Current.Resources.MergedDictionaries.First()["Primary"] as Color;
//                NextButton.IsEnabled = _fuelBillIdx + 1 != _fuelBills.Count;
                NextButton.BackgroundColor = (_fuelBillIdx+1 == _fuelBills.Count) ? Colors.Gray : Application.Current.Resources.MergedDictionaries.First()["Primary"] as Color;
                NavigatorText.Text = $"{_fuelBillIdx + 1} z {_fuelBills.Count}";
            }
        }

        private async void AcceptFuelBill_Clicked(object sender, EventArgs e)
        {
            await UpdatePhotoAction(sender as ButtonControl, FuelBillStatus.Acccepted);
        }

        private async void RejectFuelBill_Clicked(object sender, EventArgs e)
        {
            await UpdatePhotoAction(sender as ButtonControl,FuelBillStatus.Rejected);
        }

        private async Task<bool> UpdatePhotoAction(ButtonControl buttonControl, FuelBillStatus status)
        {
            if (buttonControl == null) return false;
            buttonControl.IsInProgress = true;
            try
            {
                _fuelBill.Status = status;
                await _applicationController.TenantController.UpdateTenantFuelBills(_fuelBill);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
            if (_fuelBills != null)
            {
                var currentIdx = _fuelBillIdx;
                if (_fuelBill.Equals(_fuelBills.Last()))
                    currentIdx--;
                _fuelBills.RemoveAt(_fuelBillIdx);
                if (currentIdx<0)
                    await Shell.Current.GoToAsync("..");
                else
                {
                    SetFuelBill(currentIdx);
                    RefreshNavigator();
                    await LoadPhoto(true);
                }
            }
            else
                await Shell.Current.GoToAsync("..");
            return true;
        }

        private async void PrevButton_Clicked(object sender, EventArgs e)
        {
            if (_fuelBillIdx < 1)
                return;
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            buttonControl.IsInProgress = true;
            try
            {
                SetFuelBill(_fuelBillIdx - 1);
                RefreshNavigator();
                await LoadPhoto(true);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            if (_fuelBillIdx + 1 >= _fuelBills.Count)
                return;
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            buttonControl.IsInProgress = true;
            try
            {
                SetFuelBill(_fuelBillIdx + 1);
                RefreshNavigator();
                await LoadPhoto(true);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }


        public enum ImageControlOrientation
        {
            Unknown = 0,
            North = 1,
            East = 2
        }
    }
}