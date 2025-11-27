using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface ITurnoRepository
    {
        Task<List<TurnoListadoPacienteDTO>> ListarPorPacienteAsync(int pacienteId);

        Task<List<TurnoAgendaMedicoDTO>> ListarAgendaMedicoDiaAsync(
            int medicoId,
            DateTime diaUtc,
            int? pacienteIdActual = null);

        Task<bool> ReservarAsync(int turnoId, int pacienteId, int tipoTurnoId);

        Task<bool> CancelarComoPacienteAsync(int turnoId, int pacienteId);

        // ================= NUEVO =================
        /// <summary>
        /// Devuelve la grilla de slots (turnos / bloqueos / huecos) 
        /// para un médico en un día concreto.
        /// </summary>
        Task<List<TurnoSlotDTO>> ListarSlotsPorDiaAsync(int medicoId, DateTime fechaUtc);

        Task<bool> ReservarSlotAsync(int medicoId, DateTime fechaUtc, int pacienteId, int tipoTurnoId);

    }
}
