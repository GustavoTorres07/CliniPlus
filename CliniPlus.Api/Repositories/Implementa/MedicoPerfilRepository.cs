using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class MedicoPerfilRepository : IMedicoPerfilRepository
    {
        private readonly AppDbContext _context;

        public MedicoPerfilRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PerfilMedicoDTO?> ObtenerAsync(int idUsuario)
        {
            // 1) Usuario activo
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && u.IsActive);

            if (usuario == null || usuario.Rol != "Medico")
                return null;

            // 2) Médico + Especialidad
            var medico = await _context.Medico
                .Include(m => m.Especialidad)
                .FirstOrDefaultAsync(m => m.UsuarioId == idUsuario && m.IsActive);

            if (medico == null)
                return null;

            var dto = new PerfilMedicoDTO
            {
                IdUsuario = usuario.IdUsuario,
                IdMedico = medico.IdMedico,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Bio = medico.Bio,
                FotoUrl = medico.FotoUrl,
                EspecialidadId = medico.EspecialidadId,
                EspecialidadNombre = medico.Especialidad != null
                    ? medico.Especialidad.Nombre
                    : null
            };

            // 👇 llenar también la lista de Especialidades para la UI
            if (medico.EspecialidadId.HasValue && medico.Especialidad is not null)
            {
                dto.Especialidades.Add(new SimpleEspecialidadDTO
                {
                    IdEspecialidad = medico.EspecialidadId.Value,
                    Nombre = medico.Especialidad.Nombre
                });
            }

            return dto;
        }

        public async Task<bool> ActualizarAsync(int idUsuario, PerfilMedicoDTO dto)
        {
            // 1) Usuario y médico
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && u.IsActive);

            var medico = await _context.Medico
                .FirstOrDefaultAsync(m => m.UsuarioId == idUsuario && m.IsActive);

            if (usuario == null || medico == null)
                return false;

            // 2) Actualizar datos de Usuario
            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                usuario.Nombre = dto.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Apellido))
                usuario.Apellido = dto.Apellido.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Email))
                usuario.Email = dto.Email.Trim();

            // 3) Actualizar datos de Médico
            medico.Bio = dto.Bio;
            medico.FotoUrl = dto.FotoUrl;

            // 4) Actualizar especialidad (una sola)
            if (dto.EspecialidadId.HasValue)
            {
                bool existe = await _context.Especialidad
                    .AnyAsync(e => e.IdEspecialidad == dto.EspecialidadId.Value);

                if (!existe)
                    throw new InvalidOperationException("ESPECIALIDAD_NO_ENCONTRADA");

                medico.EspecialidadId = dto.EspecialidadId.Value;
            }
            else
            {
                // Permitir dejar al médico sin especialidad
                medico.EspecialidadId = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
