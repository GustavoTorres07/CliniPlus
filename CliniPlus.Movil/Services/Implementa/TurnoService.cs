using CliniPlus.Movil.Services.Contrato;
using CliniPlus.Shared.DTOs;
using System.Net.Http.Json;

namespace CliniPlus.Movil.Services.Implementa
{
    public class TurnoService : ITurnoService
    {
        private readonly IHttpClientFactory _http;

        public TurnoService(IHttpClientFactory http)
        {
            _http = http;
        }

        private HttpClient CreateClient() => _http.CreateClient("ApiCliniPlus");

        // ============================================================
        // PACIENTE
        // ============================================================

        public async Task<List<TurnoListadoPacienteDTO>?> ObtenerMisTurnosAsync()
        {
            var cli = CreateClient();
            var res = await cli.GetAsync("api/turnos/paciente/mis-turnos");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<TurnoListadoPacienteDTO>>();
        }

        public async Task<bool> ReservarAsync(TurnoReservarDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PostAsJsonAsync("api/turnos/paciente/reservar", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> CancelarAsync(int turnoId)
        {
            var cli = CreateClient();

            var body = new TurnoCancelarDTO { TurnoId = turnoId };

            var res = await cli.PostAsJsonAsync("api/turnos/paciente/cancelar", body);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<TurnoSlotDTO>?> ObtenerSlotsDiaAsync(int medicoId, DateTime fechaLocal)
        {
            var cli = CreateClient();

            string fechaStr = fechaLocal.Date.ToString("yyyy-MM-dd");

            var url = $"api/turnos/paciente/slots?medicoId={medicoId}&fecha={fechaStr}";
            var res = await cli.GetAsync(url);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<TurnoSlotDTO>>();
        }

        // ============================================================
        // MÉDICO
        // ============================================================

        public async Task<List<TurnoAgendaMedicoDTO>?> ObtenerAgendaHoyAsync()
        {
            var cli = CreateClient();
            var res = await cli.GetAsync("api/turnos/medico/agenda-hoy");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<TurnoAgendaMedicoDTO>>();
        }

        public async Task<bool> ReservarSlotAsync(TurnoReservarSlotDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PostAsJsonAsync("api/turnos/paciente/reservar-slot", dto);
            return res.IsSuccessStatusCode;
        }

    }
}
