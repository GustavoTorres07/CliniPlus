using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IPacienteRepository
    {
        Task<List<PacienteListDTO>> ListarAsync(bool incluirInactivos = false);
        Task<PacienteDetalleDTO?> ObtenerPorIdAsync(int id);

        /// <summary>
        /// Crea un paciente (puede ser provisional o ya vinculado a Usuario).
        /// Lanza InvalidOperationException("DNI_EXISTE") si el DNI ya está usado.
        /// </summary>
        Task<PacienteDetalleDTO> CrearAsync(PacienteCrearDTO dto);

        /// <summary>
        /// Edita datos del paciente.
        /// Lanza InvalidOperationException("DNI_EXISTE") si el DNI queda duplicado.
        /// </summary>
        Task<PacienteDetalleDTO?> EditarAsync(int id, PacienteEditarDTO dto);

        /// <summary>
        /// Cambia el estado activo/inactivo.
        /// Devuelve false si no se encontró el paciente.
        /// </summary>
        Task<bool> CambiarEstadoAsync(int id, bool isActive);

        Task<List<PacienteListadoDTO>> ListarParaSecretariaAsync();

        Task<bool> ActivarCuentaProvisionalAsync(PacienteActivarCuentaDTO dto);

        Task<int?> ObtenerIdPorUsuarioAsync(int usuarioId);

    }
}
