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
    public class AtencionesController : ControllerBase
    {
        private readonly AtencionesRepository _repo;
        private readonly InMemoryStore _db;

        public AtencionesController(AtencionesRepository repo, InMemoryStore db)
        { _repo = repo; _db = db; }

        [HttpGet]
        public ActionResult<IEnumerable<AtencionReadDto>> GetAll([FromQuery] Guid? mascotaId)
        {
            var list = _repo.GetAll();
            if (mascotaId is not null) list = list.Where(a => a.MascotaId == mascotaId);
            return Ok(list.Select(a => a.ToReadDto()));
        }

        [HttpGet("{id}")]
        public ActionResult<AtencionReadDto> Get(Guid id)
        {
            var a = _repo.Get(id);
            return a is null ? NotFound() : Ok(a.ToReadDto());
        }

        [HttpPost]
        public ActionResult<AtencionReadDto> Post([FromBody] AtencionCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (!_db.Mascotas.ContainsKey(dto.MascotaId))
                return Problem(detail: "Mascota inexistente", statusCode: 400);
            if (!_db.Clientes.ContainsKey(dto.ClienteId))
                return Problem(detail: "Cliente inexistente", statusCode: 400);

            var entity = new Atencion();
            entity.Apply(dto);
            _repo.Add(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToReadDto());
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] AtencionUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            if (!_db.Mascotas.ContainsKey(dto.MascotaId))
                return Problem(detail: "Mascota inexistente", statusCode: 400);
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
