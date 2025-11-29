using CliniPlus.Shared.DTOs;

namespace CliniPlus.Api.Repositories.Contrato
{
    public interface ITipoTurnoRepository
    {
        Task<List<TipoTurnoDTO>> ListarActivosAsync();

    }
}
