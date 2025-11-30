using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface ITurnoRepository
    {
        Task<List<TurnoListadoPacienteDTO>> ListarPorPacienteAsync(int pacienteId);

        Task<List<TurnoAgendaMedicoDTO>> ListarAgendaMedicoDiaAsync(int medicoId, DateTime diaUtc, int? pacienteIdActual = null);

        Task<bool> ReservarAsync(int turnoId, int pacienteId, int tipoTurnoId);

        Task<bool> CancelarComoPacienteAsync(int turnoId, int pacienteId);

        Task<List<TurnoSlotDTO>> ListarSlotsPorDiaAsync(int medicoId, DateTime fechaUtc);

        Task<bool> ReservarSlotAsync(int medicoId, DateTime fechaUtc, int pacienteId, int tipoTurnoId);

        Task<List<TurnoAgendaMedicoDTO>> ObtenerAgendaHoyMedicoAsync(int medicoId, DateTime hoyUtc);

        Task<TurnoDetalleMedicoDTO?> ObtenerDetalleMedicoAsync(int medicoId, int turnoId);

        Task<List<HistoriaClinicaItemDTO>> ObtenerHistoriaRapidaPacienteAsync(int medicoId, int pacienteId);

        Task<int?> RegistrarConsultaAsync(int medicoId, RegistrarConsultaMedicoDTO dto);

        Task<bool> MarcarTurnoCompletadoAsync(int medicoId, int turnoId);

        Task<List<PacienteListadoMedicoDTO>> ListarPacientesPorMedicoAsync(int medicoId);

        Task<List<HistoriaClinicaListadoDTO>> ObtenerHistoriaClinicaPacienteAsync(int medicoId, int pacienteId, DateTime? desde, DateTime? hasta);

        Task<HistoriaClinicaDetalleDTO?> ObtenerHistoriaClinicaDetalleAsync(int medicoId, int entradaId);

        Task<PacienteDetalleMedicoDTO?> ObtenerPacienteDetalleAsync(int medicoId, int pacienteId);

        Task<List<HistoriaClinicaListadoDTO>> ObtenerHistoriaPacienteAsync(int pacienteId, DateTime? desde, DateTime? hasta);

        Task<HistoriaClinicaDetalleDTO?> ObtenerHistoriaDetallePacienteAsync(int pacienteId, int entradaId);

        Task<List<TurnoListadoSecretariaDTO>> ListarTurnosHoySecretariaAsync(DateTime hoyLocal);
        Task<bool> CancelarPorSecretariaAsync(int turnoId);

        Task<List<TurnoListadoSecretariaDTO>> ListarAgendaSecretariaAsync(int medicoId, DateTime? desde, DateTime? hasta);


    }
}
