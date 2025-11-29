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
                .Include(t => t.Medico).ThenInclude(m => m.Especialidad)   // 👈 UNA sola especialidad
                .Include(t => t.TipoTurno)
                .Where(t => t.PacienteId == pacienteId && t.IsActive)
                .OrderBy(t => t.ScheduledAtUtc)
                .Select(t => new TurnoListadoPacienteDTO
                {
                    IdTurno = t.IdTurno,
                    ScheduledAtUtc = t.ScheduledAtUtc,
                    DuracionMin = t.DuracionMin,
                    Estado = t.Estado,

                    MedicoNombre = t.Medico.Usuario.Nombre + " " + t.Medico.Usuario.Apellido,
                    EspecialidadNombre = t.Medico.Especialidad != null
                        ? t.Medico.Especialidad.Nombre
                        : null,

                    TipoTurnoNombre = t.TipoTurno != null ? t.TipoTurno.Nombre : null
                })
                .ToListAsync();
        }

        // ============================================================
        // 2) LISTAR AGENDA DEL MÉDICO (DÍA)
        // ============================================================
        public async Task<List<TurnoAgendaMedicoDTO>> ListarAgendaMedicoDiaAsync(
            int medicoId,
            DateTime diaLocal,
            int? pacienteIdActual = null)
        {
            DateTime desde = diaLocal.Date;
            DateTime hasta = desde.AddDays(1);

            return await _db.Turno
                .Include(t => t.Paciente)
                    .ThenInclude(p => p!.Usuario)
                .Include(t => t.TipoTurno)
                .Where(t =>
                    t.MedicoId == medicoId &&
                    t.IsActive &&
                    t.ScheduledAtUtc >= desde &&
                    t.ScheduledAtUtc < hasta &&
                    t.Estado != "Disponible" &&      // 👈 NO mostrar slots libres
                    t.Estado != "Cancelado")         // 👈 NO mostrar cancelados
                .OrderBy(t => t.ScheduledAtUtc)
                .Select(t => new TurnoAgendaMedicoDTO
                {
                    IdTurno = t.IdTurno,
                    ScheduledAtUtc = t.ScheduledAtUtc,
                    DuracionMin = t.DuracionMin,
                    Estado = t.Estado,

                    PacienteNombre = t.Paciente != null && t.Paciente.Usuario != null
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

        // ============================================================
        // 5) LISTAR SLOTS POR DÍA
        // ============================================================
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
            //    DayOfWeek: 0=Sunday, 1=Monday...
            var diaSemana = (int)inicioDiaUtc.DayOfWeek;

            var horario = await _db.MedicoHorario
                .FirstOrDefaultAsync(h =>
                    h.MedicoId == medicoId &&
                    h.Activo &&
                    h.DiaSemana == diaSemana);

            if (horario == null)
                return new List<TurnoSlotDTO>(); // no trabaja ese día

            var slotMin = horario.SlotMinOverride ?? defaultSlot;

            // 3) Inicio / fin de jornada
            var inicioJornadaUtc = inicioDiaUtc.Add(horario.HoraInicio);
            var finJornadaUtc = inicioDiaUtc.Add(horario.HoraFin);

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

            // 5) Bloqueos del médico
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
                    // Hueco disponible
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

        // ============================================================
        // 6) RESERVAR SLOT (GENERANDO TURNO)
        // ============================================================
        public async Task<bool> ReservarSlotAsync(int medicoId, DateTime fechaUtc, int pacienteId, int tipoTurnoId)
        {
            // 1) Obtenemos los slots del día
            var slots = await ListarSlotsPorDiaAsync(medicoId, fechaUtc.Date);

            // Buscamos el slot exacto
            var slot = slots.FirstOrDefault(s =>
                s.ScheduledAtUtc == fechaUtc &&
                s.EsReservable &&
                s.IdTurno == 0 &&
                s.Estado == "Disponible");

            if (slot == null)
                return false;

            // 2) Validar tipo de turno
            var tipo = await _db.TipoTurno
                .FirstOrDefaultAsync(t => t.IdTipoTurno == tipoTurnoId && t.Activo);

            if (tipo == null)
                return false;

            // 3) Creamos el Turno ya reservado
            var turno = new Turno
            {
                MedicoId = medicoId,
                PacienteId = pacienteId,
                TipoTurnoId = tipoTurnoId,
                ScheduledAtUtc = slot.ScheduledAtUtc,
                DuracionMin = tipo.DuracionMin,
                Estado = "Reservado",
                IsActive = true
            };

            _db.Turno.Add(turno);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<List<TurnoAgendaMedicoDTO>> ObtenerAgendaHoyMedicoAsync(int medicoId, DateTime hoyUtc)
        {
            var desdeUtc = hoyUtc.Date;
            var hastaUtc = desdeUtc.AddDays(1);

            var ahoraUtc = DateTime.UtcNow;

            return await _db.Turno
                .Where(t =>
                    t.MedicoId == medicoId &&
                    t.IsActive &&
                    t.ScheduledAtUtc >= desdeUtc &&
                    t.ScheduledAtUtc < hastaUtc &&
                    t.Estado != "Disponible" &&   // 👈 no libres
                    t.Estado != "Cancelado")      // 👈 no cancelados
                .OrderBy(t => t.ScheduledAtUtc)
                .Select(t => new TurnoAgendaMedicoDTO
                {
                    IdTurno = t.IdTurno,
                    ScheduledAtUtc = t.ScheduledAtUtc,
                    DuracionMin = t.DuracionMin,
                    Estado = t.Estado,

                    PacienteId = t.PacienteId,
                    PacienteNombre = t.Paciente != null
                        ? (t.Paciente.Usuario != null
                            ? t.Paciente.Usuario.Nombre + " " + t.Paciente.Usuario.Apellido
                            : t.Paciente.DNI)
                        : null,

                    TipoTurnoNombre = t.TipoTurno != null ? t.TipoTurno.Nombre : null,

                    EsPacienteActual =
                        t.Estado == "Reservado" &&
                        t.ScheduledAtUtc <= ahoraUtc &&
                        t.ScheduledAtUtc.AddMinutes(t.DuracionMin) >= ahoraUtc
                })
                .ToListAsync();
        }



        public async Task<TurnoDetalleMedicoDTO?> ObtenerDetalleMedicoAsync(int medicoId, int turnoId)
        {
            return await _db.Turno
                .Where(t => t.IdTurno == turnoId &&
                            t.MedicoId == medicoId &&
                            t.IsActive)
                .Select(t => new TurnoDetalleMedicoDTO
                {
                    IdTurno = t.IdTurno,
                    ScheduledAtUtc = t.ScheduledAtUtc,
                    DuracionMin = t.DuracionMin,
                    Estado = t.Estado,
                    TipoTurnoNombre = t.TipoTurno != null ? t.TipoTurno.Nombre : "",

                    PacienteId = t.PacienteId ?? 0,
                    PacienteNombre = t.Paciente != null && t.Paciente.Usuario != null
                        ? t.Paciente.Usuario.Nombre + " " + t.Paciente.Usuario.Apellido
                        : "Paciente sin usuario",
                    PacienteDni = t.Paciente != null ? t.Paciente.DNI : null,
                    PacienteTelefono = t.Paciente != null ? t.Paciente.Telefono : null,
                    PacienteEmail = t.Paciente != null ? t.Paciente.Email : null,

                    ObraSocialNombre = t.Paciente != null && t.Paciente.ObraSocial != null
                        ? t.Paciente.ObraSocial.Nombre
                        : null,
                    NumeroAfiliado = t.Paciente != null ? t.Paciente.NumeroAfiliado : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<HistoriaClinicaItemDTO>> ObtenerHistoriaRapidaPacienteAsync(
            int medicoId,
            int pacienteId)
        {
            return await _db.HistoriaClinicaEntrada
                .Where(h =>
                    h.MedicoId == medicoId &&
                    h.PacienteId == pacienteId)
                .OrderByDescending(h => h.Fecha)
                .Take(10)
                .Select(h => new HistoriaClinicaItemDTO
                {
                    IdEntrada = h.IdEntrada,
                    Fecha = h.Fecha,
                    CIE10Codigo = h.CIE10Codigo,
                    CIE10Descripcion = h.CIE10 != null ? h.CIE10.Descripcion : "",
                    Notas = h.Notas
                })
                .ToListAsync();
        }

        public async Task<int?> RegistrarConsultaAsync(int medicoId, RegistrarConsultaMedicoDTO dto)
        {
            var turno = await _db.Turno
                .Include(t => t.Paciente)
                .FirstOrDefaultAsync(t =>
                    t.IdTurno == dto.TurnoId &&
                    t.MedicoId == medicoId &&
                    t.IsActive);

            if (turno == null)
                return null;

            if (turno.PacienteId == null)
                return null;

            // 1) Crear la consulta
            var consulta = new Consulta
            {
                TurnoId = turno.IdTurno,
                MedicoId = medicoId,
                Notas = dto.Notas
                // FechaHora usa el default DateTime.UtcNow del modelo
            };

            _db.Consulta.Add(consulta);
            await _db.SaveChangesAsync();   // para obtener IdConsulta

            // 2) Diagnósticos CIE-10 (solo si hay)
            if (dto.Diagnosticos != null && dto.Diagnosticos.Count > 0)
            {
                foreach (var d in dto.Diagnosticos)
                {
                    var diag = new ConsultaDiagnostico
                    {
                        ConsultaId = consulta.IdConsulta,
                        CIE10Codigo = d.CIE10Codigo,
                        Comentario = d.Comentario,
                        Principal = d.Principal
                    };

                    _db.ConsultaDiagnostico.Add(diag);
                }

                // 3) Historia clínica rápida SOLO si hay CIE-10
                var ciePrincipal = dto.Diagnosticos
                    .FirstOrDefault(x => x.Principal)?.CIE10Codigo
                    ?? dto.Diagnosticos.First().CIE10Codigo;

                var entrada = new HistoriaClinicaEntrada
                {
                    PacienteId = turno.PacienteId.Value,
                    MedicoId = medicoId,
                    Fecha = DateTime.UtcNow,
                    CIE10Codigo = ciePrincipal,
                    Notas = dto.Notas,
                    TurnoId = turno.IdTurno,
                    ConsultaId = consulta.IdConsulta
                };

                _db.HistoriaClinicaEntrada.Add(entrada);
            }

            // 4) Actualizar estado del turno según la lógica de negocio
            switch (dto.EstadoFinal)
            {
                case TurnoEstadoDTO.Atendido:
                    turno.Estado = "Atendido";
                    break;

                case TurnoEstadoDTO.NoAsistio:
                    turno.Estado = "No Asistió";
                    break;

                default:
                    turno.Estado = "Atendido";
                    break;
            }

            await _db.SaveChangesAsync();

            return consulta.IdConsulta;
        }


        public async Task<bool> MarcarTurnoCompletadoAsync(int medicoId, int turnoId)
        {
            var turno = await _db.Turno
                .FirstOrDefaultAsync(t =>
                    t.IdTurno == turnoId &&
                    t.MedicoId == medicoId &&
                    t.IsActive);

            if (turno == null)
                return false;

            if (turno.Estado == "Atendido")
                return true;

            turno.Estado = "Atendido";
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<List<PacienteListadoMedicoDTO>> ListarPacientesPorMedicoAsync(int medicoId)
        {
            // 1) Agrupamos turnos por PacienteId
            var grupos = await _db.Turno
                .Where(t => t.MedicoId == medicoId &&
                            t.PacienteId != null &&
                            t.IsActive)
                .GroupBy(t => t.PacienteId)
                .Select(g => new
                {
                    PacienteId = g.Key!.Value,
                    TurnosTotales = g.Count(),
                    UltimoTurnoUtc = g.Max(t => t.ScheduledAtUtc)
                })
                .ToListAsync();

            if (grupos.Count == 0)
                return new List<PacienteListadoMedicoDTO>();

            // 2) Traemos los pacientes correspondientes (con Usuario)
            var pacienteIds = grupos.Select(x => x.PacienteId).ToList();

            var pacientes = await _db.Paciente
                .Include(p => p.Usuario)
                .Where(p => pacienteIds.Contains(p.IdPaciente))
                .ToListAsync();

            // 3) Armamos DTO mezclando grupo + paciente
            var resultado = (from g in grupos
                             join p in pacientes on g.PacienteId equals p.IdPaciente
                             select new PacienteListadoMedicoDTO
                             {
                                 PacienteId = p.IdPaciente,
                                 NombreCompleto = p.Usuario != null
                                     ? p.Usuario.Nombre + " " + p.Usuario.Apellido
                                     : p.DNI,
                                 DNI = p.DNI,
                                 Email = p.Email,
                                 Telefono = p.Telefono,
                                 TurnosTotales = g.TurnosTotales,
                                 UltimoTurnoUtc = g.UltimoTurnoUtc
                             })
                            .OrderByDescending(x => x.UltimoTurnoUtc)
                            .ToList();

            return resultado;
        }

        public async Task<List<HistoriaClinicaListadoDTO>> ObtenerHistoriaClinicaPacienteAsync(
    int medicoId,
    int pacienteId,
    DateTime? desde,
    DateTime? hasta)
        {
            var query = _db.HistoriaClinicaEntrada
                .Where(h => h.PacienteId == pacienteId && h.MedicoId == medicoId)
                .Include(h => h.CIE10)
                .OrderByDescending(h => h.Fecha)
                .AsQueryable();

            if (desde.HasValue)
            {
                var d = desde.Value.Date;
                query = query.Where(h => h.Fecha >= d);
            }

            if (hasta.HasValue)
            {
                var hFecha = hasta.Value.Date.AddDays(1);
                query = query.Where(h => h.Fecha < hFecha);
            }

            return await query
                .Select(h => new HistoriaClinicaListadoDTO
                {
                    IdEntrada = h.IdEntrada,
                    Fecha = h.Fecha,
                    CIE10Codigo = h.CIE10Codigo,
                    CIE10Descripcion = h.CIE10 != null ? h.CIE10.Descripcion : "",
                    NotasResumen = h.Notas != null && h.Notas.Length > 100
                        ? h.Notas.Substring(0, 100) + "..."
                        : h.Notas,
                    TurnoId = h.TurnoId,
                    ConsultaId = h.ConsultaId
                })
                .ToListAsync();
        }

        public async Task<HistoriaClinicaDetalleDTO?> ObtenerHistoriaClinicaDetalleAsync(
            int medicoId,
            int entradaId)
        {
            return await _db.HistoriaClinicaEntrada
                .Where(h => h.IdEntrada == entradaId && h.MedicoId == medicoId)
                .Include(h => h.CIE10)
                .Include(h => h.Turno)
                    .ThenInclude(t => t.TipoTurno)
                .Include(h => h.Consulta)
                .Select(h => new HistoriaClinicaDetalleDTO
                {
                    IdEntrada = h.IdEntrada,
                    Fecha = h.Fecha,
                    CIE10Codigo = h.CIE10Codigo,
                    CIE10Descripcion = h.CIE10 != null ? h.CIE10.Descripcion : "",
                    Notas = h.Notas,

                    TurnoId = h.TurnoId,
                    TurnoFechaHora = h.Turno != null ? h.Turno.ScheduledAtUtc : null,
                    TipoTurnoNombre = h.Turno != null && h.Turno.TipoTurno != null
                        ? h.Turno.TipoTurno.Nombre
                        : null,
                    EstadoTurno = h.Turno != null ? h.Turno.Estado : null,

                    ConsultaId = h.ConsultaId,
                    ConsultaFechaHora = h.Consulta != null ? h.Consulta.FechaHora : null,

                            // 👉 AQUÍ VA MedicoNombre:
        MedicoNombre = h.Consulta != null
                       && h.Consulta.Medico != null
                       && h.Consulta.Medico.Usuario != null
            ? h.Consulta.Medico.Usuario.Nombre + " " + h.Consulta.Medico.Usuario.Apellido
            : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PacienteDetalleMedicoDTO?> ObtenerPacienteDetalleAsync(
            int medicoId,
            int pacienteId)
        {
            // Opcional: podrías verificar que el paciente tenga al menos un turno con este médico.
            var paciente = await _db.Paciente
                .Include(p => p.Usuario)
                .Include(p => p.ObraSocial)
                .Include(p => p.FichaMedica)
                .FirstOrDefaultAsync(p => p.IdPaciente == pacienteId && p.IsActive);

            if (paciente == null)
                return null;

            return new PacienteDetalleMedicoDTO
            {
                PacienteId = paciente.IdPaciente,
                NombreCompleto = paciente.Usuario != null
                    ? paciente.Usuario.Nombre + " " + paciente.Usuario.Apellido
                    : paciente.DNI,
                DNI = paciente.DNI,
                Email = paciente.Email,
                Telefono = paciente.Telefono,
                ObraSocialNombre = paciente.ObraSocial != null ? paciente.ObraSocial.Nombre : null,
                NumeroAfiliado = paciente.NumeroAfiliado,
                Ficha = paciente.FichaMedica != null
                    ? new FichaMedicaDTO
                    {
                        PacienteId = paciente.IdPaciente,
                        GrupoSanguineo = paciente.FichaMedica.GrupoSanguineo,
                        Alergias = paciente.FichaMedica.Alergias,
                        EnfermedadesCronicas = paciente.FichaMedica.EnfermedadesCronicas,
                        MedicacionHabitual = paciente.FichaMedica.MedicacionHabitual,
                        Observaciones = paciente.FichaMedica.Observaciones
                    }
                    : null
            };
        }

        public async Task<List<HistoriaClinicaListadoDTO>> ObtenerHistoriaPacienteAsync(
    int pacienteId,
    DateTime? desde,
    DateTime? hasta)
        {
            var query = _db.HistoriaClinicaEntrada
                .Where(h => h.PacienteId == pacienteId)
                .Include(h => h.CIE10)
                .OrderByDescending(h => h.Fecha)
                .AsQueryable();

            if (desde.HasValue)
            {
                var d = desde.Value.Date;
                query = query.Where(h => h.Fecha >= d);
            }

            if (hasta.HasValue)
            {
                var hFin = hasta.Value.Date.AddDays(1);
                query = query.Where(h => h.Fecha < hFin);
            }

            return await query
                .Select(h => new HistoriaClinicaListadoDTO
                {
                    IdEntrada = h.IdEntrada,
                    Fecha = h.Fecha,
                    CIE10Codigo = h.CIE10Codigo,
                    CIE10Descripcion = h.CIE10 != null ? h.CIE10.Descripcion : "",
                    NotasResumen = h.Notas != null && h.Notas.Length > 100
                        ? h.Notas.Substring(0, 100) + "..."
                        : h.Notas,
                    TurnoId = h.TurnoId,
                    ConsultaId = h.ConsultaId
                })
                .ToListAsync();
        }

        public async Task<HistoriaClinicaDetalleDTO?> ObtenerHistoriaDetallePacienteAsync(
            int pacienteId,
            int entradaId)
        {
            return await _db.HistoriaClinicaEntrada
                .Where(h => h.IdEntrada == entradaId && h.PacienteId == pacienteId)
                .Include(h => h.CIE10)
                .Include(h => h.Turno)
                    .ThenInclude(t => t.TipoTurno)
                .Include(h => h.Consulta)
                .Select(h => new HistoriaClinicaDetalleDTO
                {
                    IdEntrada = h.IdEntrada,
                    Fecha = h.Fecha,
                    CIE10Codigo = h.CIE10Codigo,
                    CIE10Descripcion = h.CIE10 != null ? h.CIE10.Descripcion : "",
                    Notas = h.Notas,

                    TurnoId = h.TurnoId,
                    TurnoFechaHora = h.Turno != null ? h.Turno.ScheduledAtUtc : null,
                    TipoTurnoNombre = h.Turno != null && h.Turno.TipoTurno != null
                        ? h.Turno.TipoTurno.Nombre
                        : null,
                    EstadoTurno = h.Turno != null ? h.Turno.Estado : null,

                    ConsultaId = h.ConsultaId,
                    ConsultaFechaHora = h.Consulta != null ? h.Consulta.FechaHora : null,

                            MedicoNombre = h.Consulta != null
                       && h.Consulta.Medico != null
                       && h.Consulta.Medico.Usuario != null
            ? h.Consulta.Medico.Usuario.Nombre + " " + h.Consulta.Medico.Usuario.Apellido
            : null
                })
                .FirstOrDefaultAsync();
        }




    }
}
