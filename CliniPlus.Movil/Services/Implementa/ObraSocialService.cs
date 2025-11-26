using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Net.Http.Json;

namespace CliniPlus.Movil.Services.Implementa
{
    public class ObraSocialService : IObraSocialService
    {
        private readonly IHttpClientFactory _http;

        public ObraSocialService(IHttpClientFactory http)
        {
            _http = http;
        }

        public async Task<List<ObraSocialDTO>?> ListarAsync()
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.GetAsync("api/obras-sociales/listar");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<ObraSocialDTO>>();
        }

        public async Task<ObraSocialDTO?> ObtenerAsync(int id)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.GetAsync($"api/obras-sociales/{id}");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<ObraSocialDTO>();
        }

        public async Task<ObraSocialDTO?> CrearAsync(ObraSocialDTO dto)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.PostAsJsonAsync("api/obras-sociales/crear", dto);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<ObraSocialDTO>();
        }

        public async Task<ObraSocialDTO?> EditarAsync(int id, ObraSocialDTO dto)
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.PutAsJsonAsync($"api/obras-sociales/editar/{id}", dto);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<ObraSocialDTO>();
        }

        public async Task<bool> CambiarEstadoAsync(int id, bool isActive)
        {
            var cli = _http.CreateClient("ApiCliniPlus");

            var body = new ObraSocialEstadoDTO { IsActive = isActive };

            var res = await cli.PatchAsJsonAsync($"api/obras-sociales/estado/{id}", body);

            return res.IsSuccessStatusCode;
        }
    }
}
