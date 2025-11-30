using System.Security.Claims;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/fichas-medicas")]
    [Authorize] 
    public class FichasMedicasController : ControllerBase
    {
        private readonly IFichaMedicaRepository _repo;
        private readonly IPacienteRepository _pacientes;

        public FichasMedicasController(
            IFichaMedicaRepository repo,
            IPacienteRepository pacientes)
        {
            _repo = repo;
            _pacientes = pacientes;
        }

        [HttpGet("{pacienteId:int}")]
        [Authorize(Roles = "Medico,Secretaria,Administrador")]
        public async Task<ActionResult<FichaMedicaDTO>> Obtener(int pacienteId)
        {
            var ficha = await _repo.ObtenerPorPacienteAsync(pacienteId);

            if (ficha == null)
                return NotFound("El paciente no tiene ficha medica cargada.");

            return Ok(ficha);
        }

        [HttpPut("{pacienteId:int}")]
        [Authorize(Roles = "Medico,Secretaria,Administrador")]
        public async Task<ActionResult<FichaMedicaDTO>> Guardar(
            int pacienteId,
            [FromBody] FichaMedicaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos invalidos.");

            try
            {
                var guardada = await _repo.GuardarAsync(pacienteId, dto);
                return Ok(guardada);
            }
            catch (InvalidOperationException ex) when (ex.Message == "PACIENTE_NO_ENCONTRADO")
            {
                return NotFound("Paciente no encontrado o inactivo.");
            }
        }

        [HttpGet("mi-ficha")]
        [Authorize(Roles = "Paciente")]
        public async Task<ActionResult<FichaMedicaDTO>> ObtenerMiFicha()
        {
            var usuarioIdClaim = User.FindFirst("IdUsuario")
                              ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out var usuarioId))
                return Unauthorized("No se pudo determinar el usuario.");

            var pacienteId = await _pacientes.ObtenerIdPorUsuarioAsync(usuarioId);
            if (pacienteId == null)
                return NotFound("No se encontro un paciente asociado a este usuario.");

            var ficha = await _repo.ObtenerPorPacienteAsync(pacienteId.Value);
            if (ficha == null)
                return NotFound("El paciente no tiene ficha medica cargada.");

            return Ok(ficha);
        }
    }
}
