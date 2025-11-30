using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface IAdminPerfilService
    {
        Task<PerfilAdminDTO?> ObtenerAsync();

        Task<bool> ActualizarAsync(PerfilAdminDTO dto);

        Task<bool> CambiarPasswordAsync(CambiarPasswordDTO dto);

    }
}
