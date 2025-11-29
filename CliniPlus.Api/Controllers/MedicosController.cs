using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using CliniPlus.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicosController : ControllerBase
    {
        private readonly IMedicoRepository _repo;
        private readonly AppDbContext _db;


        public MedicosController(IMedicoRepository repo, AppDbContext db)
        {
            _db = db;
            _repo = repo;
        }

        // ================= MÉDICOS =================

        [HttpGet]
        [Authorize(Policy = "SecretariaOAdministrador")]
        public async Task<ActionResult<List<MedicoListadoDTO>>> Listar()
        {
            var lista = await _repo.ListarAsync();
            return Ok(lista);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Administrador,Secretaria,Paciente")]
        public async Task<ActionResult<MedicoDetalleDTO>> Obtener(int id)
        {
            var medico = await _repo.ObtenerAsync(id);
            if (medico is null) return NotFound();
            return Ok(medico);
        }

        [HttpPost]
        [Authorize(Policy = "AdministradorOnly")]
        public async Task<ActionResult<MedicoDetalleDTO>> Crear([FromBody] MedicoCrearDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            try
            {
                var id = await _repo.CrearAsync(dto);
                if (id is null)
                    return BadRequest("No se pudo crear el médico.");

                var medico = await _repo.ObtenerAsync(id.Value);
                return Ok(medico);
            }
            catch (InvalidOperationException ex)
            {
                // USUARIO_NO_ENCONTRADO, USUARIO_NO_MEDICO, USUARIO_YA_ES_MEDICO, etc.
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdministradorOnly")]
        public async Task<ActionResult<MedicoDetalleDTO>> Editar(int id, [FromBody] MedicoEditarDTO dto)
        {
            var medico = await _repo.EditarAsync(id, dto);
            if (medico is null) return NotFound();
            return Ok(medico);
        }

        [HttpPatch("{id:int}/estado")]
        [Authorize(Policy = "AdministradorOnly")]
        public async Task<ActionResult> CambiarEstado(int id, [FromBody] MedicoEstadoDTO body)
        {
            var ok = await _repo.CambiarEstadoAsync(id, body.IsActive);
            if (!ok) return NotFound();
            return Ok();
        }

        [HttpGet("especialidad/{especialidadId:int}")]
        [Authorize] // cualquier autenticado
        public async Task<ActionResult<List<MedicoListadoDTO>>> ListarPorEspecialidad(int especialidadId)
        {
            var lista = await _repo.ListarPorEspecialidadAsync(especialidadId);
            return Ok(lista);
        }

        // ================= HORARIOS =================

        [HttpGet("{medicoId:int}/horarios")]
        [Authorize(Policy = "SecretariaOAdministrador")] // o también permitir Médico si querés
        public async Task<ActionResult<List<MedicoHorarioDTO>>> ListarHorarios(int medicoId)
        {
            var lista = await _repo.ListarHorariosAsync(medicoId);
            return Ok(lista);
        }

        [HttpPost("{medicoId:int}/horarios")]
        [Authorize(Policy = "SecretariaOAdministrador")]
        public async Task<ActionResult<MedicoHorarioDTO>> CrearHorario(int medicoId, [FromBody] MedicoHorarioDTO dto)
        {
            try
            {
                var res = await _repo.CrearHorarioAsync(medicoId, dto);
                return Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{medicoId:int}/horarios/{idHorario:int}")]
        [Authorize(Policy = "SecretariaOAdministrador")]
        public async Task<ActionResult<MedicoHorarioDTO>> EditarHorario(int medicoId, int idHorario, [FromBody] MedicoHorarioDTO dto)
        {
            try
            {
                var res = await _repo.EditarHorarioAsync(medicoId, idHorario, dto);
                if (res is null) return NotFound();
                return Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{medicoId:int}/horarios/{idHorario:int}")]
        [Authorize(Policy = "SecretariaOAdministrador")]
        public async Task<ActionResult> EliminarHorario(int medicoId, int idHorario)
        {
            var ok = await _repo.EliminarHorarioAsync(medicoId, idHorario);
            if (!ok) return NotFound();
            return Ok();
        }

        // ================= BLOQUEOS =================

        [HttpGet("{medicoId:int}/bloqueos")]
        [Authorize(Policy = "SecretariaOAdministrador")]
        public async Task<ActionResult<List<MedicoBloqueoDTO>>> ListarBloqueos(int medicoId)
        {
            var lista = await _repo.ListarBloqueosAsync(medicoId);
            return Ok(lista);
        }

        [HttpPost("{medicoId:int}/bloqueos")]
        [Authorize(Policy = "SecretariaOAdministrador")]
        public async Task<ActionResult<MedicoBloqueoDTO>> CrearBloqueo(int medicoId, [FromBody] MedicoBloqueoDTO dto)
        {
            try
            {
                var res = await _repo.CrearBloqueoAsync(medicoId, dto);
                return Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                // MEDICO_NO_ENCONTRADO, BLOQUEO_RANGO_INVALIDO
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{medicoId:int}/bloqueos/{idBloqueo:int}")]
        [Authorize(Policy = "SecretariaOAdministrador")]
        public async Task<ActionResult> EliminarBloqueo(int medicoId, int idBloqueo)
        {
            var ok = await _repo.EliminarBloqueoAsync(medicoId, idBloqueo);
            if (!ok) return NotFound();
            return Ok();
        }

        [HttpGet("publico")]
        [Authorize(Roles = "Paciente,Administrador,Secretaria")]
        public async Task<ActionResult<List<MedicoListadoDTO>>> ListarPublico(
            [FromQuery] int? especialidadId = null)
        {
            List<MedicoListadoDTO> lista;

            if (especialidadId.HasValue)
            {
                lista = await _repo.ListarPorEspecialidadAsync(especialidadId.Value);
            }
            else
            {
                lista = await _repo.ListarAsync();
            }

            var activos = lista
                .Where(m => m.IsActive)
                .ToList();

            return Ok(activos);
        }

        // GET: api/turnos/paciente/medicos?especialidadId=1&q=ana
        [HttpGet("paciente/medicos")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult<List<MedicoDisponiblePacienteDTO>>> GetMedicosDisponibles(
            [FromQuery] int? especialidadId,
            [FromQuery] string? q)
        {
            // Sólo usuarios Paciente, ya está validado por el atributo Authorize

            var query = _db.Medico
                .Include(m => m.Usuario)
                .Include(m => m.Especialidad)
                .Where(m => m.IsActive)
                .AsQueryable();

            if (especialidadId.HasValue)
                query = query.Where(m => m.EspecialidadId == especialidadId.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var texto = q.Trim().ToLower();
                query = query.Where(m =>
                    (m.Usuario.Nombre + " " + m.Usuario.Apellido).ToLower().Contains(texto));
            }

            // Opcional: podrías chequear horarios o turnos para setear TieneAgenda / ProximoTurno
            var lista = await query
                .OrderBy(m => m.Usuario.Apellido)
                .ThenBy(m => m.Usuario.Nombre)
                .Select(m => new MedicoDisponiblePacienteDTO
                {
                    IdMedico = m.IdMedico,
                    NombreCompleto = m.Usuario.Nombre + " " + m.Usuario.Apellido,
                    EspecialidadNombre = m.Especialidad != null ? m.Especialidad.Nombre : null,
                    Bio = m.Bio,
                    FotoUrl = m.FotoUrl,
                    TieneAgenda = true,           // por ahora asumimos que sí
                    ProximoTurnoUtc = null        // si lo querés calcular lo vemos después
                })
                .ToListAsync();

            return Ok(lista);
        }


    }
}
