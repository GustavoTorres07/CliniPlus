using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IMedicoRepository
    {
        Task<List<MedicoDetalleDTO>> ListarAsync(bool soloActivos = true);
        Task<MedicoDetalleDTO> CrearAsync(MedicoCrearDTO dto);
    }
}
