using DriverSettlements.App.Controllers;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Models;
using DriverSettlements.Common.Helpers;
using DriverSettlements.Common.Models;
using System.Collections;
using System.Globalization;
using static DriverSettlements.App.TenantFuelBillVerificationPage;

namespace DriverSettlements.App
{
    public partial class DriverFuelBillPage : ContentPage, IQueryAttributable
    {
        public DriverFuelBillsWeek DriverFuelBillsWeek { get; private set; }
        private readonly ApplicationController _applicationController;
        public int _deltaWeek = 0;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Mode? _mode;
        private FuelBill _fuelBill;
        private ImageControlOrientation? _orientation = ImageControlOrientation.Unknown;

        public DriverFuelBillPage()
        {
            InitializeComponent();
            _applicationController = ServiceHelper.GetService<ApplicationController>();
            BindingContext = this;
            DatePicker.Date = DateTime.Now;
            DatePicker.MinimumDate = DateTime.Now.AddDays(-7);
            DatePicker.MaximumDate = DatePicker.Date;
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
                if (!_mode.HasValue || _mode == Mode.AddNew)
                    return false;
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
                        var bytes = await _applicationController.DriverController.DownloadDriverFuelBillPhoto(_fuelBill.Id, SelectedImage.Width, SelectedImage.Height);
                        SelectedImage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.Message, "OK");
                return false;
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                _mode = (Mode)query["Mode"];
                _fuelBill = query["FuelBill"] as FuelBill;
                if (_mode == Mode.Edit)
                {
                    ReferenceNumber.Text = _fuelBill.ReferenceNumber;
                    Price.Text = _fuelBill.Cost.ToString(CultureInfo.InvariantCulture);
                    DatePicker.Date = _fuelBill.Date;
                    //load photo
                }
                CapturePhoto.IsVisible = _mode == Mode.AddNew;
                PickPhoto.IsVisible = _mode == Mode.AddNew;
                RemoveFuelBill.IsVisible = _mode == Mode.Edit;
                RequestFuelBill.IsVisible = _mode == Mode.Edit && _fuelBill.Status != FuelBillStatus.Requested;
                RequestFuelBill.IsVisible = _mode == Mode.Edit && _fuelBill.Status != FuelBillStatus.Deleted;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private async void SaveFuelBill_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl == null) return;
            if (!await ValidateForm())
                return;
            buttonControl.IsInProgress = true;
            try
            {
                _fuelBill.Date = DatePicker.Date;
                _fuelBill.Cost = double.Parse(Price.Text.Replace(',','.'), CultureInfo.InvariantCulture);
                _fuelBill.ReferenceNumber = ReferenceNumber.Text;
                if (_mode == Mode.AddNew)
                {
                    _fuelBill.Status = FuelBillStatus.Requested;
                    if (await _applicationController.DriverController.FuelBillsController.AddObject(_fuelBill))
                    {
                        var byteArray = await ConvertImageSourceToStreamAsync(SelectedImage.Source);
                        if (await _applicationController.DriverController.FuelBillsController.UploadDriverFuelBillPhoto(_fuelBill.Id, byteArray))
                            await Shell.Current.GoToAsync("..");
                    }
                }
                else if (_mode == Mode.Edit)
                {
                    if (await _applicationController.DriverController.FuelBillsController.UpdateObject(_fuelBill))
                        await Shell.Current.GoToAsync("..");
                }
            }
            catch(Exception ex)
            {
                if (_mode == Mode.AddNew) 
                    await DisplayAlert("Dodawanie rachunku za paliwo", ex.Message, "OK");
                else if (_mode == Mode.Edit)
                    await DisplayAlert("Edycja rachunku za paliwo", ex.Message, "OK");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async Task<bool> ValidateForm(bool silently = false)
        {
            if (string.IsNullOrWhiteSpace(Price.Text))
            {
                if (!silently)
                    await DisplayAlert("Dodawanie rachunku za paliwo", "Wartość rachunku jest wymagana", "OK");
                Price.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(ReferenceNumber.Text))
            {
                if (!silently)
                    await DisplayAlert("Dodawanie rachunku za paliwo", "Numer rachunku jest wymagany", "OK");
                ReferenceNumber.Focus();
                return false;
            }
            if (SelectedImage.Source == null)
            {
                if (!silently)
                    await DisplayAlert("Dodawanie rachunku za paliwo", "Zdjęcie rachunku jest wymagane", "OK");
                CapturePhoto.Focus();
                return false;
            }
            return true;
        }

        private async Task<byte[]> ConvertImageSourceToStreamAsync(ImageSource imageSource)
        {
            if (imageSource is StreamImageSource)
            {
                using (var stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None))
                    return await Helpers.ImageHelper.ChangeQuality(stream,0.2f);
            }
            else if (imageSource is FileImageSource)
            {
                using (var stream = new FileStream(((FileImageSource)imageSource).File, FileMode.Open, FileAccess.Read))
                    return await Helpers.ImageHelper.ChangeQuality(stream,0.2f);
            }
            else
                return null;
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            var buttonControl = sender as ButtonControl;
            if (buttonControl==null) return;
            buttonControl.IsInProgress = true;
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                buttonControl.IsInProgress = false;
            }
        }

        private async void CapturePhoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        // save the file into local storage
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);
                        await sourceStream.CopyToAsync(localFileStream);
                        SelectedImage.Source = localFilePath;
                        if (await ValidateForm(true))
                            SaveFuelBill_Clicked(SaveButton, null);
                    }
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private async void PickPhoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var fileResult = await MediaPicker.Default.PickPhotoAsync();
                if (fileResult != null)
                {
                    SelectedImage.Source = fileResult.FullPath;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private async void RequestFuelBill_Clicked(object sender, EventArgs e)
        {
            try
            {
                _fuelBill.Status = FuelBillStatus.Requested;
                if (await ValidateForm(true))
                    SaveFuelBill_Clicked(SaveButton, null);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }
        }

        private async void RemoveFuelBill_Clicked(object sender, EventArgs e)
        {
            try
            {
                _fuelBill.Status = FuelBillStatus.Deleted;
                if (await ValidateForm(true))
                    SaveFuelBill_Clicked(SaveButton, null);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Title.Text, ex.Message, "OK");
            }

        }

        public enum Mode
        {
            Edit,
            AddNew
        }
    }
}