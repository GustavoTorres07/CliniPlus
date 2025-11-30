using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IPacienteRepository
    {
        Task<List<PacienteListDTO>> ListarAsync(bool incluirInactivos = false);

        Task<PacienteDetalleDTO?> ObtenerPorIdAsync(int id);

        Task<PacienteDetalleDTO> CrearAsync(PacienteCrearDTO dto);

        Task<PacienteDetalleDTO?> EditarAsync(int id, PacienteEditarDTO dto);

        Task<bool> CambiarEstadoAsync(int id, bool isActive);

        Task<List<PacienteListadoDTO>> ListarParaSecretariaAsync();

        Task<bool> ActivarCuentaProvisionalAsync(PacienteActivarCuentaDTO dto);

        Task<int?> ObtenerIdPorUsuarioAsync(int usuarioId);

    }
}
