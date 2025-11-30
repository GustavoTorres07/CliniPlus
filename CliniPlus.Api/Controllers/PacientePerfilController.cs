using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Paciente")]
    public class PacientePerfilController : ControllerBase
    {
        private readonly IPacientePerfilRepository _repo;

        public PacientePerfilController(IPacientePerfilRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("datos")]
        public async Task<ActionResult<PerfilPacienteDTO>> Get()
        {
            var sub = User.FindFirst("sub")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (sub is null)
                return Unauthorized("Token inválido");

            var dto = await _repo.ObtenerAsync(int.Parse(sub));

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpPut("datos")]
        public async Task<ActionResult> Put(PerfilPacienteDTO dto)
        {
            var sub = User.FindFirst("sub")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (sub is null)
                return Unauthorized("Token invalido");

            var ok = await _repo.ActualizarAsync(int.Parse(sub), dto);

            return ok ? NoContent() : BadRequest("No se pudo actualizar");
        }

        [HttpPost("cambiar-password")]
        public async Task<ActionResult> CambiarPassword(CambiarPasswordDTO dto)
        {
            var sub = User.FindFirst("sub")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (sub is null)
                return Unauthorized("Token invalido");

            var ok = await _repo.CambiarPasswordAsync(int.Parse(sub), dto.PasswordActual, dto.PasswordNueva);

            return ok ? NoContent() : BadRequest("Contraseña actual incorrecta");
        }
    }
}
