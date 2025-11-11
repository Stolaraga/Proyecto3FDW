using Microsoft.AspNetCore.Mvc;
using Veterinaria.Api.Infrastructure.Repositories;
using Veterinaria.Api.Services;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Entities;
using Veterinaria.Domain.Mappings;

namespace Veterinaria.Api.Controllers
{



    [ApiController]
    [Route("api/[controller]")]
    public sealed class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _svc;
        public EmpleadosController(IEmpleadoService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<EmpleadoReadDto>>> Get()
            => Ok(await _svc.ListarAsync());

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmpleadoReadDto?>> GetById(Guid id)
        {
            var dto = await _svc.ObtenerAsync(new EmpleadoReadDto { Id = id });
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<EmpleadoReadDto>> Post([FromBody] EmpleadoCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var creado = await _svc.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] EmpleadoUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            dto = dto with { Id = id };
            var ok = await _svc.ActualizarAsync(dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _svc.EliminarAsync(new EmpleadoReadDto { Id = id });
            return ok ? NoContent() : NotFound();
        }
    }





}
