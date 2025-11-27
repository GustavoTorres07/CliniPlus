using System.Net.Http.Json;
using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;

namespace CliniPlus.Movil.Services.Implementa
{
    public class MedicoPerfilService : IMedicoPerfilService
    {
        private readonly IHttpClientFactory _http; // ← Cambio aquí

        public MedicoPerfilService(IHttpClientFactory http) // ← Cambio aquí
        {
            _http = http;
        }

        private HttpClient CreateClient() => _http.CreateClient("ApiCliniPlus"); // ← Añadir esto

        public async Task<PerfilMedicoDTO?> ObtenerAsync()
        {
            var cli = CreateClient(); // ← Usar esto
            var res = await cli.GetAsync("api/MedicoPerfil/datos");

            Console.WriteLine("STATUS PERFIL: " + res.StatusCode);

            if (!res.IsSuccessStatusCode)
            {
                var txt = await res.Content.ReadAsStringAsync();
                Console.WriteLine("ERROR PERFIL: " + txt);
                return null;
            }

            return await res.Content.ReadFromJsonAsync<PerfilMedicoDTO>();
        }

        public async Task<bool> ActualizarAsync(PerfilMedicoDTO dto)
        {
            var cli = CreateClient(); // ← Usar esto
            var resp = await cli.PutAsJsonAsync("api/MedicoPerfil/datos", dto);
            return resp.IsSuccessStatusCode;
        }
    }
}