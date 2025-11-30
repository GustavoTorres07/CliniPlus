using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class PacienteRepository : IPacienteRepository
    {
        private readonly AppDbContext _db;

        public PacienteRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<PacienteListDTO>> ListarAsync(bool incluirInactivos = false)
        {
            var query = _db.Paciente
                .Include(p => p.Usuario)
                .Include(p => p.ObraSocial)
                .AsQueryable();

            if (!incluirInactivos)
                query = query.Where(p => p.IsActive);

            var lista = await query
                .OrderBy(p => p.DNI)
                .Select(p => new PacienteListDTO
                {
                    IdPaciente = p.IdPaciente,
                    NombreCompleto = p.Usuario != null
                        ? p.Usuario.Nombre + " " + p.Usuario.Apellido
                        : null,
                    DNI = p.DNI,
                    Email = p.Email ?? p.Usuario!.Email,
                    IsProvisional = p.IsProvisional,
                    IsActive = p.IsActive,
                    ObraSocialNombre = p.ObraSocial != null ? p.ObraSocial.Nombre : null
                })
                .ToListAsync();

            return lista;
        }

        public async Task<PacienteDetalleDTO?> ObtenerPorIdAsync(int id)
        {
            var p = await _db.Paciente
                .Include(p => p.Usuario)
                .Include(p => p.ObraSocial)
                .FirstOrDefaultAsync(p => p.IdPaciente == id);

            if (p == null) return null;

            return new PacienteDetalleDTO
            {
                IdPaciente = p.IdPaciente,
                UsuarioId = p.UsuarioId,
                NombreCompleto = p.Usuario != null
            ? p.Usuario.Nombre + " " + p.Usuario.Apellido
            : null,
                DNI = p.DNI,
                Email = p.Email ?? p.Usuario?.Email,
                Telefono = p.Telefono,
                IsProvisional = p.IsProvisional,
                IsActive = p.IsActive,
                ObraSocialId = p.ObraSocialId,
                ObraSocialNombre = p.ObraSocial?.Nombre,
                NumeroAfiliado = p.NumeroAfiliado
            };
        }

        public async Task<PacienteDetalleDTO> CrearAsync(PacienteCrearDTO dto)
        {
            var existeDni = await _db.Paciente.AnyAsync(x => x.DNI == dto.DNI);
            if (existeDni)
                throw new InvalidOperationException("DNI_EXISTE");

            int? usuarioId = null;

            if (dto.UsuarioId.HasValue)
            {
                var usuario = await _db.Usuario
                    .FirstOrDefaultAsync(u =>
                        u.IdUsuario == dto.UsuarioId.Value &&
                        u.Rol == "Paciente" &&
                        u.IsActive);

                if (usuario == null)
                    throw new InvalidOperationException("USUARIO_INVALIDO");

                bool usuarioTomado = await _db.Paciente
                    .AnyAsync(p => p.UsuarioId == dto.UsuarioId.Value);

                if (usuarioTomado)
                    throw new InvalidOperationException("USUARIO_YA_VINCULADO");

                usuarioId = dto.UsuarioId.Value;
            }

            var paciente = new Paciente
            {
                DNI = dto.DNI,
                UsuarioId = usuarioId,
                IsProvisional = dto.IsProvisional,
                Telefono = dto.Telefono,
                Email = dto.Email,
                ObraSocialId = dto.ObraSocialId,
                NumeroAfiliado = dto.NumeroAfiliado,
                IsActive = true
            };

            _db.Paciente.Add(paciente);
            await _db.SaveChangesAsync();

            return await ObtenerPorIdAsync(paciente.IdPaciente)
                ?? throw new InvalidOperationException("ERROR_CREAR_PACIENTE");
        }


        public async Task<PacienteDetalleDTO?> EditarAsync(int id, PacienteEditarDTO dto)
        {
            var p = await _db.Paciente.FirstOrDefaultAsync(x => x.IdPaciente == id);
            if (p == null) return null;

            if (dto.DNI != p.DNI)
            {
                var existeDni = await _db.Paciente.AnyAsync(x => x.DNI == dto.DNI && x.IdPaciente != id);
                if (existeDni)
                    throw new InvalidOperationException("DNI_EXISTE");
            }

            int? usuarioId = null;

            if (dto.UsuarioId.HasValue)
            {
                var usuario = await _db.Usuario
                    .FirstOrDefaultAsync(u =>
                        u.IdUsuario == dto.UsuarioId.Value &&
                        u.Rol == "Paciente" &&
                        u.IsActive);

                if (usuario == null)
                    throw new InvalidOperationException("USUARIO_INVALIDO");

                bool usuarioTomado = await _db.Paciente
                    .AnyAsync(x => x.UsuarioId == dto.UsuarioId.Value && x.IdPaciente != id);

                if (usuarioTomado)
                    throw new InvalidOperationException("USUARIO_YA_VINCULADO");

                usuarioId = dto.UsuarioId.Value;
            }

            p.DNI = dto.DNI;
            p.Telefono = dto.Telefono;
            p.Email = dto.Email;
            p.ObraSocialId = dto.ObraSocialId;
            p.NumeroAfiliado = dto.NumeroAfiliado;
            p.IsProvisional = dto.IsProvisional;
            p.UsuarioId = usuarioId; 

            await _db.SaveChangesAsync();

            return await ObtenerPorIdAsync(p.IdPaciente);
        }


        public async Task<bool> CambiarEstadoAsync(int id, bool isActive)
        {
            var p = await _db.Paciente.FirstOrDefaultAsync(x => x.IdPaciente == id);
            if (p == null) return false;

            p.IsActive = isActive;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PacienteListadoDTO>> ListarParaSecretariaAsync()
        {
            return await _db.Paciente
                .Include(p => p.Usuario)
                .Include(p => p.ObraSocial)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Usuario!.Apellido)
                .ThenBy(p => p.Usuario!.Nombre)
                .Select(p => new PacienteListadoDTO
                {
                    IdPaciente = p.IdPaciente,
                    NombreCompleto = p.Usuario != null
                        ? p.Usuario.Nombre + " " + p.Usuario.Apellido
                        : p.DNI,
                    DNI = p.DNI,
                    IsProvisional = p.IsProvisional,
                    ObraSocialNombre = p.ObraSocial != null
                        ? p.ObraSocial.Nombre
                        : null
                })
                .ToListAsync();
        }

        public async Task<bool> ActivarCuentaProvisionalAsync(PacienteActivarCuentaDTO dto)
        {
            var paciente = await _db.Paciente
                .FirstOrDefaultAsync(p => p.IdPaciente == dto.PacienteId && p.IsActive);

            if (paciente == null)
                throw new InvalidOperationException("PACIENTE_NO_ENCONTRADO");

            if (!paciente.IsProvisional)
                throw new InvalidOperationException("PACIENTE_NO_ES_PROVISIONAL");

            if (!string.Equals(paciente.DNI, dto.DNI, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("DNI_NO_COINCIDE");

            var usuario = await _db.Usuario
                .FirstOrDefaultAsync(u => u.IdUsuario == dto.UsuarioId && u.IsActive);

            if (usuario == null)
                throw new InvalidOperationException("USUARIO_NO_ENCONTRADO");

            if (usuario.Rol != "Paciente")
                throw new InvalidOperationException("USUARIO_NO_ES_PACIENTE");

            var yaVinculado = await _db.Paciente.AnyAsync(p =>
                p.UsuarioId == dto.UsuarioId && p.IsActive);

            if (yaVinculado)
                throw new InvalidOperationException("USUARIO_YA_VINCULADO");

            paciente.UsuarioId = dto.UsuarioId;
            paciente.Email = dto.Email;
            paciente.Telefono = dto.Telefono;
            paciente.IsProvisional = false;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int?> ObtenerIdPorUsuarioAsync(int usuarioId)
        {
            return await _db.Paciente
                .Where(p => p.UsuarioId == usuarioId && p.IsActive)
                .Select(p => (int?)p.IdPaciente)
                .FirstOrDefaultAsync();
        }
    }
}
