using DriverSettlements.App.Controllers;
using DriverSettlements.App.Services;
using DriverSettlements.Common.Interfaces;
using CommunityToolkit.Maui;
using DriverSettlements.App.CustomControls;
using Microsoft.Extensions.Logging;
using Maui.FreakyControls.Extensions;
using zoft.MauiExtensions.Controls;
using Camera.MAUI;

namespace DriverSettlements.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseZoftAutoCompleteEntry()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers((handlers) =>
                {
#if IOS
                    handlers.AddHandler(typeof(ExtendedShell), typeof(DriverSettlements.App.Platforms.iOS.CustomRenderers.iOSExtendedShellRenderer));
#endif
                });

            builder.Services.AddSingleton<ApplicationController>();
            builder.Services.AddSingleton<IHttpsClientHandlerService, HttpsClientHandlerService>();
            builder.Services.AddSingleton<IApplicationInstanceAsync, ApplicationInstanceService>();
            builder.Services.AddSingleton<IMasterAdminAsync, MasterAdminService>();
            builder.Services.AddSingleton<ITenantAsync, TenantService>();
            builder.Services.AddSingleton<IDriverAsync, DriverService>();
#if ANDROID || IOS
            builder.Services.AddSingleton<IDeviceOrientationService, DeviceOrientationService>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();
            ServiceHelper.Initialize(app.Services);
            return app;
        }
    }
}