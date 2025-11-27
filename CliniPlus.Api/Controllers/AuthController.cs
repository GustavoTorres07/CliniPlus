using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CliniPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITokenRepository _tokens;

        public AuthController(AppDbContext db, ITokenRepository tokens)
        {
            _db = db;
            _tokens = tokens;
        }

        /// <summary>
        /// Ping público (útil para verificar despliegue en Somee).
        /// </summary>
        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping() => Ok(new { ok = true, message = "Auth OK" });

        /// <summary>
        /// Login con Email/Password -> JWT
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthLoginResponseDTO>> Login([FromBody] AuthLoginRequest req)
        {
            if (!ModelState.IsValid) return BadRequest("Datos inválidos.");

            var u = await _db.Usuario.FirstOrDefaultAsync(x => x.Email == req.Email && x.IsActive);
            if (u == null) return Unauthorized("Usuario o contraseña inválidos.");

            // Verificar hash con BCrypt.Net-Next
            if (!BCrypt.Net.BCrypt.Verify(req.Password, u.PasswordHash))
                return Unauthorized("Usuario o contraseña inválidos.");

            var token = _tokens.EmitirToken(u.IdUsuario, u.Nombre, u.Apellido, u.Email, u.Rol);

            return new AuthLoginResponseDTO
            {
                UsuarioId = u.IdUsuario,
                NombreCompleto = $"{u.Nombre} {u.Apellido}",
                Rol = u.Rol,
                Token = token
            };
        }

        /// <summary>
        /// Info del usuario autenticado usando las claims del token.
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<AuthMeResponse> Me()
        {
            string? id =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            string? nombre =
                User.FindFirstValue(ClaimTypes.Name) ??
                User.FindFirstValue("name");

            string? email =
                User.FindFirstValue(ClaimTypes.Email) ??
                User.FindFirstValue("email");

            string? rol = User.FindFirstValue("role");


            if (id is null)
                return Unauthorized("No se pudo obtener el usuario desde el token.");

            var dto = new AuthMeResponse
            {
                UsuarioId = int.Parse(id),
                NombreCompleto = nombre ?? "",
                Email = email ?? "",
                Rol = rol ?? ""
            };

            return Ok(dto);
        }

    }
}
