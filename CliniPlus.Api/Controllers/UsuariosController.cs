using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize(Policy = "AdministradorOnly")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _repo;

        public UsuariosController(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<List<UsuarioListDTO>>> Listar()
        {
            return Ok(await _repo.ListarAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDetalleDTO>> Obtener(int id)
        {
            var u = await _repo.ObtenerPorIdAsync(id);
            if (u == null) return NotFound("Usuario no encontrado");
            return Ok(u);
        }

        [HttpPost("crear")]
        public async Task<ActionResult<UsuarioDetalleDTO>> Crear([FromBody] UsuarioCrearDTO dto)
        {
            try
            {
                var creado = await _repo.CrearAsync(dto);
                return Ok(creado);
            }
            catch (InvalidOperationException ex) when (ex.Message == "EMAIL_EXISTE")
            {
                return Conflict("Ya existe un usuario con ese email.");
            }
        }

        [HttpPut("editar/{id:int}")]
        public async Task<ActionResult<UsuarioDetalleDTO>> Editar(int id, [FromBody] UsuarioEditarDTO dto)
        {
            try
            {
                var editado = await _repo.EditarAsync(id, dto);
                if (editado == null) return NotFound("Usuario no encontrado");
                return Ok(editado);
            }
            catch (InvalidOperationException ex) when (ex.Message == "EMAIL_EXISTE")
            {
                return Conflict("Ya existe un usuario con ese email.");
            }
        }

        [HttpPatch("estado/{id:int}")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] UsuarioEstadoDTO dto)
        {
            var ok = await _repo.CambiarEstadoAsync(id, dto.IsActive);
            return ok ? Ok(new { mensaje = "Estado actualizado" })
                      : NotFound("Usuario no encontrado");
        }

        [HttpPatch("password/{id:int}")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] UsuarioPasswordAdminDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Contraseña invalida.");

            var ok = await _repo.CambiarPasswordAdminAsync(id, dto.NuevaPassword);

            return ok
                ? Ok(new { mensaje = "Contraseña actualizada correctamente." })
                : NotFound("Usuario no encontrado.");
        }

    }
}
