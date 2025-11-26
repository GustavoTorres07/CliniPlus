using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface IUsuarioService
    {
        Task<List<UsuarioListDTO>?> ListarAsync();
        Task<UsuarioDetalleDTO?> ObtenerAsync(int id);
        Task<UsuarioDetalleDTO?> CrearAsync(UsuarioCrearDTO dto);
        Task<UsuarioDetalleDTO?> EditarAsync(int id, UsuarioEditarDTO dto);
        Task<bool> CambiarEstadoAsync(int id, bool activo);
        Task<bool> CambiarPasswordAdminAsync(int idUsuario, string nuevaPassword);

    }
}
