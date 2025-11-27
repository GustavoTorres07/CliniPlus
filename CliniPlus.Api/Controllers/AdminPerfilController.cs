using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class AdminPerfilController : ControllerBase
    {
        private readonly IAdminPerfilRepository _repo;

        public AdminPerfilController(IAdminPerfilRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("datos")]
        public async Task<ActionResult<PerfilAdminDTO>> Get()
        {
            var sub = User.FindFirst("sub")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (sub == null)
                return Unauthorized();

            int idUsuario = int.Parse(sub);

            var dto = await _repo.ObtenerAsync(idUsuario);

            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPut("datos")]
        public async Task<ActionResult> Put(PerfilAdminDTO dto)
        {
            var sub = User.FindFirst("sub")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (sub == null)
                return Unauthorized();

            int idUsuario = int.Parse(sub);

            var ok = await _repo.ActualizarAsync(idUsuario, dto);

            return ok ? NoContent() : BadRequest();
        }

        [HttpPut("password")]
        public async Task<ActionResult> CambiarPassword(CambiarPasswordDTO dto)
        {
            var sub = User.FindFirst("sub")?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (sub is null)
                return Unauthorized();

            int idUsuario = int.Parse(sub);

            var ok = await _repo.CambiarPasswordAsync(idUsuario, dto);

            return ok ? NoContent() : BadRequest("Contraseña actual incorrecta.");
        }

    }
}
