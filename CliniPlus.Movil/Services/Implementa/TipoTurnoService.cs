// CliniPlus.Movil/Services/Implementa/TipoTurnoService.cs
using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Net.Http.Json;

namespace CliniPlus.Movil.Services.Implementa
{
    public class TipoTurnoService : ITipoTurnoService
    {
        private readonly IHttpClientFactory _http;

        public TipoTurnoService(IHttpClientFactory http)
        {
            _http = http;
        }

        private HttpClient CreateClient() => _http.CreateClient("ApiCliniPlus");

        public async Task<List<TipoTurnoDTO>?> ListarAsync()
        {
            var cli = CreateClient();

            var res = await cli.GetAsync("api/tipoturnos");

            if (!res.IsSuccessStatusCode)

                return null;

            return await res.Content.ReadFromJsonAsync<List<TipoTurnoDTO>>();
        }
    }
}
