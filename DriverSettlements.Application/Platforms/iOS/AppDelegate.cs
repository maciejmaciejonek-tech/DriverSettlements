using DriverSettlements.App.Platforms.iOS;
using Foundation;
using UIKit;

namespace DriverSettlements.App
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

/*
        [Export("application:supportedInterfaceOrientationsForWindow:")]
        public UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            return OrientationManager.LockOrientation
                ? OrientationManager.CurrentOrientation
                : UIInterfaceOrientationMask.All;
        }
*/
    }
}
