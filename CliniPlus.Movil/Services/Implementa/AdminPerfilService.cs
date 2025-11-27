using System.Net.Http.Json;
using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;

namespace CliniPlus.Movil.Services.Implementa
{
    public class AdminPerfilService : IAdminPerfilService
    {
        private readonly IHttpClientFactory _http;

        public AdminPerfilService(IHttpClientFactory http)
        {
            _http = http;
        }

        private HttpClient CreateClient() => _http.CreateClient("ApiCliniPlus");

        public Task<PerfilAdminDTO?> ObtenerAsync()
        {
            return CreateClient().GetFromJsonAsync<PerfilAdminDTO>("api/AdminPerfil/datos");
        }

        public async Task<bool> ActualizarAsync(PerfilAdminDTO dto)
        {
            var res = await CreateClient().PutAsJsonAsync("api/AdminPerfil/datos", dto);
            return res.IsSuccessStatusCode;
        }
        public async Task<bool> CambiarPasswordAsync(CambiarPasswordDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PutAsJsonAsync("api/AdminPerfil/password", dto);
            return res.IsSuccessStatusCode;
        }
    }
}
