using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IPacientePerfilRepository
    {
        Task<PerfilPacienteDTO?> ObtenerAsync(int idUsuario);

        Task<bool> ActualizarAsync(int idUsuario, PerfilPacienteDTO dto);

        Task<bool> CambiarPasswordAsync(int idUsuario, string passwordActual, string passwordNueva);
    }
}
