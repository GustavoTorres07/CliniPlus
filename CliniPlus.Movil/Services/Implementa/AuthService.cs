using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Implementa
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _http;
        private readonly ITokenService _token;

        public AuthService(IHttpClientFactory http, ITokenService tokenStorage)
        {
            _http = http;
            _token = tokenStorage;
        }

        public async Task<AuthLoginResponseDTO?> LoginAsync(AuthLoginRequest req, bool recordar)
        {
            // Para login no necesitamos el handler que agrega token. Usamos el mismo client (no hay token todavía).
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.PostAsJsonAsync("api/auth/login", req);
            if (!res.IsSuccessStatusCode) return null;

            var payload = await res.Content.ReadFromJsonAsync<AuthLoginResponseDTO>();
            if (payload != null && !string.IsNullOrWhiteSpace(payload.Token))
                await _token.GuardarAsync(payload.Token, recordar);

            return payload;
        }

        public async Task<AuthMeResponse?> MeAsync()
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.GetAsync("api/auth/me");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<AuthMeResponse>();
        }

        public async Task LogoutAsync() => await _token.BorrarAsync();

        public async Task<bool> PingAsync()
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.GetAsync("api/auth/ping");
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> RecuperarPasswordAsync(string email)
        {
            var http = _http.CreateClient("ApiCliniPlus");

            var body = new { Email = email };

            var resp = await http.PostAsJsonAsync("api/auth/recuperar-password", body);

            return resp.IsSuccessStatusCode;
        }

    }
}
