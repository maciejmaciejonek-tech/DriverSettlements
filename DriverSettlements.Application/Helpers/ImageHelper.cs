#if IOS || ANDROID || MACCATALYST
using Microsoft.Maui.Graphics.Platform;
#elif WINDOWS
using Microsoft.Maui.Graphics.Win2D;
#endif
using IImage = Microsoft.Maui.Graphics.IImage;

namespace DriverSettlements.App.Helpers
{
    public class ImageHelper
    {
        public async static Task<byte[]> Downsize(Stream stream, float quality = 1)
        {
            IImage image;
#if IOS || ANDROID || MACCATALYST
            // PlatformImage isn't currently supported on Windows.
            image = PlatformImage.FromStream(stream);
#elif WINDOWS

            image = new W2DImageLoadingService().FromStream(stream);
#endif        
            if (image != null)
            {
                IImage newImage = image.Downsize(800, true);
                return await newImage.AsBytesAsync(ImageFormat.Jpeg, quality);
            }
            return null;
        }

        public async static Task<byte[]> ChangeQuality(Stream stream, float quality = 1) 
        {
            IImage image;
#if IOS || ANDROID || MACCATALYST
            // PlatformImage isn't currently supported on Windows.
            image = PlatformImage.FromStream(stream);
#elif WINDOWS

            image = new W2DImageLoadingService().FromStream(stream);
#endif        
            if (image != null)
            {
                return await image.AsBytesAsync(ImageFormat.Jpeg, quality);
            }
            return null;
        }
    }
}
