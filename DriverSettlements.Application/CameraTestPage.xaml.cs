
using DriverSettlements.App.Services;

namespace DriverSettlements.App;

public partial class CameraTestPage : ContentPage
{
    private readonly IDeviceOrientationService _deviceOrientationService;

    private int i=-1;
	public CameraTestPage(IDeviceOrientationService deviceOrientationService)
	{
		InitializeComponent();
        _deviceOrientationService = deviceOrientationService;
        Appearing += CameraTestPage_Appearing;
        Disappearing += CameraTestPage_Disappearing;
    }

    private void CameraTestPage_Appearing(object? sender, EventArgs e)
    {
        Button_Clicked(null, null);
    }

    private void CameraTestPage_Disappearing(object? sender, EventArgs e)
    {
        _deviceOrientationService.UnlockOrientation();
    }

    private void CameraView_CamerasLoaded(object sender, EventArgs e)
    {
		CameraView.Camera = CameraView.Cameras.First();
		MainThread.BeginInvokeOnMainThread(async () =>
		{
            await CameraView.StopCameraAsync();
            await CameraView.StartCameraAsync();
		});
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
		i++;
        if (i > 5)
            i = 0;
		switch(i)
		{
            case 0:
                MaskImage.Source = "licznik.png";
                Title = "Ewidencja przebiegu samochodu";
                _deviceOrientationService.SetDeviceOrientation(DisplayOrientation.Landscape);
                break;
			case 1:
				MaskImage.Source = "poziom_oleju.png";
                Title = "Weryfikacja poziomu oleju";
                _deviceOrientationService.SetDeviceOrientation(DisplayOrientation.Portrait);
                break;
            case 2:
                MaskImage.Source = "auto1.png";
                Title = "Dokumentacja profilu samochodu - prawy przód";
                _deviceOrientationService.SetDeviceOrientation(DisplayOrientation.Landscape);
                break;
            case 3:
                MaskImage.Source = "auto2.png";
                Title = "Dokumentacja profilu samochodu - lewy przód";
                _deviceOrientationService.SetDeviceOrientation(DisplayOrientation.Landscape);
                break;
            case 4:
                MaskImage.Source = "auto3.png";
                Title = "Dokumentacja profilu samochodu - lewy tył";
                _deviceOrientationService.SetDeviceOrientation(DisplayOrientation.Landscape);
                break;
            case 5:
                MaskImage.Source = "auto4.png";
                Title = "Dokumentacja profilu samochodu - prawy tył";
                _deviceOrientationService.SetDeviceOrientation(DisplayOrientation.Landscape);
                break;
        }
    }
}