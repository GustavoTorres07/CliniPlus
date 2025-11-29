using CliniPlus.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface IMedicoService
    {
        // MÉDICOS
        Task<List<MedicoListadoDTO>?> ListarAsync(int? especialidadId = null);
        Task<MedicoDetalleDTO?> ObtenerAsync(int id);
        Task<MedicoDetalleDTO?> CrearAsync(MedicoCrearDTO dto);
        Task<MedicoDetalleDTO?> EditarAsync(int id, MedicoEditarDTO dto);
        Task<bool> CambiarEstadoAsync(int id, bool activo);


        // HORARIOS
        Task<List<MedicoHorarioDTO>?> ListarHorariosAsync(int medicoId);
        Task<MedicoHorarioDTO?> CrearHorarioAsync(int medicoId, MedicoHorarioDTO dto);
        Task<MedicoHorarioDTO?> EditarHorarioAsync(int medicoId, int idHorario, MedicoHorarioDTO dto);
        Task<bool> EliminarHorarioAsync(int medicoId, int idHorario);

        // BLOQUEOS (ya existen)
        Task<List<MedicoBloqueoDTO>?> ListarBloqueosAsync(int medicoId);
        Task<MedicoBloqueoDTO?> CrearBloqueoAsync(int medicoId, MedicoBloqueoDTO dto);
        Task<bool> EliminarBloqueoAsync(int medicoId, int idBloqueo);

        Task<List<MedicoDisponiblePacienteDTO>> ObtenerMedicosParaPacienteAsync(
    int? especialidadId = null,
    string? q = null);
    }
}
