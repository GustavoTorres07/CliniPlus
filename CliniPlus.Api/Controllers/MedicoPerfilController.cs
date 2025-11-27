using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Medico")]
    public class MedicoPerfilController : ControllerBase
    {
        private readonly IMedicoPerfilRepository _repo;

        public MedicoPerfilController(IMedicoPerfilRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("datos")]
        public async Task<ActionResult<PerfilMedicoDTO>> Get()
        {
            // Buscar el claim de forma más robusta
            var subClaim = User.FindFirst("sub")?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(subClaim))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }

            int idUsuario = int.Parse(subClaim);
            var dto = await _repo.ObtenerAsync(idUsuario);

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpPut("datos")]
        public async Task<ActionResult> Put(PerfilMedicoDTO dto)
        {
            var subClaim = User.FindFirst("sub")?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(subClaim))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }

            int idUsuario = int.Parse(subClaim);
            var ok = await _repo.ActualizarAsync(idUsuario, dto);

            if (!ok)
                return BadRequest();

            return NoContent();
        }
    }
}
