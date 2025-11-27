using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly AppDbContext _db;

        public TurnoRepository(AppDbContext db)
        {
            _db = db;
        }

        // ============================================================
        // 1) LISTAR TURNOS DEL PACIENTE
        // ============================================================
        public async Task<List<TurnoListadoPacienteDTO>> ListarPorPacienteAsync(int pacienteId)
        {
            return await _db.Turno
                .Include(t => t.Medico).ThenInclude(m => m.Usuario)
                .Include(t => t.TipoTurno)
                .Include(t => t.Medico).ThenInclude(m => m.Especialidades).ThenInclude(e => e.Especialidad)
                .Where(t => t.PacienteId == pacienteId && t.IsActive)
                .OrderBy(t => t.ScheduledAtUtc)
                .Select(t => new TurnoListadoPacienteDTO
                {
                    IdTurno = t.IdTurno,
                    ScheduledAtUtc = t.ScheduledAtUtc,
                    DuracionMin = t.DuracionMin,
                    Estado = t.Estado,

                    MedicoNombre = t.Medico.Usuario.Nombre + " " + t.Medico.Usuario.Apellido,
                    EspecialidadNombre = t.Medico.Especialidades
                        .Select(e => e.Especialidad.Nombre)
                        .FirstOrDefault(),

                    TipoTurnoNombre = t.TipoTurno != null ? t.TipoTurno.Nombre : null
                })
                .ToListAsync();
        }

        // ============================================================
        // 2) LISTAR AGENDA DEL MÉDICO (DÍA)
        // ============================================================
        public async Task<List<TurnoAgendaMedicoDTO>> ListarAgendaMedicoDiaAsync(
            int medicoId,
            DateTime diaUtc,
            int? pacienteIdActual = null)
        {
            DateTime desde = diaUtc.Date;
            DateTime hasta = desde.AddDays(1);

            return await _db.Turno
                .Include(t => t.Paciente).ThenInclude(p => p.Usuario)
                .Include(t => t.TipoTurno)
                .Where(t =>
                    t.MedicoId == medicoId &&
                    t.IsActive &&
                    t.ScheduledAtUtc >= desde &&
                    t.ScheduledAtUtc < hasta)
                .OrderBy(t => t.ScheduledAtUtc)
                .Select(t => new TurnoAgendaMedicoDTO
                {
                    IdTurno = t.IdTurno,
                    ScheduledAtUtc = t.ScheduledAtUtc,
                    DuracionMin = t.DuracionMin,
                    Estado = t.Estado,

                    PacienteNombre = t.Paciente != null
                        ? t.Paciente.Usuario.Nombre + " " + t.Paciente.Usuario.Apellido
                        : null,

                    EsPacienteActual = (pacienteIdActual != null && t.PacienteId == pacienteIdActual),

                    TipoTurnoNombre = t.TipoTurno != null ? t.TipoTurno.Nombre : null
                })
                .ToListAsync();
        }

        // ============================================================
        // 3) RESERVAR TURNO (PACIENTE)
        // ============================================================
        public async Task<bool> ReservarAsync(int turnoId, int pacienteId, int tipoTurnoId)
        {
            var turno = await _db.Turno.FirstOrDefaultAsync(t => t.IdTurno == turnoId && t.IsActive);
            if (turno == null) return false;

            // VALIDACIONES DE NEGOCIO
            if (turno.Estado != "Disponible") return false;

            // Asignar el turno
            turno.PacienteId = pacienteId;
            turno.TipoTurnoId = tipoTurnoId;
            turno.Estado = "Reservado";

            await _db.SaveChangesAsync();
            return true;
        }

        // ============================================================
        // 4) CANCELAR COMO PACIENTE
        // ============================================================
        public async Task<bool> CancelarComoPacienteAsync(int turnoId, int pacienteId)
        {
            var turno = await _db.Turno.FirstOrDefaultAsync(t => t.IdTurno == turnoId && t.IsActive);
            if (turno == null) return false;

            // No puede cancelar turnos ajenos
            if (turno.PacienteId != pacienteId) return false;

            // Debe estar reservado
            if (turno.Estado != "Reservado") return false;

            // Liberar turno
            turno.PacienteId = null;
            turno.TipoTurnoId = null;
            turno.Estado = "Disponible";

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<TurnoSlotDTO>> ListarSlotsPorDiaAsync(int medicoId, DateTime fechaUtc)
        {
            // Tomamos solo la parte de fecha (UTC) y generamos el rango día completo
            var inicioDiaUtc = fechaUtc.Date;                 // 00:00 UTC
            var finDiaUtc = inicioDiaUtc.AddDays(1);          // 00:00 del día siguiente

            // 1) Obtenemos el médico y su slot base
            var medico = await _db.Medico.FirstOrDefaultAsync(m => m.IdMedico == medicoId && m.IsActive);
            if (medico == null)
                return new List<TurnoSlotDTO>();

            var defaultSlot = medico.DefaultSlotMin; // ej: 30 min

            // 2) Horarios del médico para ese día de la semana
            //    ojo: DayOfWeek: 0=Sunday, 1=Monday...
            var diaSemana = (int)inicioDiaUtc.DayOfWeek;

            var horario = await _db.MedicoHorario
                .FirstOrDefaultAsync(h =>
                    h.MedicoId == medicoId &&
                    h.Activo &&
                    h.DiaSemana == diaSemana);

            if (horario == null)
                return new List<TurnoSlotDTO>(); // no trabaja ese día

            var slotMin = horario.SlotMinOverride ?? defaultSlot;

            // 3) Pasamos las horas de inicio/fin del horario al día actual (asumimos que ScheduledAtUtc ya está en UTC)
            var inicioJornadaUtc = inicioDiaUtc
                .Add(horario.HoraInicio); // HoraInicio es tipo time -> TimeSpan

            var finJornadaUtc = inicioDiaUtc
                .Add(horario.HoraFin);

            if (finJornadaUtc <= inicioJornadaUtc)
                return new List<TurnoSlotDTO>();

            // 4) Turnos existentes en ese rango
            var turnosDia = await _db.Turno
                .Include(t => t.TipoTurno)
                .Where(t =>
                    t.MedicoId == medicoId &&
                    t.IsActive &&
                    t.ScheduledAtUtc >= inicioJornadaUtc &&
                    t.ScheduledAtUtc < finJornadaUtc)
                .ToListAsync();

            // 5) Bloqueos del médico que superponen con la jornada
            var bloqueosDia = await _db.MedicoBloqueo
                .Where(b =>
                    b.MedicoId == medicoId &&
                    b.Desde < finJornadaUtc &&
                    b.Hasta > inicioJornadaUtc)
                .ToListAsync();

            var slots = new List<TurnoSlotDTO>();

            // 6) Recorremos de a slotMin minutos
            for (var cursor = inicioJornadaUtc; cursor < finJornadaUtc; cursor = cursor.AddMinutes(slotMin))
            {
                var finSlot = cursor.AddMinutes(slotMin);

                // 6.1) ¿Está bloqueado?
                bool bloqueado = bloqueosDia.Any(b =>
                    cursor < b.Hasta && finSlot > b.Desde);

                if (bloqueado)
                {
                    slots.Add(new TurnoSlotDTO
                    {
                        IdTurno = 0,
                        ScheduledAtUtc = cursor,
                        DuracionMin = slotMin,
                        EsReservable = false,
                        Estado = "Bloqueado",
                        HoraLocalTexto = cursor.ToLocalTime().ToString("HH:mm")
                    });
                    continue;
                }

                // 6.2) ¿Hay turno exacto en este horario?
                var turno = turnosDia.FirstOrDefault(t => t.ScheduledAtUtc == cursor);

                if (turno == null)
                {
                    // No hay turno generado: hueco disponible para reservar
                    slots.Add(new TurnoSlotDTO
                    {
                        IdTurno = 0, // se creará al reservar
                        ScheduledAtUtc = cursor,
                        DuracionMin = slotMin,
                        EsReservable = true,
                        Estado = "Disponible",
                        HoraLocalTexto = cursor.ToString("HH:mm")
                    });
                }
                else
                {
                    bool esReservable = turno.Estado == "Disponible";

                    slots.Add(new TurnoSlotDTO
                    {
                        IdTurno = turno.IdTurno,
                        ScheduledAtUtc = turno.ScheduledAtUtc,
                        DuracionMin = turno.DuracionMin,
                        EsReservable = esReservable,
                        Estado = turno.Estado,
                        HoraLocalTexto = turno.ScheduledAtUtc.ToString("HH:mm"),
                        TipoTurnoNombre = turno.TipoTurno?.Nombre
                    });
                }
            }

            return slots;
        }


        public async Task<bool> ReservarSlotAsync(int medicoId, DateTime fechaUtc, int pacienteId, int tipoTurnoId)
        {
            // 1) Obtenemos los slots del día
            var slots = await ListarSlotsPorDiaAsync(medicoId, fechaUtc.Date);

            // Buscamos el slot exacto (misma hora exacta)
            var slot = slots.FirstOrDefault(s =>
                s.ScheduledAtUtc == fechaUtc &&
                s.EsReservable &&
                s.IdTurno == 0 &&
                s.Estado == "Disponible");

            if (slot == null)
                return false;

            // 2) Validamos el tipo de turno
            var tipo = await _db.TipoTurno.FirstOrDefaultAsync(t => t.IdTipoTurno == tipoTurnoId && t.Activo);
            if (tipo == null)
                return false;

            // 3) Creamos el Turno nuevo ya reservado
            var turno = new Turno
            {
                MedicoId = medicoId,
                PacienteId = pacienteId,
                TipoTurnoId = tipoTurnoId,
                ScheduledAtUtc = slot.ScheduledAtUtc,
                DuracionMin = tipo.DuracionMin, // o slot.DuracionMin
                Estado = "Reservado",
                IsActive = true
            };

            _db.Turno.Add(turno);
            await _db.SaveChangesAsync();

            return true;
        }



    }
}
