using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface ICie10Repository
    {
        Task<List<Cie10DTO>> ListarAsync(string? q = null);

        Task<Cie10DTO?> CrearAsync(Cie10CrearDTO dto);
        Task<Cie10DTO?> EditarAsync(string codigo, Cie10EditarDTO dto);
        Task<bool> EliminarAsync(string codigo);
    }
}
