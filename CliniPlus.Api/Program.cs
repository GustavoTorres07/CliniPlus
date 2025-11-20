// ---------------------------------------------------------------
// 1. AÑADIDOS LOS USINGS PARA MANEJAR CLAIMS Y JWT
// ---------------------------------------------------------------
using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Api.Repositories.Implementa;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// CORS (Somee + clientes)
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Default", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// ---------------------------------------------------------------
// 2. AÑADIDA LÍNEA PARA EVITAR EL RENOMBRAMIENTO DE CLAIMS
//    Debe ir ANTES de AddAuthentication
// ---------------------------------------------------------------
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Somee puede ser http
        options.SaveToken = true;

        // ---------------------------------------------------------------
        // 3. AÑADIDA OPCIÓN PARA EVITAR EL MAPEADO (redundante pero claro)
        // ---------------------------------------------------------------
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromMinutes(2),

            // ---------------------------------------------------------------
            // 4. AÑADIDAS LÍNEAS PARA INDICAR DÓNDE LEER EL NOMBRE Y ROL
            //    Esto es clave para que [Authorize(Role = "...")] funcione.
            // ---------------------------------------------------------------
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", p => p.RequireRole("Administrador"));
    opt.AddPolicy("SoloMedico", p => p.RequireRole("Medico"));
    opt.AddPolicy("MedicoOAdmin", p => p.RequireRole("Medico", "Administrador"));
    opt.AddPolicy("SecretariaOAdmin", p => p.RequireRole("Secretario", "Administrador"));
});

builder.Services.AddControllers();

// Servicios propios
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<IEspecialidadRepository, EspecialidadRepository>(); 



var app = builder.Build();

app.UseCors("Default");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();