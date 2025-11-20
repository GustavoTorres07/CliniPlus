using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Implementa
{
    public class MedicoService : IMedicoService
    {
        private readonly IHttpClientFactory _http;

        public MedicoService(IHttpClientFactory http)
        {
            _http = http;
        }

        public async Task<List<MedicoDetalleDTO>?> GetMedicosAsync()
        {
            var cli = _http.CreateClient("ApiCliniPlus");
            var res = await cli.GetAsync("api/medicos/listar");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<MedicoDetalleDTO>>();
        }
    }
}
