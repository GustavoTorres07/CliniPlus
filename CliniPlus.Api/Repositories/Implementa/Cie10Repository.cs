using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class Cie10Repository : ICie10Repository
    {
        private readonly AppDbContext _db;

        public Cie10Repository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Cie10DTO>> ListarAsync(string? q = null)
        {
            var query = _db.CIE10.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(c =>
                    c.Codigo.Contains(q) ||
                    c.Descripcion.Contains(q));
            }

            query = query.OrderBy(c => c.Codigo).Take(200);

            return await query
                .Select(c => new Cie10DTO
                {
                    Codigo = c.Codigo,
                    Descripcion = c.Descripcion
                })
                .ToListAsync();
        }

        public async Task<Cie10DTO?> CrearAsync(Cie10CrearDTO dto)
        {
            var codigo = dto.Codigo.Trim().ToUpper();

            var existe = await _db.CIE10.AnyAsync(c => c.Codigo == codigo);
            if (existe)
                throw new InvalidOperationException("CIE10_YA_EXISTE");

            var entidad = new CIE10
            {
                Codigo = codigo,
                Descripcion = dto.Descripcion.Trim()
            };

            _db.CIE10.Add(entidad);
            await _db.SaveChangesAsync();

            return new Cie10DTO
            {
                Codigo = entidad.Codigo,
                Descripcion = entidad.Descripcion
            };
        }

        public async Task<Cie10DTO?> EditarAsync(string codigo, Cie10EditarDTO dto)
        {
            codigo = codigo.Trim().ToUpper();

            var entidad = await _db.CIE10.FirstOrDefaultAsync(c => c.Codigo == codigo);
            if (entidad == null) return null;

            entidad.Descripcion = dto.Descripcion.Trim();
            await _db.SaveChangesAsync();

            return new Cie10DTO
            {
                Codigo = entidad.Codigo,
                Descripcion = entidad.Descripcion
            };
        }

        public async Task<bool> EliminarAsync(string codigo)
        {
            codigo = codigo.Trim().ToUpper();

            var entidad = await _db.CIE10.FirstOrDefaultAsync(c => c.Codigo == codigo);
            if (entidad == null) return false;

            _db.CIE10.Remove(entidad);

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
