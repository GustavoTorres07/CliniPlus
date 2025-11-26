using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Implementa
{
    public class Cie10Service : ICie10Service
    {
        private readonly IHttpClientFactory _http;

        public Cie10Service(IHttpClientFactory http)
        {
            _http = http;
        }

        private HttpClient CreateClient() => _http.CreateClient("ApiCliniPlus");

        public async Task<List<Cie10DTO>?> BuscarAsync(string? filtro)
        {
            var cli = CreateClient();

            string url = "api/cie10";
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                url += $"?q={System.Net.WebUtility.UrlEncode(filtro)}";
            }

            var res = await cli.GetAsync(url);
            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<Cie10DTO>>();
        }

        public async Task<Cie10DTO?> CrearAsync(Cie10CrearDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PostAsJsonAsync("api/cie10", dto);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<Cie10DTO>();
        }

        public async Task<Cie10DTO?> EditarAsync(string codigo, Cie10EditarDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PutAsJsonAsync($"api/cie10/{codigo}", dto);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<Cie10DTO>();
        }

        public async Task<bool> EliminarAsync(string codigo)
        {
            var cli = CreateClient();
            var res = await cli.DeleteAsync($"api/cie10/{codigo}");
            return res.IsSuccessStatusCode;
        }
    }
}
