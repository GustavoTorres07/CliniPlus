using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/pacientes")]
    [Authorize(Policy = "SecretariaOAdmin")]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteRepository _repo;

        public PacientesController(IPacienteRepository repo)
        {
            _repo = repo;
        }

        /// <summary>Lista de pacientes (por defecto solo activos).</summary>
        /// Nombre en Postman: Pacientes - Listar
        [HttpGet("listar")]
        public async Task<ActionResult<List<PacienteListDTO>>> Listar([FromQuery] bool incluirInactivos = false)
        {
            var lista = await _repo.ListarAsync(incluirInactivos);
            return Ok(lista);
        }

        /// <summary>Detalle de un paciente por Id.</summary>
        /// Nombre en Postman: Pacientes - Obtener por Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PacienteDetalleDTO>> Obtener(int id)
        {
            var p = await _repo.ObtenerPorIdAsync(id);
            if (p == null) return NotFound("Paciente no encontrado");
            return Ok(p);
        }

        /// <summary>Crea un paciente (provisional o completo).</summary>
        /// Nombre en Postman: Pacientes - Crear
        [HttpPost("crear")]
        public async Task<ActionResult<PacienteDetalleDTO>> Crear([FromBody] PacienteCrearDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            try
            {
                var creado = await _repo.CrearAsync(dto);
                return Ok(creado);
            }
            catch (InvalidOperationException ex) when (ex.Message == "DNI_EXISTE")
            {
                return Conflict("Ya existe un paciente registrado con ese DNI.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "USUARIO_INVALIDO")
            {
                return BadRequest("El usuario no existe o no es un usuario paciente activo.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "USUARIO_YA_VINCULADO")
            {
                return Conflict("Ese usuario ya está vinculado a otro paciente.");
            }
        }

        /// <summary>Edita datos de un paciente existente.</summary>
        /// Nombre en Postman: Pacientes - Editar
        [HttpPut("editar/{id:int}")]
        public async Task<ActionResult<PacienteDetalleDTO>> Editar(int id, [FromBody] PacienteEditarDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            try
            {
                var editado = await _repo.EditarAsync(id, dto);
                if (editado == null) return NotFound("Paciente no encontrado");
                return Ok(editado);
            }
            catch (InvalidOperationException ex) when (ex.Message == "DNI_EXISTE")
            {
                return Conflict("Ya existe un paciente registrado con ese DNI.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "USUARIO_INVALIDO")
            {
                return BadRequest("El usuario no existe o no es un usuario paciente activo.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "USUARIO_YA_VINCULADO")
            {
                return Conflict("Ese usuario ya está vinculado a otro paciente.");
            }
        }

        /// <summary>Activa / desactiva un paciente.</summary>
        /// Nombre en Postman: Pacientes - Cambiar estado
        [HttpPatch("estado/{id:int}")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] PacienteEstadoDTO dto)
        {
            var ok = await _repo.CambiarEstadoAsync(id, dto.IsActive);
            return ok ? Ok(new { mensaje = "Estado actualizado" })
                      : NotFound("Paciente no encontrado");
        }
    }
}
