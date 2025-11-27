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
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (usuario == null || usuario.Rol != "Medico")
                return null;

            var medico = await _context.Medico
                .Include(m => m.Especialidades).ThenInclude(e => e.Especialidad)
                .FirstOrDefaultAsync(m => m.UsuarioId == idUsuario);

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
                FotoUrl = medico.FotoUrl
            };

            dto.Especialidades = medico.Especialidades
                .Select(e => new SimpleEspecialidadDTO
                {
                    IdEspecialidad = e.EspecialidadId,
                    Nombre = e.Especialidad.Nombre
                }).ToList();

            return dto;
        }

        public async Task<bool> ActualizarAsync(int idUsuario, PerfilMedicoDTO dto)
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            var medico = await _context.Medico.FirstOrDefaultAsync(m => m.UsuarioId == idUsuario);

            if (usuario == null || medico == null)
                return false;

            // Usuario
            usuario.Nombre = dto.Nombre;
            usuario.Apellido = dto.Apellido;
            usuario.Email = dto.Email;

            // Medico
            medico.Bio = dto.Bio;
            medico.FotoUrl = dto.FotoUrl;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
