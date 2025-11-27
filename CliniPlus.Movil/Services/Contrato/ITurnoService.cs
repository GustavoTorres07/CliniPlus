using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface ITurnoService
    {
        // PACIENTE
        Task<List<TurnoListadoPacienteDTO>?> ObtenerMisTurnosAsync();
        Task<bool> ReservarAsync(TurnoReservarDTO dto);
        Task<bool> CancelarAsync(int turnoId);
        Task<List<TurnoSlotDTO>?> ObtenerSlotsDiaAsync(int medicoId, DateTime fechaLocal);

        // MÉDICO
        Task<List<TurnoAgendaMedicoDTO>?> ObtenerAgendaHoyAsync();

        Task<bool> ReservarSlotAsync(TurnoReservarSlotDTO dto);

    }
}
