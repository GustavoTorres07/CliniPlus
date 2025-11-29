using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Api.Services.Contrato;
using CliniPlus.Api.Utils;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IEmailService _email;   // 👈 nuevo

        public AuthController(AppDbContext db, ITokenRepository tokens, IEmailService email)
        {
            _db = db;
            _tokens = tokens;
            _email = email;
        }

        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping() => Ok(new { ok = true, message = "Auth OK" });

        // ---------------- LOGIN ----------------
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthLoginResponseDTO>> Login([FromBody] AuthLoginRequest req)
        {
            if (!ModelState.IsValid) return BadRequest("Datos inválidos.");

            var u = await _db.Usuario.FirstOrDefaultAsync(x => x.Email == req.Email && x.IsActive);
            if (u == null) return Unauthorized("Usuario o contraseña inválidos.");

            if (!BCrypt.Net.BCrypt.Verify(req.Password, u.PasswordHash))
                return Unauthorized("Usuario o contraseña inválidos.");

            var token = _tokens.EmitirToken(u.IdUsuario, u.Nombre, u.Apellido, u.Email, u.Rol);

            return new AuthLoginResponseDTO
            {
                UsuarioId = u.IdUsuario,
                NombreCompleto = $"{u.Nombre} {u.Apellido}",
                Rol = u.Rol,
                Token = token,

                // 👇 sólo Paciente usa recuperación de contraseña
                DebeCambiarPassword = (u.Rol == "Paciente" && u.RecuperarContrasena)
            };
        }

        // ---------------- ME ----------------
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

        // ---------------- RECUPERAR PASSWORD (solo Paciente) ----------------
        [HttpPost("recuperar-password")]
        [AllowAnonymous]
        public async Task<IActionResult> RecuperarPassword([FromBody] RecuperarPasswordRequest body)
        {
            if (string.IsNullOrWhiteSpace(body.Email))
                return BadRequest("El email es obligatorio.");

            var email = body.Email.Trim();

            var usuario = await _db.Usuario
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    u.IsActive &&
                    u.Rol == "Paciente");

            if (usuario is null)
                return NotFound("No existe un paciente con ese email.");

            var tempPassword = GeneradorPassword.RandomPassword(8);

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            usuario.RecuperarContrasena = true;

            await _db.SaveChangesAsync();

            await _email.EnviarPasswordTemporalAsync(
                usuario.Email,
                usuario.Nombre,
                tempPassword
            );

            return Ok(new { mensaje = "Se envió una contraseña temporal a tu correo." });
        }

    }
}
