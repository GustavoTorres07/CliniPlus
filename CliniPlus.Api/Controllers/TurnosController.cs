using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurnosController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITurnoRepository _repo;

        public TurnosController(AppDbContext db, ITurnoRepository repo)
        {
            _db = db;
            _repo = repo;
        }

        // ============================================================
        // Helper para obtener IdUsuario desde el token
        // ============================================================
        private int? GetUsuarioId()
        {
            var sub = User.FindFirst("sub")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(sub, out var id))
                return id;

            return null;
        }

        // ============================================================
        // 1) LISTAR TURNOS DEL PACIENTE LOGUEADO
        // GET api/turnos/paciente/mis-turnos
        // ============================================================
        [HttpGet("paciente/mis-turnos")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult<List<TurnoListadoPacienteDTO>>> MisTurnos()
        {
            var usuarioId = GetUsuarioId();
            if (usuarioId is null)
                return Unauthorized("No se pudo identificar al usuario.");

            var paciente = await _db.Paciente
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.IsActive);

            if (paciente == null)
                return Unauthorized("Paciente no encontrado para este usuario.");

            var lista = await _repo.ListarPorPacienteAsync(paciente.IdPaciente);
            return Ok(lista);
        }

        // ============================================================
        // 2) RESERVAR UN TURNO COMO PACIENTE
        // POST api/turnos/paciente/reservar
        // Body: TurnoReservarDTO { TurnoId, TipoTurnoId }
        // ============================================================
        [HttpPost("paciente/reservar")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult> Reservar([FromBody] TurnoReservarDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            var usuarioId = GetUsuarioId();
            if (usuarioId is null)
                return Unauthorized("No se pudo identificar al usuario.");

            var paciente = await _db.Paciente
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.IsActive);

            if (paciente == null)
                return Unauthorized("Paciente no encontrado para este usuario.");

            var ok = await _repo.ReservarAsync(dto.TurnoId, paciente.IdPaciente, dto.TipoTurnoId);

            if (!ok)
                return BadRequest("No se pudo reservar el turno (ya reservado o no disponible).");

            return NoContent();
        }

        // ============================================================
        // 3) CANCELAR UN TURNO COMO PACIENTE
        // POST api/turnos/paciente/cancelar
        // Body: TurnoCancelarDTO { TurnoId }
        // ============================================================
        [HttpPost("paciente/cancelar")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult> Cancelar([FromBody] TurnoCancelarDTO dto)
        {
            var usuarioId = GetUsuarioId();
            if (usuarioId is null)
                return Unauthorized("No se pudo identificar al usuario.");

            var paciente = await _db.Paciente
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.IsActive);

            if (paciente == null)
                return Unauthorized("Paciente no encontrado para este usuario.");

            var ok = await _repo.CancelarComoPacienteAsync(dto.TurnoId, paciente.IdPaciente);

            if (!ok)
                return BadRequest("No se pudo cancelar el turno (no pertenece al paciente o no está reservado).");

            return NoContent();
        }

        // ============================================================
        // 4) AGENDA DEL MÉDICO (HOY)
        // GET api/turnos/medico/agenda-hoy
        // ============================================================
        [HttpGet("medico/agenda-hoy")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult<List<TurnoAgendaMedicoDTO>>> AgendaHoy()
        {
            var usuarioId = GetUsuarioId();
            if (usuarioId is null)
                return Unauthorized("No se pudo identificar al usuario.");

            var medico = await _db.Medico
                .FirstOrDefaultAsync(m => m.UsuarioId == usuarioId && m.IsActive);

            if (medico == null)
                return Unauthorized("Médico no encontrado para este usuario.");

            // Usamos la fecha de hoy en UTC, como base
            var hoyUtc = DateTime.UtcNow.Date;

            var lista = await _repo.ListarAgendaMedicoDiaAsync(medico.IdMedico, hoyUtc, null);

            return Ok(lista);
        }

        [HttpGet("paciente/slots")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult<List<TurnoSlotDTO>>> SlotsPaciente(
    int medicoId,
    string fecha )
        {
            // Parseamos la fecha (sin hora)
            if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var fechaUtc))
            {
                return BadRequest("Formato de fecha inválido. Use yyyy-MM-dd.");
            }

            var slots = await _repo.ListarSlotsPorDiaAsync(medicoId, fechaUtc);
            return Ok(slots);
        }



        [HttpPost("paciente/reservar-slot")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult> ReservarSlot([FromBody] TurnoReservarSlotDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            var usuarioId = GetUsuarioId();
            if (usuarioId is null)
                return Unauthorized("No se pudo identificar al usuario.");

            var paciente = await _db.Paciente
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.IsActive);

            if (paciente == null)
                return Unauthorized("Paciente no encontrado para este usuario.");

            var ok = await _repo.ReservarSlotAsync(dto.MedicoId, dto.ScheduledAtUtc, paciente.IdPaciente, dto.TipoTurnoId);

            if (!ok)
                return BadRequest("No se pudo reservar ese horario (no disponible o inválido).");

            return NoContent();
        }




    }
}
