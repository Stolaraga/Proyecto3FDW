using Microsoft.AspNetCore.Mvc;
using Veterinaria.Api.Infrastructure;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Entities;
using Veterinaria.Domain.Mappings;
using Veterinaria.Domain.Services;


namespace Veterinaria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class MascotasController : ControllerBase
    {
        private readonly IMascotaService _svc;
        public MascotasController(IMascotaService svc) => _svc = svc;

        // GET: /api/mascotas
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MascotaReadDto>>> Get([FromQuery] Guid? clienteId)
        {
            var list = await _svc.ListarAsync(clienteId);
            return Ok(list);
        }

        // GET: /api/mascotas/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MascotaReadDto?>> GetById(Guid id)
        {
            var dto = await _svc.ObtenerAsync(new MascotaReadDto { Id = id });
            return dto is null ? NotFound() : Ok(dto);
        }

        // POST: /api/mascotas
        [HttpPost]
        public async Task<ActionResult<MascotaReadDto>> Post([FromBody] MascotaCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var creado = await _svc.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        // PUT: /api/mascotas/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] MascotaUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            dto = dto with { Id = id };
            var ok = await _svc.ActualizarAsync(dto);
            return ok ? NoContent() : NotFound();
        }

        // DELETE: /api/mascotas/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _svc.EliminarAsync(new MascotaReadDto { Id = id });
            return ok ? NoContent() : NotFound();
        }
    }
}





