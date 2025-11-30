using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface ICie10Service
    {
        Task<List<Cie10DTO>?> BuscarAsync(string? filtro);

        Task<Cie10DTO?> CrearAsync(Cie10CrearDTO dto);

        Task<Cie10DTO?> EditarAsync(string codigo, Cie10EditarDTO dto);

        Task<bool> EliminarAsync(string codigo);
    }
}
