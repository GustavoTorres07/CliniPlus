using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class ObraSocialRepository : IObraSocialRepository
    {
        private readonly AppDbContext _db;

        public ObraSocialRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ObraSocialDTO>> ListarAsync()
        {
            return await _db.ObraSocial
                .OrderBy(o => o.Nombre)
                .Select(o => new ObraSocialDTO
                {
                    IdObraSocial = o.IdObraSocial,
                    Nombre = o.Nombre,
                    Descripcion = o.Descripcion,
                    IsActive = o.IsActive
                })
                .ToListAsync();
        }

        public async Task<ObraSocialDTO?> ObtenerPorIdAsync(int id)
        {
            return await _db.ObraSocial
                .Where(o => o.IdObraSocial == id)
                .Select(o => new ObraSocialDTO
                {
                    IdObraSocial = o.IdObraSocial,
                    Nombre = o.Nombre,
                    Descripcion = o.Descripcion,
                    IsActive = o.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ObraSocialDTO> CrearAsync(ObraSocialDTO dto)
        {
            var nombreTrim = dto.Nombre.Trim();

            bool existe = await _db.ObraSocial
                .AnyAsync(o => o.Nombre == nombreTrim);

            if (existe)
                throw new InvalidOperationException("OBRASOCIAL_EXISTE");

            var entity = new ObraSocial
            {
                Nombre = nombreTrim,
                Descripcion = dto.Descripcion,
                IsActive = dto.IsActive
            };

            _db.ObraSocial.Add(entity);
            await _db.SaveChangesAsync();

            dto.IdObraSocial = entity.IdObraSocial;
            dto.Nombre = entity.Nombre;
            return dto;
        }

        public async Task<ObraSocialDTO?> EditarAsync(int id, ObraSocialDTO dto)
        {
            var os = await _db.ObraSocial.FindAsync(id);
            if (os == null) return null;

            var nombreTrim = dto.Nombre.Trim();

            bool existe = await _db.ObraSocial
                .AnyAsync(o => o.IdObraSocial != id && o.Nombre == nombreTrim);

            if (existe)
                throw new InvalidOperationException("OBRASOCIAL_EXISTE");

            os.Nombre = nombreTrim;
            os.Descripcion = dto.Descripcion;
            os.IsActive = dto.IsActive;

            await _db.SaveChangesAsync();

            dto.IdObraSocial = os.IdObraSocial;
            dto.Nombre = os.Nombre;
            return dto;
        }

        public async Task<bool> CambiarEstadoAsync(int id, bool isActive)
        {
            var os = await _db.ObraSocial.FindAsync(id);
            if (os == null) return false;

            os.IsActive = isActive;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
