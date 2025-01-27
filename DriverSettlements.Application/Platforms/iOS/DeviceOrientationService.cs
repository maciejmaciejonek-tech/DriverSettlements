using DriverSettlements.App.Services;
using Foundation;
using Microsoft.Maui.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace DriverSettlements.App
{
    public class DeviceOrientationService : IDeviceOrientationService
    {
        private readonly IReadOnlyDictionary<DisplayOrientation, UIInterfaceOrientation> _iosDisplayOrientationMap =
           new Dictionary<DisplayOrientation, UIInterfaceOrientation>
           {
               [DisplayOrientation.Landscape] = UIInterfaceOrientation.LandscapeLeft,
               [DisplayOrientation.Portrait] = UIInterfaceOrientation.Portrait,
           };

        public void SetDeviceOrientation(DisplayOrientation displayOrientation)
        {
            OrientationManager.LockOrientation = true;
            if (displayOrientation == DisplayOrientation.Portrait)
            {
                OrientationManager.CurrentOrientation = UIInterfaceOrientationMask.Portrait;
            }
            else
            {
                OrientationManager.CurrentOrientation = UIInterfaceOrientationMask.Landscape;
            }

            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)_iosDisplayOrientationMap[displayOrientation]), new NSString("orientation"));
        }

        public void UnlockOrientation()
        {
            OrientationManager.LockOrientation = false;
            OrientationManager.CurrentOrientation = UIInterfaceOrientationMask.AllButUpsideDown;
            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)_iosDisplayOrientationMap[DisplayOrientation.Unknown]), new NSString("orientation"));
        }
    }

    public static class OrientationManager
    {
        public static bool LockOrientation { get; set; }
        public static UIInterfaceOrientationMask CurrentOrientation { get; set; } = UIInterfaceOrientationMask.All;
    }
}
