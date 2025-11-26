using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Net.Http.Json;

namespace CliniPlus.Movil.Services.Implementa
{
    public class PacienteService : IPacienteService
    {
        private readonly IHttpClientFactory _http;

        public PacienteService(IHttpClientFactory http)
        {
            _http = http;
        }

        private HttpClient CreateClient() => _http.CreateClient("ApiCliniPlus");

        public async Task<List<PacienteListDTO>?> ListarAsync(bool incluirInactivos = false)
        {
            var cli = CreateClient();
            var url = $"api/pacientes/listar?incluirInactivos={incluirInactivos.ToString().ToLower()}";

            var res = await cli.GetAsync(url);
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<List<PacienteListDTO>>();
        }

        public async Task<PacienteDetalleDTO?> ObtenerAsync(int id)
        {
            var cli = CreateClient();
            var res = await cli.GetAsync($"api/pacientes/{id}");

            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<PacienteDetalleDTO>();
        }

        public async Task<PacienteDetalleDTO?> CrearAsync(PacienteCrearDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PostAsJsonAsync("api/pacientes/crear", dto);

            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<PacienteDetalleDTO>();
        }

        public async Task<PacienteDetalleDTO?> EditarAsync(int id, PacienteEditarDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PutAsJsonAsync($"api/pacientes/editar/{id}", dto);

            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<PacienteDetalleDTO>();
        }

        public async Task<bool> CambiarEstadoAsync(int id, bool isActive)
        {
            var cli = CreateClient();
            var payload = new PacienteEstadoDTO { IsActive = isActive };

            var res = await cli.PatchAsJsonAsync($"api/pacientes/estado/{id}", payload);
            return res.IsSuccessStatusCode;
        }
    }
}
