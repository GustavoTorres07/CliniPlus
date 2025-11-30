using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador,Secretaria, Medico")]
    public class EspecialidadesController : ControllerBase
    {
        private readonly IEspecialidadRepository _repo;

        public EspecialidadesController(IEspecialidadRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("listar")]
        [AllowAnonymous]   
        public async Task<ActionResult<List<EspecialidadDTO>>> Listar()
        {
            var lista = await _repo.ListarAsync();
            return Ok(lista);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Administrador,Secretaria")]
        public async Task<ActionResult<EspecialidadDTO>> ObtenerPorId(int id)
        {
            var esp = await _repo.ObtenerPorIdAsync(id);
            if (esp is null)
                return NotFound("Especialidad no encontrada.");

            return Ok(esp);
        }

        [HttpPost("crear")]
        [Authorize(Roles = "Administrador,Secretaria")]
        public async Task<ActionResult<EspecialidadDTO>> Crear([FromBody] EspecialidadDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre es obligatorio.");

            try
            {
                var creada = await _repo.CrearAsync(dto);
                return Ok(creada);
            }
            catch (InvalidOperationException ex) when (ex.Message == "ESPECIALIDAD_EXISTE")
            {
                return Conflict("Ya existe una especialidad con ese nombre.");
            }
        }

        [HttpPut("editar/{id:int}")]
        [Authorize(Roles = "Administrador,Secretaria")]
        public async Task<ActionResult<EspecialidadDTO>> Editar(int id, [FromBody] EspecialidadDTO dto)
        {
            try
            {
                var editada = await _repo.EditarAsync(id, dto);

                if (editada is null)
                    return NotFound("Especialidad no encontrada.");

                return Ok(editada);
            }
            catch (InvalidOperationException ex) when (ex.Message == "ESPECIALIDAD_EXISTE")
            {
                return Conflict("Ya existe una especialidad con ese nombre.");
            }
        }

        [HttpPatch("estado/{id:int}")]
        [Authorize(Roles = "Administrador,Secretaria")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] EspecialidadEstadoDTO body)
        {
            var ok = await _repo.CambiarEstadoAsync(id, body.IsActive);

            if (!ok)
                return NotFound("Especialidad no encontrada.");

            return Ok(new { mensaje = "Estado actualizado exitosamente." });
        }
    }
}

