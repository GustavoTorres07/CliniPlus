using CliniPlus.Movil;
using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Movil.Services.Http;
using CliniPlus.Movil.Services.Implementa;
using Microsoft.Extensions.Logging;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Blazor WebView
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Handler que inyecta el token en cada request
        builder.Services.AddSingleton<TokenMessageHandler>();

        // HttpClient nombrado para la API (Somee)
        builder.Services.AddHttpClient("ApiCliniPlus", c =>
        {
            c.BaseAddress = new Uri("https://CliniPlus.somee.com/");
            c.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<TokenMessageHandler>();

        // Servicios del front (Contrato / Implementa)
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();

        return builder.Build();
    }
}
