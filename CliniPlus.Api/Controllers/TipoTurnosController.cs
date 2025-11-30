// CliniPlus.Api/Controllers/TipoTurnosController.cs
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CliniPlus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoTurnosController : ControllerBase
    {
        private readonly ITipoTurnoRepository _repo;

        public TipoTurnosController(ITipoTurnoRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<TipoTurnoDTO>>> Get()
        {
            var lista = await _repo.ListarActivosAsync();
            return Ok(lista);
        }
    }
}
