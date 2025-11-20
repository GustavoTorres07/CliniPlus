using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicosController : ControllerBase
    {
        private readonly IMedicoRepository _repo;

        public MedicosController(IMedicoRepository repo)
        {
            _repo = repo;
        }

        /// <summary>Lista todos los médicos activos</summary>
        [HttpGet("listar")]
        [Authorize(Policy = "SecretariaOAdmin")]
        public async Task<ActionResult<List<MedicoDetalleDTO>>> Listar()
        {
            return Ok(await _repo.ListarAsync());
        }

        /// <summary>Crea un médico (Usuario + Médico + Especialidades)</summary>
        [HttpPost("crear")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<MedicoDetalleDTO>> Crear([FromBody] MedicoCrearDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos.");

            var result = await _repo.CrearAsync(dto);

            return Ok(result);
        }
    }
}
