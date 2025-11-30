using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class AdminPerfilRepository : IAdminPerfilRepository
    {
        private readonly AppDbContext _db;

        public AdminPerfilRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PerfilAdminDTO?> ObtenerAsync(int idUsuario)
        {
            var u = await _db.Usuario.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);

            if (u == null) return null;

            return new PerfilAdminDTO
            {
                UsuarioId = u.IdUsuario,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Rol = u.Rol,
                FechaRegistro = u.FechaRegistro
            };
        }

        public async Task<bool> ActualizarAsync(int idUsuario, PerfilAdminDTO dto)
        {
            var u = await _db.Usuario.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
            if (u == null) return false;

            u.Nombre = dto.Nombre;
            u.Apellido = dto.Apellido;
            u.Email = dto.Email;

            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CambiarPasswordAsync(int idUsuario, CambiarPasswordDTO dto)
        {
            var u = await _db.Usuario.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);

            if (u == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.PasswordActual, u.PasswordHash))
                return false;

            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordNueva);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
