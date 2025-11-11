using Microsoft.AspNetCore.Mvc;
using Veterinaria.Api.Infrastructure.Repositories;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Entities;
using Veterinaria.Domain.Mappings;
using Veterinaria.Domain.Services;


namespace Veterinaria.Api.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public sealed class ClientesController : ControllerBase
    {
        private readonly IClienteService _svc;
        public ClientesController(IClienteService svc) => _svc = svc;

        // GET: /api/clientes
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ClienteReadDto>>> Get()
        {
            var list = await _svc.ListarAsync();
            return Ok(list);
        }

        // GET: /api/clientes/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClienteReadDto?>> GetById(Guid id)
        {
            var dto = await _svc.ObtenerAsync(new ClienteReadDto { Id = id });
            return dto is null ? NotFound() : Ok(dto);
        }

        // POST: /api/clientes
        [HttpPost]
        public async Task<ActionResult<ClienteReadDto>> Post([FromBody] ClienteCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var creado = await _svc.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        // PUT: /api/clientes/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ClienteUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            dto = dto with { Id = id };
            var ok = await _svc.ActualizarAsync(dto);
            return ok ? NoContent() : NotFound();
        }

        // DELETE: /api/clientes/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _svc.EliminarAsync(new ClienteReadDto { Id = id });
            return ok ? NoContent() : NotFound();
        }
    }


}
