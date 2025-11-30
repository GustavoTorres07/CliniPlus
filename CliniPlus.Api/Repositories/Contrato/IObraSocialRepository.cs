using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IObraSocialRepository
    {
        Task<List<ObraSocialDTO>> ListarAsync();

        Task<ObraSocialDTO?> ObtenerPorIdAsync(int id);

        Task<ObraSocialDTO> CrearAsync(ObraSocialDTO dto);

        Task<ObraSocialDTO?> EditarAsync(int id, ObraSocialDTO dto);

        Task<bool> CambiarEstadoAsync(int id, bool isActive);
    }
}
