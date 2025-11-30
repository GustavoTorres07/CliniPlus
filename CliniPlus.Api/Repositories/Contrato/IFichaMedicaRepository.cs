using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface IFichaMedicaRepository
    {
        Task<FichaMedicaDTO> ObtenerPorPacienteAsync(int pacienteId);

        Task<FichaMedicaDTO> GuardarAsync(int pacienteId, FichaMedicaDTO dto);
    }
}
