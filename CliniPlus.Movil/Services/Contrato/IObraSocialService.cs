using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface IObraSocialService
    {
        Task<List<ObraSocialDTO>?> ListarAsync();

        Task<ObraSocialDTO?> ObtenerAsync(int id);

        Task<ObraSocialDTO?> CrearAsync(ObraSocialDTO dto);

        Task<ObraSocialDTO?> EditarAsync(int id, ObraSocialDTO dto);

        Task<bool> CambiarEstadoAsync(int id, bool isActive);
    }
}
