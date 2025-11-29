// CliniPlus.Api/Repositories/Implementa/TipoTurnoRepository.cs
using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class TipoTurnoRepository : ITipoTurnoRepository
    {
        private readonly AppDbContext _db;

        public TipoTurnoRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TipoTurnoDTO>> ListarActivosAsync()
        {
            return await _db.TipoTurno
                .Where(t => t.Activo)
                .OrderBy(t => t.Nombre)
                .Select(t => new TipoTurnoDTO
                {
                    IdTipoTurno = t.IdTipoTurno,
                    Nombre = t.Nombre,
                    DuracionMin = t.DuracionMin
                })
                .ToListAsync();
        }
    }
}
