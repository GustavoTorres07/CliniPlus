using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class EspecialidadRepository : IEspecialidadRepository
    {
        private readonly AppDbContext _db;

        public EspecialidadRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<EspecialidadDTO>> ListarAsync()
        {
            return await _db.Especialidad
                .OrderBy(e => e.Nombre)
                .Select(e => new EspecialidadDTO
                {
                    IdEspecialidad = e.IdEspecialidad,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    IsActive = e.IsActive
                })
                .ToListAsync();
        }

        public async Task<EspecialidadDTO?> ObtenerPorIdAsync(int id)
        {
            return await _db.Especialidad
                .Where(e => e.IdEspecialidad == id)
                .Select(e => new EspecialidadDTO
                {
                    IdEspecialidad = e.IdEspecialidad,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    IsActive = e.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<EspecialidadDTO> CrearAsync(EspecialidadDTO dto)
        {
            var nombreTrim = dto.Nombre.Trim();

            bool existe = await _db.Especialidad
                .AnyAsync(e => e.Nombre == nombreTrim);

            if (existe)
                throw new InvalidOperationException("ESPECIALIDAD_EXISTE");

            var entity = new Especialidad
            {
                Nombre = nombreTrim,
                Descripcion = dto.Descripcion,
                IsActive = dto.IsActive
            };

            _db.Especialidad.Add(entity);
            await _db.SaveChangesAsync();

            dto.IdEspecialidad = entity.IdEspecialidad;
            dto.Nombre = entity.Nombre;
            return dto;
        }

        public async Task<EspecialidadDTO?> EditarAsync(int id, EspecialidadDTO dto)
        {
            var esp = await _db.Especialidad.FindAsync(id);
            if (esp == null) return null;

            var nombreTrim = dto.Nombre.Trim();

            bool existe = await _db.Especialidad
                .AnyAsync(e => e.IdEspecialidad != id && e.Nombre == nombreTrim);

            if (existe)
                throw new InvalidOperationException("ESPECIALIDAD_EXISTE");

            esp.Nombre = nombreTrim;
            esp.Descripcion = dto.Descripcion;
            esp.IsActive = dto.IsActive;

            await _db.SaveChangesAsync();

            dto.IdEspecialidad = esp.IdEspecialidad;
            dto.Nombre = esp.Nombre;
            return dto;
        }

        public async Task<bool> CambiarEstadoAsync(int id, bool isActive)
        {
            var esp = await _db.Especialidad.FindAsync(id);
            if (esp == null) return false;

            esp.IsActive = isActive;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
