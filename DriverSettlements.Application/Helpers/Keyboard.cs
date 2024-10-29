using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if (IOS || MACCATALYST)
using UIKit;
#endif
#if ANDROID
using Android.Views.InputMethods;
using Android.Content;

#endif

namespace DriverSettlements.App.Helpers
{
    public class Keyboard
    {
        public static void HideKeyboard()
        {
#if ANDROID
    {
        var context = Platform.AppContext;
        var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
        if (inputMethodManager != null)
        {
            var activity = Platform.CurrentActivity;
            if (activity != null)
            {
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
            }

            if (activity?.Window != null) activity.Window.DecorView.ClearFocus();
        }
    }
#elif (IOS || MACCATALYST)
            {
                if (UIApplication.SharedApplication?.KeyWindow != null) UIApplication.SharedApplication.KeyWindow.EndEditing(true);
            }
#endif
        }
    }
}
