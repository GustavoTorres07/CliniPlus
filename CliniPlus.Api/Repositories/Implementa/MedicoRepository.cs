using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly AppDbContext _db;

        public MedicoRepository(AppDbContext db)
        {
            _db = db;
        }

        // -----------------------------
        // LISTAR MÉDICOS
        // -----------------------------

        public async Task<List<MedicoDetalleDTO>> ListarAsync(bool soloActivos = true)
        {
            var query = _db.Medico
                .Include(m => m.Usuario)
                .Include(m => m.Especialidades)
                    .ThenInclude(me => me.Especialidad)
                .AsQueryable();

            if (soloActivos)
                query = query.Where(m => m.IsActive);

            var lista = await query
                .OrderBy(m => m.Usuario!.Apellido)
                .ThenBy(m => m.Usuario!.Nombre)
                .Select(m => new MedicoDetalleDTO
                {
                    IdMedico = m.IdMedico,
                    NombreCompleto = m.Usuario!.Nombre + " " + m.Usuario!.Apellido,
                    Email = m.Usuario!.Email,
                    Bio = m.Bio,
                    FotoUrl = m.FotoUrl,
                    IsActive = m.IsActive,

                    // Tomamos la PRIMER (y única) especialidad
                    Especialidad = m.Especialidades
                        .Select(e => e.Especialidad!.Nombre)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return lista;
        }


        // -----------------------------
        // CREAR MÉDICO COMPLETO
        // -----------------------------
        public async Task<MedicoDetalleDTO> CrearAsync(MedicoCrearDTO dto)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            // 1) Crear usuario con rol Médico
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = "Medico",
                IsActive = true,
                FechaRegistro = DateTime.UtcNow
            };

            _db.Usuario.Add(usuario);
            await _db.SaveChangesAsync();

            // 2) Crear médico
            var medico = new Medico
            {
                UsuarioId = usuario.IdUsuario,
                Bio = dto.Bio,
                FotoUrl = dto.FotoUrl,
                DefaultSlotMin = dto.DefaultSlotMin,
                IsActive = true
            };

            _db.Medico.Add(medico);
            await _db.SaveChangesAsync();

            // 3) Crear UNA sola especialidad
            _db.MedicoEspecialidad.Add(new MedicoEspecialidad
            {
                MedicoId = medico.IdMedico,
                EspecialidadId = dto.EspecialidadId
            });

            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            // 4) Mapear a DTO de salida
            var especialidadNombre = await _db.MedicoEspecialidad
                .Include(x => x.Especialidad)
                .Where(x => x.MedicoId == medico.IdMedico)
                .Select(x => x.Especialidad!.Nombre)
                .FirstOrDefaultAsync();

            return new MedicoDetalleDTO
            {
                IdMedico = medico.IdMedico,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                Email = usuario.Email,
                Bio = medico.Bio,
                FotoUrl = medico.FotoUrl,
                IsActive = medico.IsActive,
                Especialidad = especialidadNombre
            };
        }
    }
}
