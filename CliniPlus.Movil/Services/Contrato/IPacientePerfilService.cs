using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface IPacientePerfilService
    {
        Task<PerfilPacienteDTO?> ObtenerAsync();
        Task<bool> ActualizarAsync(PerfilPacienteDTO dto);
        Task<bool> CambiarPasswordAsync(CambiarPasswordDTO dto);
    }
}
