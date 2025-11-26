using CliniPlus.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IMedicoRepository
    {
        // MÉDICOS
        // MÉDICOS
        Task<List<MedicoListadoDTO>> ListarAsync();
        Task<MedicoDetalleDTO?> ObtenerAsync(int id);
        Task<int?> CrearAsync(MedicoCrearDTO dto);
        Task<MedicoDetalleDTO?> EditarAsync(int id, MedicoEditarDTO dto);
        Task<bool> CambiarEstadoAsync(int id, bool activo);
        Task<List<MedicoListadoDTO>> ListarPorEspecialidadAsync(int especialidadId);

        // HORARIOS
        Task<List<MedicoHorarioDTO>> ListarHorariosAsync(int medicoId);
        Task<MedicoHorarioDTO?> CrearHorarioAsync(int medicoId, MedicoHorarioDTO dto);
        Task<MedicoHorarioDTO?> EditarHorarioAsync(int medicoId, int idHorario, MedicoHorarioDTO dto);
        Task<bool> EliminarHorarioAsync(int medicoId, int idHorario);

        // BLOQUEOS (ya los tenés, los dejamos)
        Task<List<MedicoBloqueoDTO>> ListarBloqueosAsync(int medicoId);
        Task<MedicoBloqueoDTO?> CrearBloqueoAsync(int medicoId, MedicoBloqueoDTO dto);
        Task<bool> EliminarBloqueoAsync(int medicoId, int idBloqueo);
    }
}
