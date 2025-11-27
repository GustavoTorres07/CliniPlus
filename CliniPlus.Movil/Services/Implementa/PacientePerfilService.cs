using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Net.Http.Json;

namespace CliniPlus.Movil.Services.Implementa
{
    public class PacientePerfilService : IPacientePerfilService
    {
        private readonly IHttpClientFactory _http;

        public PacientePerfilService(IHttpClientFactory http)
        {
            _http = http;
        }

        private HttpClient Client() => _http.CreateClient("ApiCliniPlus");

        public async Task<PerfilPacienteDTO?> ObtenerAsync()
        {
            var resp = await Client().GetAsync("api/PacientePerfil/datos");
            if (!resp.IsSuccessStatusCode) return null;

            return await resp.Content.ReadFromJsonAsync<PerfilPacienteDTO>();
        }

        public async Task<bool> ActualizarAsync(PerfilPacienteDTO dto)
        {
            var resp = await Client().PutAsJsonAsync("api/PacientePerfil/datos", dto);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> CambiarPasswordAsync(CambiarPasswordDTO dto)
        {
            var resp = await Client().PostAsJsonAsync("api/PacientePerfil/cambiar-password", dto);
            return resp.IsSuccessStatusCode;
        }
    }
}
