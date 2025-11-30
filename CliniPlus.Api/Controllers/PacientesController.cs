using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/pacientes")]
    [Authorize(Policy = "SecretariaOAdministrador")]
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

        // LISTADO PARA SECRETARÍA
        // GET: api/pacientes/secretaria/listar
        [HttpGet("secretaria/listar")]
        public async Task<ActionResult<List<PacienteListadoDTO>>> ListarSecretaria()
        {
            var lista = await _repo.ListarParaSecretariaAsync();
            return Ok(lista);
        }

        /// <summary>
        /// Secretaría: completa datos y vincula un paciente provisional con un Usuario existente.
        /// </summary>
        [HttpPost("activar-cuenta")]
        [Authorize(Policy = "SecretariaOAdministrador")]
        public async Task<IActionResult> ActivarCuenta([FromBody] PacienteActivarCuentaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            try
            {
                var ok = await _repo.ActivarCuentaProvisionalAsync(dto);

                if (!ok)
                    return BadRequest("No se pudo activar la cuenta del paciente.");

                return Ok(new { mensaje = "Cuenta de paciente activada correctamente." });
            }
            catch (InvalidOperationException ex) when (ex.Message == "PACIENTE_NO_ENCONTRADO")
            {
                return NotFound("Paciente no encontrado.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "PACIENTE_NO_ES_PROVISIONAL")
            {
                return BadRequest("El paciente no es provisional.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "DNI_NO_COINCIDE")
            {
                return BadRequest("El DNI ingresado no coincide con el registrado.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "USUARIO_NO_ENCONTRADO")
            {
                return BadRequest("El usuario indicado no existe o no está activo.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "USUARIO_NO_ES_PACIENTE")
            {
                return BadRequest("El usuario no tiene rol de Paciente.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "USUARIO_YA_VINCULADO")
            {
                return Conflict("Ese usuario ya está vinculado a otro paciente.");
            }
        }
    }
}
