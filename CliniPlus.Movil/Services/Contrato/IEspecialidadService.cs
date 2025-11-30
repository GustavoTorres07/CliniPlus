using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface IEspecialidadService
    {
        Task<List<EspecialidadDTO>?> ListarAsync();

        Task<EspecialidadDTO?> ObtenerAsync(int id);

        Task<EspecialidadDTO?> CrearAsync(EspecialidadDTO dto);

        Task<EspecialidadDTO?> EditarAsync(int id, EspecialidadDTO dto);

        Task<bool> CambiarEstadoAsync(int id, bool isActive);
    }
}
