using BCrypt.Net;
using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _db;

        public UsuarioRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<UsuarioListDTO>> ListarAsync()
        {
            return await _db.Usuario
                .OrderBy(u => u.Nombre)
                .Select(u => new UsuarioListDTO
                {
                    IdUsuario = u.IdUsuario,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Email = u.Email,
                    Rol = u.Rol,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<UsuarioDetalleDTO?> ObtenerPorIdAsync(int id)
        {
            return await _db.Usuario
                .Where(u => u.IdUsuario == id)
                .Select(u => new UsuarioDetalleDTO
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Email = u.Email,
                    Rol = u.Rol,
                    IsActive = u.IsActive,
                    FechaRegistro = u.FechaRegistro
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UsuarioDetalleDTO> CrearAsync(UsuarioCrearDTO dto)
        {
            var emailTrim = dto.Email.Trim().ToLower();

            bool existe = await _db.Usuario.AnyAsync(u => u.Email == emailTrim);
            if (existe)
                throw new InvalidOperationException("EMAIL_EXISTE");

            var entity = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Email = emailTrim,
                Rol = dto.Rol,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Contraseña),
                FechaRegistro = DateTime.UtcNow,
                IsActive = true
            };

            _db.Usuario.Add(entity);
            await _db.SaveChangesAsync();

            return new UsuarioDetalleDTO
            {
                IdUsuario = entity.IdUsuario,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                Email = entity.Email,
                Rol = entity.Rol,
                IsActive = entity.IsActive,
                FechaRegistro = entity.FechaRegistro
            };
        }

        public async Task<UsuarioDetalleDTO?> EditarAsync(int id, UsuarioEditarDTO dto)
        {
            var u = await _db.Usuario.FindAsync(id);
            if (u == null) return null;

            var emailTrim = dto.Email.Trim().ToLower();

            bool existe = await _db.Usuario
                .AnyAsync(x => x.IdUsuario != id && x.Email == emailTrim);

            if (existe)
                throw new InvalidOperationException("EMAIL_EXISTE");

            u.Nombre = dto.Nombre.Trim();
            u.Apellido = dto.Apellido.Trim();
            u.Email = emailTrim;
            u.Rol = dto.Rol;

            await _db.SaveChangesAsync();

            return new UsuarioDetalleDTO
            {
                IdUsuario = u.IdUsuario,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Rol = u.Rol,
                IsActive = u.IsActive,
                FechaRegistro = u.FechaRegistro
            };
        }

        public async Task<bool> CambiarEstadoAsync(int id, bool isActive)
        {
            var u = await _db.Usuario.FindAsync(id);
            if (u == null) return false;

            u.IsActive = isActive;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CambiarPasswordAdminAsync(int id, string nuevaPassword)
        {
            var u = await _db.Usuario.FirstOrDefaultAsync(x => x.IdUsuario == id);
            if (u == null) return false;

            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
