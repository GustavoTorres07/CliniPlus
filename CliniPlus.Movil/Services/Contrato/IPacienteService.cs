using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface IPacienteService
    {
        Task<List<PacienteListDTO>?> ListarAsync(bool incluirInactivos = false);

        Task<PacienteDetalleDTO?> ObtenerAsync(int id);

        Task<PacienteDetalleDTO?> CrearAsync(PacienteCrearDTO dto);

        Task<PacienteDetalleDTO?> EditarAsync(int id, PacienteEditarDTO dto);

        Task<bool> CambiarEstadoAsync(int id, bool isActive);

        Task<List<PacienteListadoDTO>?> ListarPacientesSecretariaAsync();

        Task<bool> ActivarCuentaPacienteAsync(PacienteActivarCuentaDTO dto);

        Task<bool> ActivarCuentaProvisionalAsync(PacienteActivarCuentaDTO dto);
    }
}
