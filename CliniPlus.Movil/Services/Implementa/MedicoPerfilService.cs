using System.Net.Http.Json;
using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;

namespace CliniPlus.Movil.Services.Implementa
{
    public class MedicoPerfilService : IMedicoPerfilService
    {
        private readonly IHttpClientFactory _http; 

        public MedicoPerfilService(IHttpClientFactory http) 
        {
            _http = http;
        }

        private HttpClient CreateClient() => _http.CreateClient("ApiCliniPlus"); 

        public async Task<PerfilMedicoDTO?> ObtenerAsync()
        {
            var cli = CreateClient(); 

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
            var cli = CreateClient(); 

            var resp = await cli.PutAsJsonAsync("api/MedicoPerfil/datos", dto);

            return resp.IsSuccessStatusCode;
        }
    }
}