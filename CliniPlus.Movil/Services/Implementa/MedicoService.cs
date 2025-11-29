using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Collections.Generic;
using System.Net.Http.Json;
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

        private HttpClient CreateClient() => _http.CreateClient("ApiCliniPlus");

        // ================= MÉDICOS =================

        public async Task<List<MedicoListadoDTO>?> ListarAsync(int? especialidadId = null)
        {
            var cli = CreateClient();

            string url = "api/medicos/publico";

            if (especialidadId.HasValue && especialidadId.Value > 0)
                url += $"?especialidadId={especialidadId.Value}";

            var res = await cli.GetAsync(url);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<MedicoListadoDTO>>();
        }

        public async Task<MedicoDetalleDTO?> ObtenerAsync(int id)
        {
            var cli = CreateClient();
            var res = await cli.GetAsync($"api/medicos/{id}");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<MedicoDetalleDTO>();
        }

        public async Task<MedicoDetalleDTO?> CrearAsync(MedicoCrearDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PostAsJsonAsync("api/medicos", dto);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<MedicoDetalleDTO>();
        }

        public async Task<MedicoDetalleDTO?> EditarAsync(int id, MedicoEditarDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PutAsJsonAsync($"api/medicos/{id}", dto);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<MedicoDetalleDTO>();
        }

        public async Task<bool> CambiarEstadoAsync(int id, bool activo)
        {
            var cli = CreateClient();
            var body = new MedicoEstadoDTO { IsActive = activo };
            var res = await cli.PatchAsJsonAsync($"api/medicos/{id}/estado", body);
            return res.IsSuccessStatusCode;
        }

        // ================= HORARIOS =================

        public async Task<List<MedicoHorarioDTO>?> ListarHorariosAsync(int medicoId)
        {
            var cli = CreateClient();
            var res = await cli.GetAsync($"api/medicos/{medicoId}/horarios");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<MedicoHorarioDTO>>();
        }

        public async Task<MedicoHorarioDTO?> CrearHorarioAsync(int medicoId, MedicoHorarioDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PostAsJsonAsync($"api/medicos/{medicoId}/horarios", dto);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<MedicoHorarioDTO>();
        }

        public async Task<MedicoHorarioDTO?> EditarHorarioAsync(int medicoId, int idHorario, MedicoHorarioDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PutAsJsonAsync($"api/medicos/{medicoId}/horarios/{idHorario}", dto);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<MedicoHorarioDTO>();
        }

        public async Task<bool> EliminarHorarioAsync(int medicoId, int idHorario)
        {
            var cli = CreateClient();
            var res = await cli.DeleteAsync($"api/medicos/{medicoId}/horarios/{idHorario}");
            return res.IsSuccessStatusCode;
        }

        // ================= BLOQUEOS =================

        public async Task<List<MedicoBloqueoDTO>?> ListarBloqueosAsync(int medicoId)
        {
            var cli = CreateClient();
            var res = await cli.GetAsync($"api/medicos/{medicoId}/bloqueos");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<MedicoBloqueoDTO>>();
        }

        public async Task<MedicoBloqueoDTO?> CrearBloqueoAsync(int medicoId, MedicoBloqueoDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PostAsJsonAsync($"api/medicos/{medicoId}/bloqueos", dto);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<MedicoBloqueoDTO>();
        }

        public async Task<bool> EliminarBloqueoAsync(int medicoId, int idBloqueo)
        {
            var cli = CreateClient();
            var res = await cli.DeleteAsync($"api/medicos/{medicoId}/bloqueos/{idBloqueo}");
            return res.IsSuccessStatusCode;
        }

        public async Task<List<MedicoDisponiblePacienteDTO>> ObtenerMedicosParaPacienteAsync(
            int? especialidadId = null,
            string? q = null)
        {
            var cli = CreateClient();         // 👈 cliente real

            var url = "api/medicos/paciente/medicos";

            var queryParams = new List<string>();

            if (especialidadId.HasValue)
                queryParams.Add($"especialidadId={especialidadId.Value}");

            if (!string.IsNullOrWhiteSpace(q))
                queryParams.Add($"q={q}");

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            // 👇 ahora sí usamos GetFromJsonAsync sobre HttpClient
            return await cli.GetFromJsonAsync<List<MedicoDisponiblePacienteDTO>>(url)
                   ?? new List<MedicoDisponiblePacienteDTO>();
        }
    }
}
