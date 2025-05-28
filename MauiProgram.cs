using Microsoft.Extensions.Logging;
using Plugin.Maui.OCR;
using Syncfusion.Maui.Core.Hosting;
using Microcharts.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace app_s8
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseMicrocharts()
                .UseOcr()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
