using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IMedicoPerfilRepository
    {
        Task<PerfilMedicoDTO?> ObtenerAsync(int idUsuario);

        Task<bool> ActualizarAsync(int idUsuario, PerfilMedicoDTO dto);
    }
}
