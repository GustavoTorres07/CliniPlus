using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface IFichaMedicaService
    {
        Task<FichaMedicaDTO?> ObtenerAsync(int pacienteId);
        Task<bool> GuardarAsync(FichaMedicaDTO dto);
        Task<FichaMedicaDTO?> ObtenerMiFichaAsync();

    }
}
