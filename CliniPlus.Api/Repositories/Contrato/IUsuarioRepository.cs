using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IUsuarioRepository
    {
        Task<List<UsuarioListDTO>> ListarAsync();

        Task<UsuarioDetalleDTO?> ObtenerPorIdAsync(int id);

        Task<UsuarioDetalleDTO> CrearAsync(UsuarioCrearDTO dto);

        Task<UsuarioDetalleDTO?> EditarAsync(int id, UsuarioEditarDTO dto);

        Task<bool> CambiarEstadoAsync(int id, bool isActive);

        Task<bool> CambiarPasswordAdminAsync(int id, string nuevaPassword);

    }
}
