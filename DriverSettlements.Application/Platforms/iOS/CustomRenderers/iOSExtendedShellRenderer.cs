using CoreGraphics;
using DriverSettlements.App.CustomControls;
using DriverSettlements.App.Platforms.iOS.CustomRenderers;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

[assembly: ExportRenderer(typeof(ExtendedShell), typeof(iOSExtendedShellRenderer))]
namespace DriverSettlements.App.Platforms.iOS.CustomRenderers
{
    public class iOSExtendedShellRenderer:ShellRenderer
    {
        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {
            return new CustomShellSectionRenderer(this);
        }

        private class CustomShellSectionRenderer : ShellSectionRenderer
        {
            public CustomShellSectionRenderer(IShellContext context) : base(context)
            {
            }

            protected override void UpdateTabBarItem()
            {
                base.UpdateTabBarItem();
                //TODO: Calculate the size according the screen.
                //According to Apple:
                //@1x: 48x32
                //@2x: 96x64
                //@3x: 144x96
                TabBarItem.Image = ResizeImage(TabBarItem.Image, 48, 32);
            }

            private UIImage ResizeImage(UIImage source, float width, float height)
            {
                //If source is null, return null
                if (source == null)
                    return null;

                //Calculate the resize factor
                CGSize sourceSize = source.Size;
                double resizeFactor = Math.Min(width / sourceSize.Width, height / sourceSize.Height);
                //If the resize factor is greater than 1 (i.e. the Width and Height are bigger than source's size), then DON'T rescale
                if (resizeFactor > 1)
                    return source;
                //Calculate the new size
                double resizeWidth = sourceSize.Width * resizeFactor;
                double resizeHeight = sourceSize.Height * resizeFactor;
                //NOTE: DON'T use UIGraphics.BeginImageContext, otherwise the final result will lose some quality.
                UIGraphics.BeginImageContextWithOptions(new CGSize(resizeWidth, resizeHeight), false, 0.0f);
                //Redraw the image in the new size
                source.Draw(new CGRect(0, 0, resizeWidth, resizeHeight));
                //Save the new image for later
                UIImage scaledImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                //Take some coffe
                return scaledImage;
            }
        }
    }
}
