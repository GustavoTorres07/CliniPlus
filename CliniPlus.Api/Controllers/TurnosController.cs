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

            var hoyLocal = DateTime.Now.Date;

            var lista = await _repo.ListarAgendaMedicoDiaAsync(medico.IdMedico, hoyLocal, null);

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

        [HttpGet("public/slots")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TurnoSlotDTO>>> SlotsPublic(
    int medicoId,
    string fecha // "yyyy-MM-dd"
)
        {
            if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out var fechaUtc))
            {
                return BadRequest("Formato de fecha inválido. Use yyyy-MM-dd.");
            }

            var slots = await _repo.ListarSlotsPorDiaAsync(medicoId, fechaUtc);
            return Ok(slots);
        }

        [HttpPost("public/reservar")]
        [AllowAnonymous]
        public async Task<ActionResult> ReservarPublico([FromBody] TurnoPublicoReservarDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            if (string.IsNullOrWhiteSpace(dto.DNI) ||
                string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Apellido))
            {
                return BadRequest("DNI, Nombre y Apellido son obligatorios.");
            }

            // 1) Buscar paciente por DNI (reuse registro si existe)
            var paciente = await _db.Paciente
                .FirstOrDefaultAsync(p => p.DNI == dto.DNI);

            if (paciente == null)
            {
                // 2) Crear Paciente Provisional (IsProvisional = true)
                paciente = new CliniPlus.Shared.Models.Paciente
                {
                    DNI = dto.DNI,
                    IsProvisional = true,
                    ObraSocialId = dto.ObraSocialId,
                    NumeroAfiliado = dto.NumeroAfiliado,
                    IsActive = true
                    // UsuarioId queda null -> sin acceso a la app por ahora (RN11)
                };

                _db.Paciente.Add(paciente);
                await _db.SaveChangesAsync();
            }
            else
            {
                // Si estaba dado de baja lógica, lo reactivamos
                if (!paciente.IsActive)
                    paciente.IsActive = true;

                // Actualizar obra social si viene
                if (dto.ObraSocialId.HasValue)
                    paciente.ObraSocialId = dto.ObraSocialId.Value;

                if (!string.IsNullOrWhiteSpace(dto.NumeroAfiliado))
                    paciente.NumeroAfiliado = dto.NumeroAfiliado;

                await _db.SaveChangesAsync();
            }

            // 3) Reservar el turno usando la misma lógica que el Paciente logueado
            var ok = await _repo.ReservarAsync(dto.TurnoId, paciente.IdPaciente, dto.TipoTurnoId);

            if (!ok)
                return BadRequest("No se pudo reservar el turno (ya reservado o no disponible).");

            // Podrías devolver info extra si querés (Ej: código de reserva)
            return Ok(new
            {
                Mensaje = "Turno reservado correctamente.",
                PacienteId = paciente.IdPaciente
            });
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

        // =============== MÉDICO ===============

        [HttpGet("medico/detalle/{turnoId:int}")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult<TurnoDetalleMedicoDTO>> GetDetalleMedico(int turnoId)
        {
            int medicoId = await ResolverMedicoIdAsync();
            var dto = await _repo.ObtenerDetalleMedicoAsync(medicoId, turnoId);

            if (dto == null)
                return NotFound("Turno no encontrado para este médico.");

            return Ok(dto);
        }

        [HttpGet("medico/historia-rapida/{pacienteId:int}")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult<List<HistoriaClinicaItemDTO>>> GetHistoriaRapida(int pacienteId)
        {
            int medicoId = await ResolverMedicoIdAsync();
            var lista = await _repo.ObtenerHistoriaRapidaPacienteAsync(medicoId, pacienteId);
            return Ok(lista);
        }

        [HttpPost("medico/registrar-consulta")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult> RegistrarConsulta([FromBody] RegistrarConsultaMedicoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            int medicoId = await ResolverMedicoIdAsync();

            var consultaId = await _repo.RegistrarConsultaAsync(medicoId, dto);

            if (consultaId == null)
                return BadRequest("No se pudo registrar la consulta.");

            return Ok(new { ConsultaId = consultaId.Value });
        }

        [HttpPost("medico/completar/{turnoId:int}")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult> MarcarCompletado(int turnoId)
        {
            int medicoId = await ResolverMedicoIdAsync();
            var ok = await _repo.MarcarTurnoCompletadoAsync(medicoId, turnoId);

            return ok ? NoContent() : BadRequest("No se pudo marcar el turno como completado.");
        }

        // LISTAR PACIENTES DEL MÉDICO
        [HttpGet("medico/pacientes")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult<List<PacienteListadoMedicoDTO>>> GetPacientesMedico()
        {
            int medicoId = await ResolverMedicoIdAsync();

            var lista = await _repo.ListarPacientesPorMedicoAsync(medicoId);

            return Ok(lista);
        }

        // AGENDA DEL MÉDICO POR DÍA
        // GET api/turnos/medico/agenda-dia?fecha=yyyy-MM-dd
        [HttpGet("medico/agenda-dia")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult<List<TurnoAgendaMedicoDTO>>> AgendaDia([FromQuery] string? fecha)
        {
            int medicoId = await ResolverMedicoIdAsync();

            DateTime fechaLocal;

            if (!string.IsNullOrWhiteSpace(fecha) &&
                DateTime.TryParseExact(fecha, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
            {
                fechaLocal = parsed.Date;
            }
            else
            {
                fechaLocal = DateTime.Now.Date;
            }

            var lista = await _repo.ListarAgendaMedicoDiaAsync(medicoId, fechaLocal, null);

            return Ok(lista);
        }

        // -------- helper para obtener MedicoId desde el token --------

        private async Task<int> ResolverMedicoIdAsync()
        {
            var sub = User.FindFirst("sub")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(sub))
                throw new UnauthorizedAccessException("Token inválido.");

            int usuarioId = int.Parse(sub);

            var medico = await _db.Medico
                .FirstOrDefaultAsync(m => m.UsuarioId == usuarioId && m.IsActive);

            if (medico == null)
                throw new UnauthorizedAccessException("El usuario no tiene perfil de médico activo.");

            return medico.IdMedico;
        }

        [HttpGet("medico/historia-clinica/{pacienteId:int}")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult<List<HistoriaClinicaListadoDTO>>> GetHistoriaClinica(
    int pacienteId,
    [FromQuery] string? desde,
    [FromQuery] string? hasta)
        {
            int medicoId = await ResolverMedicoIdAsync();

            DateTime? desdeDt = null;
            DateTime? hastaDt = null;

            if (!string.IsNullOrWhiteSpace(desde) &&
                DateTime.TryParseExact(desde, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var d))
            {
                desdeDt = d;
            }

            if (!string.IsNullOrWhiteSpace(hasta) &&
                DateTime.TryParseExact(hasta, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var h))
            {
                hastaDt = h;
            }

            var lista = await _repo.ObtenerHistoriaClinicaPacienteAsync(medicoId, pacienteId, desdeDt, hastaDt);
            return Ok(lista);
        }

        // Detalle de una entrada de historia clínica
        // GET: api/turnos/medico/historia-clinica/detalle/{entradaId}
        [HttpGet("medico/historia-clinica/detalle/{entradaId:int}")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult<HistoriaClinicaDetalleDTO>> GetHistoriaDetalle(int entradaId)
        {
            int medicoId = await ResolverMedicoIdAsync();

            var dto = await _repo.ObtenerHistoriaClinicaDetalleAsync(medicoId, entradaId);
            if (dto == null)
                return NotFound("Entrada de historia clínica no encontrada.");

            return Ok(dto);
        }

        // Detalle del paciente para el médico
        // GET: api/turnos/medico/pacientes/{pacienteId}
        [HttpGet("medico/pacientes/{pacienteId:int}")]
        [Authorize(Roles = "Medico")]
        public async Task<ActionResult<PacienteDetalleMedicoDTO>> GetPacienteDetalle(int pacienteId)
        {
            int medicoId = await ResolverMedicoIdAsync();

            var dto = await _repo.ObtenerPacienteDetalleAsync(medicoId, pacienteId);
            if (dto == null)
                return NotFound("Paciente no encontrado.");

            return Ok(dto);
        }

        private async Task<int> ResolverPacienteIdAsync()
        {
            var usuarioId = GetUsuarioId();
            if (usuarioId is null)
                throw new UnauthorizedAccessException("Token inválido.");

            var paciente = await _db.Paciente
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.IsActive);

            if (paciente == null)
                throw new UnauthorizedAccessException("El usuario no tiene perfil de paciente activo.");

            return paciente.IdPaciente;
        }


        [HttpGet("paciente/historia")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult<List<HistoriaClinicaListadoDTO>>> GetMiHistoria(
    [FromQuery] string? desde,
    [FromQuery] string? hasta)
        {
            int pacienteId = await ResolverPacienteIdAsync();

            DateTime? desdeDt = null;
            DateTime? hastaDt = null;

            if (!string.IsNullOrWhiteSpace(desde) &&
                DateTime.TryParseExact(desde, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var d))
            {
                desdeDt = d;
            }

            if (!string.IsNullOrWhiteSpace(hasta) &&
                DateTime.TryParseExact(hasta, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var h))
            {
                hastaDt = h;
            }

            var lista = await _repo.ObtenerHistoriaPacienteAsync(pacienteId, desdeDt, hastaDt);
            return Ok(lista);
        }

        // GET: api/turnos/paciente/historia/{entradaId}
        [HttpGet("paciente/historia/{entradaId:int}")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult<HistoriaClinicaDetalleDTO>> GetMiHistoriaDetalle(int entradaId)
        {
            int pacienteId = await ResolverPacienteIdAsync();

            var dto = await _repo.ObtenerHistoriaDetallePacienteAsync(pacienteId, entradaId);
            if (dto == null)
                return NotFound("Entrada de historia clínica no encontrada.");

            return Ok(dto);
        }


        [HttpGet("secretaria/turnos-hoy")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<List<TurnoListadoSecretariaDTO>>> GetTurnosHoySecretaria()
        {
            var hoyLocal = DateTime.Now.Date;
            var lista = await _repo.ListarTurnosHoySecretariaAsync(hoyLocal);
            return Ok(lista);
        }

        // POST: api/turnos/secretaria/cancelar
        // POST: api/turnos/secretaria/cancelar
        [HttpPost("secretaria/cancelar")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult> CancelarComoSecretaria([FromBody] TurnoCancelarSecretariaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            // 👇 OJO: solo pasamos el TurnoId, sin motivo
            var ok = await _repo.CancelarPorSecretariaAsync(dto.TurnoId);

            if (!ok)
                return BadRequest("No se pudo cancelar el turno (no está reservado o no existe).");

            return NoContent();
        }


        [HttpGet("secretaria/agenda")]
        [Authorize(Policy = "SecretariaOAdministrador")]
        public async Task<ActionResult<List<TurnoListadoSecretariaDTO>>> GetAgendaSecretaria(
    [FromQuery] int medicoId,
    [FromQuery] string? desde,
    [FromQuery] string? hasta)
        {
            if (medicoId <= 0)
                return BadRequest("Debe indicar un médico.");

            DateTime? desdeDt = null;
            DateTime? hastaDt = null;

            if (!string.IsNullOrWhiteSpace(desde) &&
                DateTime.TryParseExact(desde, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var d))
            {
                desdeDt = d;
            }

            if (!string.IsNullOrWhiteSpace(hasta) &&
                DateTime.TryParseExact(hasta, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var h))
            {
                hastaDt = h;
            }

            var lista = await _repo.ListarAgendaSecretariaAsync(medicoId, desdeDt, hastaDt);
            return Ok(lista);
        }

        [HttpGet("secretaria/historia/{pacienteId:int}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<List<HistoriaClinicaListadoDTO>>> GetHistoriaSecretaria(
    int pacienteId,
    [FromQuery] string? desde,
    [FromQuery] string? hasta)
        {
            DateTime? desdeDt = null;
            DateTime? hastaDt = null;

            if (!string.IsNullOrWhiteSpace(desde) &&
                DateTime.TryParseExact(desde, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var d))
            {
                desdeDt = d;
            }

            if (!string.IsNullOrWhiteSpace(hasta) &&
                DateTime.TryParseExact(hasta, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var h))
            {
                hastaDt = h;
            }

            // 👇 reutilizamos la misma lógica que para el paciente,
            // pero SIN depender del usuario logueado
            var lista = await _repo.ObtenerHistoriaPacienteAsync(pacienteId, desdeDt, hastaDt);
            return Ok(lista);
        }

        // GET: api/turnos/secretaria/historia/detalle/{entradaId}
        [HttpGet("secretaria/historia/{pacienteId:int}/detalle/{entradaId:int}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<HistoriaClinicaDetalleDTO>> GetHistoriaDetalleSecretaria(
            int pacienteId,
            int entradaId)
        {
            // reutilizamos el método que ya existe para paciente,
            // pero SIN depender del usuario logueado
            var dto = await _repo.ObtenerHistoriaDetallePacienteAsync(pacienteId, entradaId);

            if (dto == null)
                return NotFound("Entrada de historia clínica no encontrada.");

            return Ok(dto);
        }

    }
}
