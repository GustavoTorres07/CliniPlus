using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Net.Http.Json;

namespace CliniPlus.Movil.Services.Implementa
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly IHttpClientFactory _http;

        public EspecialidadService(IHttpClientFactory http)
        {
            _http = http;
        }

        public async Task<List<EspecialidadDTO>?> ListarAsync()
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.GetAsync("api/especialidades/listar");
            return res.IsSuccessStatusCode
                ? await res.Content.ReadFromJsonAsync<List<EspecialidadDTO>>()
                : null;
        }

        public async Task<EspecialidadDTO?> ObtenerAsync(int id)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.GetAsync($"api/especialidades/{id}");
            return res.IsSuccessStatusCode
                ? await res.Content.ReadFromJsonAsync<EspecialidadDTO>()
                : null;
        }

        public async Task<EspecialidadDTO?> CrearAsync(EspecialidadDTO dto)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.PostAsJsonAsync("api/especialidades/crear", dto);
            return res.IsSuccessStatusCode
                ? await res.Content.ReadFromJsonAsync<EspecialidadDTO>()
                : null;
        }

        public async Task<EspecialidadDTO?> EditarAsync(int id, EspecialidadDTO dto)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.PutAsJsonAsync($"api/especialidades/editar/{id}", dto);
            return res.IsSuccessStatusCode
                ? await res.Content.ReadFromJsonAsync<EspecialidadDTO>()
                : null;
        }

        public async Task<bool> CambiarEstadoAsync(int id, bool isActive)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.PatchAsJsonAsync(
                $"api/especialidades/estado/{id}",
                new { IsActive = isActive });

            return res.IsSuccessStatusCode;
        }
    }
}
