using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IAdminPerfilRepository
    {
        Task<PerfilAdminDTO?> ObtenerAsync(int idUsuario);

        Task<bool> ActualizarAsync(int idUsuario, PerfilAdminDTO dto);

        Task<bool> CambiarPasswordAsync(int idUsuario, CambiarPasswordDTO dto);

    }
}
