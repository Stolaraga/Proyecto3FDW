using Microsoft.AspNetCore.Mvc;
using Veterinaria.Api.Infrastructure.Repositories;
using Veterinaria.Domain.Entities;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Mappings;


namespace Veterinaria.Api.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClientesRepository _repo;
        public ClientesController(ClientesRepository repo) => _repo = repo;

        [HttpGet]
        public ActionResult<IEnumerable<ClienteReadDto>> GetAll([FromQuery] string? q)
        {
            var list = _repo.GetAll();
            if (!string.IsNullOrWhiteSpace(q))
                list = (List<Cliente>)list.Where(c =>
                    ($"{c.Nombre} {c.Apellidos}")
                    .Contains(q, StringComparison.OrdinalIgnoreCase)
                    || c.Cedula.Contains(q));
            return Ok(list.Select(c => c.ToReadDto()));
        }

        [HttpGet("{id}")]
        public ActionResult<ClienteReadDto> Get(Guid id)
        {
            var c = _repo.Get(id);
            return c is null ? NotFound() : Ok(c.ToReadDto());
        }

        [HttpPost]
        public ActionResult<ClienteReadDto> Post([FromBody] ClienteCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var entity = new Cliente();
            entity.Apply(dto);
            _repo.Add(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToReadDto());
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] ClienteUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var existing = _repo.Get(id);
            if (existing is null) return NotFound();
            existing.Apply(dto with { Id = id });
            return _repo.Update(existing) ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
            => _repo.Delete(id) ? NoContent() : NotFound();
    }


}
