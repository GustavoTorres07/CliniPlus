using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IEspecialidadRepository
    {
        Task<List<EspecialidadDTO>> ListarAsync();

        Task<EspecialidadDTO?> ObtenerPorIdAsync(int id);

        Task<EspecialidadDTO> CrearAsync(EspecialidadDTO dto);

        Task<EspecialidadDTO?> EditarAsync(int id, EspecialidadDTO dto);

        Task<bool> CambiarEstadoAsync(int id, bool isActive);
    }
}
