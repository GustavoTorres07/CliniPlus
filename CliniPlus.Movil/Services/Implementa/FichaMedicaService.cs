using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Net.Http.Json;

namespace CliniPlus.Movil.Services.Implementa
{
    public class FichaMedicaService : IFichaMedicaService
    {
        private readonly IHttpClientFactory _http;

        public FichaMedicaService(IHttpClientFactory http)
        {
            _http = http;
        }

        private HttpClient CreateClient()
            => _http.CreateClient("ApiCliniPlus");

        public async Task<FichaMedicaDTO?> ObtenerAsync(int pacienteId)
        {
            var cli = CreateClient();
            var res = await cli.GetAsync($"api/fichas-medicas/{pacienteId}");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<FichaMedicaDTO>();
        }

        public async Task<bool> GuardarAsync(FichaMedicaDTO dto)
        {
            if (dto.PacienteId <= 0)
                return false;

            var cli = CreateClient();
            var res = await cli.PutAsJsonAsync(
                $"api/fichas-medicas/{dto.PacienteId}", dto);

            return res.IsSuccessStatusCode;
        }

        public async Task<FichaMedicaDTO?> ObtenerMiFichaAsync()
        {
            var cli = CreateClient();
            var res = await cli.GetAsync("api/fichas-medicas/mi-ficha");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<FichaMedicaDTO>();
        }

    }
}
