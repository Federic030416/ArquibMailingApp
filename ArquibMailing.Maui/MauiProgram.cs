namespace ArquibMailing.Maui;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ArquibMailing.Application.Interfaces;
using ArquibMailing.Application.UseCases;
using ArquibMailing.Infrastructure.Services;
using ArquibMailing.Maui.Pages;
using ArquibMailing.Maui.ViewModels;



public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                //Aca definimos los alias para usar las fuentes en el XAML
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fonnts.com-Bicyclette_Black 1.otf", "BicycletteBlack");
                fonts.AddFont("fonnts.com-Bicyclette_Bold 1.otf", "BicycletteNormal");
                fonts.AddFont("fonnts.com-Bicyclette_Regular 1.otf", "BicycletteRegular");
                fonts.AddFont("fonnts.com-Bicyclette_Thin 1.otf", "BicycletteItalic");
            });

        // ── Configuración (appsettings.json del backend) ──────────────────
        // Lee el mismo archivo de configuración que usa el backend existente
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        builder.Services.AddSingleton<IConfiguration>(config);

        //  Capa Infrastructure 
        // Aquí registramos las implementaciones concretas.
        // MAUI solo conoce las interfaces (IMailingService, IDestinatarioProvider).
        builder.Services.AddSingleton<IMailingService,        GraphMailingService>();
        builder.Services.AddSingleton<IDestinatarioProvider,  ExcelDestinatarioProvider>();

        //  Capa Application (Casos de Uso) 
        builder.Services.AddTransient<EnviarDocumentoUseCase>();
        builder.Services.AddTransient<ObtenerDestinatariosUseCase>();

        //  Capa Presentación (ViewModels y Pages) 
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<NuevoEnvioViewModel>();
        builder.Services.AddTransient<DestinatariosViewModel>();

        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<NuevoEnvioPage>();
        builder.Services.AddTransient<DestinatariosPage>();
        builder.Services.AddTransient<TemplatesPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

