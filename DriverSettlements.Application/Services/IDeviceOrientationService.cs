namespace DriverSettlements.App.Services
{
    public interface IDeviceOrientationService
    {
        void SetDeviceOrientation(DisplayOrientation displayOrientation);
        void UnlockOrientation();
    }
}
