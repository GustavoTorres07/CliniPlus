using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly AppDbContext _db;

        public MedicoRepository(AppDbContext db)
        {
            _db = db;
        }

        // =====================================================
        //  LISTAR
        // =====================================================
        public async Task<List<MedicoListadoDTO>> ListarAsync()
        {
            return await _db.Medico
                .Include(m => m.Usuario)
                .Include(m => m.Especialidades)
                    .ThenInclude(me => me.Especialidad)
                .OrderBy(m => m.Usuario.Nombre)
                .Select(m => new MedicoListadoDTO
                {
                    IdMedico = m.IdMedico,
                    UsuarioId = m.UsuarioId,
                    NombreCompleto = m.Usuario.Nombre + " " + m.Usuario.Apellido,
                    Email = m.Usuario.Email,
                    Especialidad = m.Especialidades
                        .Select(x => x.Especialidad.Nombre)
                        .FirstOrDefault(),
                    IsActive = m.IsActive,
                    FotoUrl = m.FotoUrl         // 👈 agregar esto si no estaba
                })
                .ToListAsync();
        }

        // =====================================================
        //  OBTENER DETALLE POR ID
        // =====================================================
        public async Task<MedicoDetalleDTO?> ObtenerAsync(int id)
        {
            var m = await _db.Medico
                .Include(m => m.Usuario)
                .Include(m => m.Especialidades)
                    .ThenInclude(me => me.Especialidad)
                .FirstOrDefaultAsync(m => m.IdMedico == id);

            if (m == null) return null;

            var esp = m.Especialidades.FirstOrDefault();

            return new MedicoDetalleDTO
            {
                IdMedico = m.IdMedico,
                UsuarioId = m.UsuarioId,
                NombreCompleto = m.Usuario.Nombre + " " + m.Usuario.Apellido,
                Email = m.Usuario.Email,
                Bio = m.Bio,
                FotoUrl = m.FotoUrl,
                DefaultSlotMin = m.DefaultSlotMin,
                IsActive = m.IsActive,
                EspecialidadId = esp?.EspecialidadId,
                EspecialidadNombre = esp?.Especialidad.Nombre
            };
        }

        // =====================================================
        //  CREAR MÉDICO
        // =====================================================
        public async Task<int?> CrearAsync(MedicoCrearDTO dto)
        {
            // 1) ¿Existe el usuario?
            var usuario = await _db.Usuario
                .FirstOrDefaultAsync(u => u.IdUsuario == dto.UsuarioId && u.IsActive);

            if (usuario is null)
                throw new InvalidOperationException("USUARIO_NO_ENCONTRADO");

            if (usuario.Rol != "Medico")
                throw new InvalidOperationException("USUARIO_NO_MEDICO");

            // 3) ¿Ya está vinculado como médico?
            var yaExiste = await _db.Medico.AnyAsync(m => m.UsuarioId == dto.UsuarioId);
            if (yaExiste)
                throw new InvalidOperationException("USUARIO_YA_ES_MEDICO");

            // Crear médico
            var med = new Medico
            {
                UsuarioId = dto.UsuarioId,
                Bio = dto.Bio,
                FotoUrl = dto.FotoUrl,
                DefaultSlotMin = dto.DefaultSlotMin > 0 ? dto.DefaultSlotMin : 30,
                IsActive = true
            };

            _db.Medico.Add(med);
            await _db.SaveChangesAsync();

            // Especialidad (opcional)
            if (dto.EspecialidadId.HasValue)
            {
                bool espExiste = await _db.Especialidad
                    .AnyAsync(e => e.IdEspecialidad == dto.EspecialidadId.Value);

                if (!espExiste)
                    throw new InvalidOperationException("ESPECIALIDAD_NO_ENCONTRADA");

                _db.MedicoEspecialidad.Add(new MedicoEspecialidad
                {
                    MedicoId = med.IdMedico,
                    EspecialidadId = dto.EspecialidadId.Value
                });

                await _db.SaveChangesAsync();
            }

            return med.IdMedico;
        }

        // =====================================================
        //  EDITAR
        // =====================================================
        public async Task<MedicoDetalleDTO?> EditarAsync(int id, MedicoEditarDTO dto)
        {
            var m = await _db.Medico
                .Include(x => x.Especialidades)
                .FirstOrDefaultAsync(x => x.IdMedico == id);

            if (m == null) return null;

            m.Bio = dto.Bio;
            m.FotoUrl = dto.FotoUrl;
            m.DefaultSlotMin = dto.DefaultSlotMin > 0 ? dto.DefaultSlotMin : m.DefaultSlotMin;

            // BORRAR especialidad previa correctamente
            _db.MedicoEspecialidad.RemoveRange(m.Especialidades);

            if (dto.EspecialidadId.HasValue)
            {
                _db.MedicoEspecialidad.Add(new MedicoEspecialidad
                {
                    MedicoId = m.IdMedico,
                    EspecialidadId = dto.EspecialidadId.Value
                });
            }

            await _db.SaveChangesAsync();
            return await ObtenerAsync(id);
        }

        // =====================================================
        //  CAMBIAR ESTADO
        // =====================================================
        public async Task<bool> CambiarEstadoAsync(int id, bool activo)
        {
            var m = await _db.Medico.FirstOrDefaultAsync(x => x.IdMedico == id);
            if (m == null) return false;

            m.IsActive = activo;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<MedicoListadoDTO>> ListarPorEspecialidadAsync(int especialidadId)
        {
            return await _db.Medico
                .Include(m => m.Usuario)
                .Include(m => m.Especialidades)
                    .ThenInclude(me => me.Especialidad)
                .Where(m => m.Especialidades.Any(e => e.EspecialidadId == especialidadId))
                .Select(m => new MedicoListadoDTO
                {
                    IdMedico = m.IdMedico,
                    UsuarioId = m.UsuarioId,
                    NombreCompleto = m.Usuario.Nombre + " " + m.Usuario.Apellido,
                    Email = m.Usuario.Email,
                    FotoUrl = m.FotoUrl,
                    Especialidad = m.Especialidades.Select(e => e.Especialidad.Nombre).FirstOrDefault(),
                    IsActive = m.IsActive
                })
                .ToListAsync();
        }

        public async Task<List<MedicoHorarioDTO>> ListarHorariosAsync(int medicoId)
        {
            return await _db.MedicoHorario
                .Where(h => h.MedicoId == medicoId)
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .Select(h => new MedicoHorarioDTO
                {
                    IdHorario = h.IdHorario,
                    MedicoId = h.MedicoId,
                    DiaSemana = h.DiaSemana,
                    // 👇 Los envío como string HH:mm
                    HoraInicio = h.HoraInicio.ToString(@"hh\:mm"),
                    HoraFin = h.HoraFin.ToString(@"hh\:mm"),
                    SlotMinOverride = h.SlotMinOverride,
                    Activo = h.Activo
                })
                .ToListAsync();
        }

        public async Task<MedicoHorarioDTO?> CrearHorarioAsync(int medicoId, MedicoHorarioDTO dto)
        {
            var medicoExiste = await _db.Medico.AnyAsync(m => m.IdMedico == medicoId);
            if (!medicoExiste)
                throw new InvalidOperationException("MEDICO_NO_ENCONTRADO");

            if (dto.DiaSemana < 0 || dto.DiaSemana > 6)
                throw new InvalidOperationException("DIA_INVALIDO");

            // 👇 Convertimos string → TimeSpan
            if (!TimeSpan.TryParse(dto.HoraInicio, out var hi))
                throw new InvalidOperationException("HORA_INICIO_INVALIDA");

            if (!TimeSpan.TryParse(dto.HoraFin, out var hf))
                throw new InvalidOperationException("HORA_FIN_INVALIDA");

            if (hf <= hi)
                throw new InvalidOperationException("HORARIO_RANGO_INVALIDO");

            if (dto.SlotMinOverride.HasValue && dto.SlotMinOverride.Value <= 0)
                throw new InvalidOperationException("SLOT_INVALIDO");

            var h = new MedicoHorario
            {
                MedicoId = medicoId,
                DiaSemana = dto.DiaSemana,
                HoraInicio = hi,
                HoraFin = hf,
                SlotMinOverride = dto.SlotMinOverride,
                Activo = dto.Activo
            };

            _db.MedicoHorario.Add(h);
            await _db.SaveChangesAsync();

            // 👇 Devolvemos DTO ya normalizado
            return new MedicoHorarioDTO
            {
                IdHorario = h.IdHorario,
                MedicoId = h.MedicoId,
                DiaSemana = h.DiaSemana,
                HoraInicio = h.HoraInicio.ToString(@"hh\:mm"),
                HoraFin = h.HoraFin.ToString(@"hh\:mm"),
                SlotMinOverride = h.SlotMinOverride,
                Activo = h.Activo
            };
        }

        public async Task<MedicoHorarioDTO?> EditarHorarioAsync(int medicoId, int idHorario, MedicoHorarioDTO dto)
        {
            var h = await _db.MedicoHorario
                .FirstOrDefaultAsync(x => x.IdHorario == idHorario && x.MedicoId == medicoId);

            if (h == null) return null;

            if (dto.DiaSemana < 0 || dto.DiaSemana > 6)
                throw new InvalidOperationException("DIA_INVALIDO");

            if (!TimeSpan.TryParse(dto.HoraInicio, out var hi))
                throw new InvalidOperationException("HORA_INICIO_INVALIDA");

            if (!TimeSpan.TryParse(dto.HoraFin, out var hf))
                throw new InvalidOperationException("HORA_FIN_INVALIDA");

            if (hf <= hi)
                throw new InvalidOperationException("HORARIO_RANGO_INVALIDO");

            if (dto.SlotMinOverride.HasValue && dto.SlotMinOverride.Value <= 0)
                throw new InvalidOperationException("SLOT_INVALIDO");

            h.DiaSemana = dto.DiaSemana;
            h.HoraInicio = hi;
            h.HoraFin = hf;
            h.SlotMinOverride = dto.SlotMinOverride;
            h.Activo = dto.Activo;

            await _db.SaveChangesAsync();

            return new MedicoHorarioDTO
            {
                IdHorario = h.IdHorario,
                MedicoId = h.MedicoId,
                DiaSemana = h.DiaSemana,
                HoraInicio = h.HoraInicio.ToString(@"hh\:mm"),
                HoraFin = h.HoraFin.ToString(@"hh\:mm"),
                SlotMinOverride = h.SlotMinOverride,
                Activo = h.Activo
            };
        }

        public async Task<bool> EliminarHorarioAsync(int medicoId, int idHorario)
        {
            var h = await _db.MedicoHorario
                .FirstOrDefaultAsync(x => x.IdHorario == idHorario && x.MedicoId == medicoId);

            if (h == null) return false;

            _db.MedicoHorario.Remove(h);
            await _db.SaveChangesAsync();
            return true;

        }

        public async Task<List<MedicoBloqueoDTO>> ListarBloqueosAsync(int medicoId)
        {
            return await _db.MedicoBloqueo
    .Where(b => b.MedicoId == medicoId)
    .OrderByDescending(b => b.Desde)
    .Select(b => new MedicoBloqueoDTO
    {
        IdBloqueo = b.IdBloqueo,
        MedicoId = b.MedicoId,
        Desde = b.Desde,
        Hasta = b.Hasta,
        Motivo = b.Motivo
    })
    .ToListAsync();
        }

        public async Task<MedicoBloqueoDTO?> CrearBloqueoAsync(int medicoId, MedicoBloqueoDTO dto)
        {
            var medicoExiste = await _db.Medico.AnyAsync(m => m.IdMedico == medicoId);
            if (!medicoExiste)
                throw new InvalidOperationException("MEDICO_NO_ENCONTRADO");

            if (dto.Hasta <= dto.Desde)
                throw new InvalidOperationException("BLOQUEO_RANGO_INVALIDO");

            var b = new MedicoBloqueo
            {
                MedicoId = medicoId,
                Desde = dto.Desde,
                Hasta = dto.Hasta,
                Motivo = dto.Motivo
            };

            _db.MedicoBloqueo.Add(b);
            await _db.SaveChangesAsync();

            return new MedicoBloqueoDTO
            {
                IdBloqueo = b.IdBloqueo,
                MedicoId = b.MedicoId,
                Desde = b.Desde,
                Hasta = b.Hasta,
                Motivo = b.Motivo
            };
        }

        public async Task<bool> EliminarBloqueoAsync(int medicoId, int idBloqueo)
        {
            var b = await _db.MedicoBloqueo
    .FirstOrDefaultAsync(x => x.IdBloqueo == idBloqueo && x.MedicoId == medicoId);

            if (b == null) return false;

            _db.MedicoBloqueo.Remove(b);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
