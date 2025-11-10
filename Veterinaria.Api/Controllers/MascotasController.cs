using Microsoft.AspNetCore.Mvc;
using Veterinaria.Api.Infrastructure;
using Veterinaria.Api.Infrastructure.Repositories;
using Veterinaria.Domain.Entities;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Mappings;

namespace Veterinaria.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MascotasController : ControllerBase
    {
        private readonly MascotasRepository _repo;
        private readonly InMemoryStore _db;

        public MascotasController(MascotasRepository repo, InMemoryStore db)
        { _repo = repo; _db = db; }

        [HttpGet]
        public ActionResult<IEnumerable<MascotaReadDto>> GetAll([FromQuery] Guid? clienteId)
        {
            var list = _repo.GetAll();
            if (clienteId is not null) list = (List<Mascota>)list.Where(m => m.ClienteId == clienteId);
            return Ok(list.Select(m => m.ToReadDto()));
        }

        [HttpGet("{id}")]
        public ActionResult<MascotaReadDto> Get(Guid id)
        {
            var m = _repo.Get(id);
            return m is null ? NotFound() : Ok(m.ToReadDto());
        }

        [HttpPost]
        public ActionResult<MascotaReadDto> Post([FromBody] MascotaCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (!_db.Clientes.ContainsKey(dto.ClienteId))
                return Problem(detail: "Cliente inexistente", statusCode: 400);

            var entity = new Mascota();
            entity.Apply(dto);
            _repo.Add(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToReadDto());
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] MascotaUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (!_db.Clientes.ContainsKey(dto.ClienteId))
                return Problem(detail: "Cliente inexistente", statusCode: 400);

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
