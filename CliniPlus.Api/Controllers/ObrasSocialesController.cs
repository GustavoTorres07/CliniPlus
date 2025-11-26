using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/obras-sociales")]
    [Authorize] // todas requieren JWT
    public class ObrasSocialesController : ControllerBase
    {
        private readonly IObraSocialRepository _repo;

        public ObrasSocialesController(IObraSocialRepository repo)
        {
            _repo = repo;
        }

        // GET api/obras-sociales/listar
        [HttpGet("listar")]
        public async Task<ActionResult<List<ObraSocialDTO>>> Listar()
        {
            var lista = await _repo.ListarAsync();
            return Ok(lista);
        }

        // GET api/obras-sociales/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ObraSocialDTO>> Obtener(int id)
        {
            var os = await _repo.ObtenerPorIdAsync(id);
            if (os == null) return NotFound("Obra social no encontrada.");
            return Ok(os);
        }

        // POST api/obras-sociales/crear
        [HttpPost("crear")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ObraSocialDTO>> Crear([FromBody] ObraSocialDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre es obligatorio.");

            try
            {
                var creada = await _repo.CrearAsync(dto);
                return Ok(creada);
            }
            catch (InvalidOperationException ex) when (ex.Message == "OBRASOCIAL_EXISTE")
            {
                return Conflict("Ya existe una obra social con ese nombre.");
            }
        }

        // PUT api/obras-sociales/editar/5
        [HttpPut("editar/{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ObraSocialDTO>> Editar(int id, [FromBody] ObraSocialDTO dto)
        {
            try
            {
                var editada = await _repo.EditarAsync(id, dto);
                if (editada == null)
                    return NotFound("Obra social no encontrada.");

                return Ok(editada);
            }
            catch (InvalidOperationException ex) when (ex.Message == "OBRASOCIAL_EXISTE")
            {
                return Conflict("Ya existe una obra social con ese nombre.");
            }
        }

        // PATCH api/obras-sociales/estado/5
        [HttpPatch("estado/{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] ObraSocialEstadoDTO body)
        {
            var ok = await _repo.CambiarEstadoAsync(id, body.IsActive);
            if (!ok) return NotFound("Obra social no encontrada.");

            return Ok(new { mensaje = "Estado actualizado." });
        }
    }
}
