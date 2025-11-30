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

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Handler que inyecta el token en cada request
        builder.Services.AddTransient<TokenMessageHandler>();

        builder.Services.AddHttpClient("ApiCliniPlus", c =>
        {
            //c.BaseAddress = new Uri("https://CliniPlus.somee.com/");
            c.BaseAddress = new Uri("https://cliniplus.runasp.net");
            c.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<TokenMessageHandler>();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IMedicoService, MedicoService>();
        builder.Services.AddScoped<IEspecialidadService, EspecialidadService>();
        builder.Services.AddScoped<IObraSocialService, ObraSocialService>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        builder.Services.AddScoped<IPacienteService, PacienteService>();
        builder.Services.AddScoped<ICie10Service, Cie10Service>();
        builder.Services.AddScoped<IMedicoPerfilService, MedicoPerfilService>();
        builder.Services.AddScoped<IAdminPerfilService, AdminPerfilService>();
        builder.Services.AddScoped<IPacientePerfilService, PacientePerfilService>();
        builder.Services.AddScoped<ITurnoService, TurnoService>();
        builder.Services.AddScoped<ITipoTurnoService, TipoTurnoService>();
        builder.Services.AddScoped<IFichaMedicaService, FichaMedicaService>();



        return builder.Build();
    }
}
