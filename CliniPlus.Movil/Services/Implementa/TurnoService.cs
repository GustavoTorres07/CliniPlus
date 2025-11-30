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

        public async Task<List<TurnoSlotDTO>?> ObtenerSlotsDiaPublicAsync(int medicoId, DateTime fechaLocal)
        {
            var cli = CreateClient();
            var fechaStr = fechaLocal.Date.ToString("yyyy-MM-dd");

            var url = $"api/turnos/public/slots?medicoId={medicoId}&fecha={fechaStr}";
            var res = await cli.GetAsync(url);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<TurnoSlotDTO>>();
        }

        public async Task<bool> ReservarPublicoAsync(TurnoPublicoReservarDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PostAsJsonAsync("api/turnos/public/reservar", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<TurnoDetalleMedicoDTO?> ObtenerDetalleMedicoAsync(int turnoId)
        {
            var cli = CreateClient();
            return await cli.GetFromJsonAsync<TurnoDetalleMedicoDTO>(
                $"api/turnos/medico/detalle/{turnoId}");
        }

        public async Task<List<HistoriaClinicaItemDTO>?> ObtenerHistoriaRapidaPacienteAsync(int pacienteId)
        {
            var cli = CreateClient();
            var res = await cli.GetAsync($"api/turnos/medico/historia-rapida/{pacienteId}");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<HistoriaClinicaItemDTO>>();
        }

        public async Task<bool> RegistrarConsultaAsync(RegistrarConsultaMedicoDTO dto)
        {
            var cli = CreateClient();
            var res = await cli.PostAsJsonAsync("api/turnos/medico/registrar-consulta", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> MarcarTurnoCompletadoAsync(int turnoId)
        {
            var cli = CreateClient();
            var res = await cli.PostAsync($"api/turnos/medico/completar/{turnoId}", content: null);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<PacienteListadoMedicoDTO>?> ObtenerPacientesMedicoAsync()
        {
            var cli = CreateClient();
            var res = await cli.GetAsync("api/turnos/medico/pacientes");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<PacienteListadoMedicoDTO>>();
        }

        public async Task<List<TurnoAgendaMedicoDTO>?> ObtenerAgendaDiaAsync(DateTime fechaLocal)
        {
            var cli = CreateClient();

            string fechaStr = fechaLocal.Date.ToString("yyyy-MM-dd");

            var res = await cli.GetAsync($"api/turnos/medico/agenda-dia?fecha={fechaStr}");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<TurnoAgendaMedicoDTO>>();
        }


        public async Task<PacienteDetalleMedicoDTO?> ObtenerPacienteDetalleAsync(int pacienteId)
        {
            var cli = CreateClient();
            return await cli.GetFromJsonAsync<PacienteDetalleMedicoDTO>(
                $"api/turnos/medico/pacientes/{pacienteId}");
        }

        public async Task<List<HistoriaClinicaListadoDTO>?> ObtenerHistoriaClinicaAsync(
            int pacienteId,
            DateTime? desde,
            DateTime? hasta)
        {
            var cli = CreateClient();

            var query = new List<string>();
            if (desde.HasValue)
                query.Add("desde=" + desde.Value.ToString("yyyy-MM-dd"));
            if (hasta.HasValue)
                query.Add("hasta=" + hasta.Value.ToString("yyyy-MM-dd"));

            var q = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

            var res = await cli.GetAsync($"api/turnos/medico/historia-clinica/{pacienteId}{q}");
            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<HistoriaClinicaListadoDTO>>();
        }

        public async Task<HistoriaClinicaDetalleDTO?> ObtenerHistoriaClinicaDetalleAsync(int entradaId)
        {
            var cli = CreateClient();
            return await cli.GetFromJsonAsync<HistoriaClinicaDetalleDTO>(
                $"api/turnos/medico/historia-clinica/detalle/{entradaId}");
        }


        public async Task<List<HistoriaClinicaListadoDTO>?> ObtenerMiHistoriaClinicaAsync(
    DateTime? desde,
    DateTime? hasta)
        {
            var cli = CreateClient();

            var query = new List<string>();
            if (desde.HasValue)
                query.Add("desde=" + desde.Value.ToString("yyyy-MM-dd"));
            if (hasta.HasValue)
                query.Add("hasta=" + hasta.Value.ToString("yyyy-MM-dd"));

            var q = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

            var res = await cli.GetAsync($"api/turnos/paciente/historia{q}");
            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<HistoriaClinicaListadoDTO>>();
        }


        public async Task<HistoriaClinicaDetalleDTO?> ObtenerMiHistoriaClinicaDetalleAsync(int entradaId)
        {
            var cli = CreateClient();
            return await cli.GetFromJsonAsync<HistoriaClinicaDetalleDTO>(
                $"api/turnos/paciente/historia/{entradaId}");
        }


        public async Task<List<TurnoListadoSecretariaDTO>?> ListarTurnosHoySecretariaAsync()
        {
            var cli = CreateClient();

            var res = await cli.GetAsync("api/turnos/secretaria/turnos-hoy");

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<TurnoListadoSecretariaDTO>>();
        }

        public async Task<bool> CancelarTurnoSecretariaAsync(int turnoId)
        {
            var cli = CreateClient();

            var dto = new TurnoCancelarDTO
            {
                TurnoId = turnoId
            };

            var res = await cli.PostAsJsonAsync("api/turnos/secretaria/cancelar", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<TurnoListadoSecretariaDTO>?> ListarAgendaSecretariaAsync(int medicoId, DateTime? desde, DateTime? hasta)
        {
            var cli = CreateClient();

            if (medicoId <= 0)
                return new List<TurnoListadoSecretariaDTO>();

            var url = $"api/turnos/secretaria/agenda?medicoId={medicoId}";

            if (desde.HasValue)
                url += $"&desde={desde.Value:yyyy-MM-dd}";

            if (hasta.HasValue)
                url += $"&hasta={hasta.Value:yyyy-MM-dd}";

            var res = await cli.GetAsync(url);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<List<TurnoListadoSecretariaDTO>>();
        }


        public async Task<List<HistoriaClinicaListadoDTO>?> ObtenerHistoriaSecretariaAsync(
    int pacienteId,
    DateTime? desde,
    DateTime? hasta)
        {
            var cli = CreateClient();

            var query = new List<string>();
            if (desde.HasValue) query.Add($"desde={desde.Value:yyyy-MM-dd}");
            if (hasta.HasValue) query.Add($"hasta={hasta.Value:yyyy-MM-dd}");

            var url = $"api/turnos/secretaria/historia/{pacienteId}";
            if (query.Count > 0)
                url += "?" + string.Join("&", query);

            var res = await cli.GetAsync(url);
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<List<HistoriaClinicaListadoDTO>>();
        }

        public async Task<HistoriaClinicaDetalleDTO?> ObtenerHistoriaSecretariaDetalleAsync(int pacienteId, int entradaId)
        {
            var cli = CreateClient();
            // 👇 Coincide EXACTO con el controller
            var url = $"api/turnos/secretaria/historia/{pacienteId}/detalle/{entradaId}";

            var res = await cli.GetAsync(url);
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<HistoriaClinicaDetalleDTO>();
        }


    }
}
