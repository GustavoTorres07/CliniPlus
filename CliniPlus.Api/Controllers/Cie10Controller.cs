using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class Cie10Controller : ControllerBase
    {
        private readonly ICie10Repository _repo;

        public Cie10Controller(ICie10Repository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cie10DTO>>> Get([FromQuery] string? q)
        {
            var res = await _repo.ListarAsync(q);
            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult<Cie10DTO>> Post([FromBody] Cie10CrearDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var creado = await _repo.CrearAsync(dto);
                return CreatedAtAction(nameof(Get), new { q = creado!.Codigo }, creado);
            }
            catch (InvalidOperationException ex) when (ex.Message == "CIE10_YA_EXISTE")
            {
                return Conflict("Ya existe un diagnóstico con ese código.");
            }
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult<Cie10DTO>> Put(string codigo, [FromBody] Cie10EditarDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var editado = await _repo.EditarAsync(codigo, dto);
            if (editado == null) return NotFound();

            return Ok(editado);
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Delete(string codigo)
        {
            var ok = await _repo.EliminarAsync(codigo);
            if (!ok) return BadRequest("No se pudo eliminar el diagnóstico (puede estar en uso).");

            return NoContent();
        }
    }
}
