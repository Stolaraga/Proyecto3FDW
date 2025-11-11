using Microsoft.AspNetCore.Mvc;
using Veterinaria.Api.Services;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Services;


namespace Veterinaria.Api.Controllers
{



    [ApiController]
    [Route("api/[controller]")]
    public sealed class CitasController : ControllerBase
    {
        private readonly ICitaService _svc;
        public CitasController(ICitaService svc) => _svc = svc;

        // GET /api/citas?mascotaId={guid}&estado=Pendiente&desde=2025-01-01&hasta=2025-12-31
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CitaReadDto>>> Get(
            [FromQuery] Guid? mascotaId, [FromQuery] string? estado,
            [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            var list = await _svc.ListarAsync(mascotaId, estado, desde, hasta);
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CitaReadDto?>> GetById(Guid id)
        {
            var dto = await _svc.ObtenerAsync(new CitaReadDto { Id = id });
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<CitaReadDto>> Post([FromBody] CitaCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var creado = await _svc.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] CitaUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            dto = dto with { Id = id };
            var ok = await _svc.ActualizarAsync(dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _svc.EliminarAsync(new CitaReadDto { Id = id });
            return ok ? NoContent() : NotFound();
        }



    }
}
