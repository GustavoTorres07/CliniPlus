using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class PacientePerfilRepository : IPacientePerfilRepository
    {
        private readonly AppDbContext _db;

        public PacientePerfilRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PerfilPacienteDTO?> ObtenerAsync(int idUsuario)
        {
            var usuario = await _db.Usuario
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && u.IsActive);

            if (usuario == null)
                return null;

            var paciente = await _db.Paciente
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UsuarioId == idUsuario);

            if (paciente == null)
                return null;

            var os = paciente.ObraSocialId.HasValue
                ? await _db.ObraSocial
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.IdObraSocial == paciente.ObraSocialId.Value)
                : null;

            return new PerfilPacienteDTO
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                DNI = paciente.DNI,
                Telefono = paciente.Telefono ?? "",
                ObraSocialId = paciente.ObraSocialId,
                ObraSocialNombre = os?.Nombre,
                NumeroAfiliado = paciente.NumeroAfiliado ?? "",
                FechaRegistro = usuario.FechaRegistro
            };
        }

        public async Task<bool> ActualizarAsync(int idUsuario, PerfilPacienteDTO dto)
        {
            var usuario = await _db.Usuario.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            var paciente = await _db.Paciente.FirstOrDefaultAsync(p => p.UsuarioId == idUsuario);

            if (usuario == null || paciente == null)
                return false;

            usuario.Nombre = dto.Nombre;
            usuario.Apellido = dto.Apellido;
            usuario.Email = dto.Email;

            paciente.Telefono = dto.Telefono;
            paciente.DNI = dto.DNI;
            paciente.ObraSocialId = dto.ObraSocialId;
            paciente.NumeroAfiliado = dto.NumeroAfiliado;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CambiarPasswordAsync(int idUsuario, string passwordActual, string passwordNueva)
        {
            // 1) Obtener usuario
            var usuario = await _db.Usuario
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && u.IsActive);

            if (usuario == null)
                return false;

            // 2) Verificar contraseña actual con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(passwordActual, usuario.PasswordHash))
                return false;

            // 3) Hashear nueva contraseña
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordNueva);

            // 4) IMPORTANTE: si venía de recuperación, marcar como resuelto
            if (usuario.RecuperarContrasena)
                usuario.RecuperarContrasena = false;

            // 5) Guardar cambios
            await _db.SaveChangesAsync();
            return true;
        }

    }
}
