using Microsoft.AspNetCore.Mvc;
using Veterinaria.Api.Infrastructure.Repositories;
using Veterinaria.Domain.Entities;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Mappings;

namespace Veterinaria.Api.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly EmpleadosRepository _repo;
        public EmpleadosController(EmpleadosRepository repo) => _repo = repo;

        [HttpGet]
        public ActionResult<IEnumerable<EmpleadoReadDto>> GetAll()
            => Ok(_repo.GetAll().Select(e => e.ToReadDto()));

        [HttpGet("{id}")]
        public ActionResult<EmpleadoReadDto> Get(Guid id)
        {
            var e = _repo.Get(id);
            return e is null ? NotFound() : Ok(e.ToReadDto());
        }

        [HttpPost]
        public ActionResult<EmpleadoReadDto> Post([FromBody] EmpleadoCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var entity = new Empleado();
            entity.Apply(dto);
            _repo.Add(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToReadDto());
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] EmpleadoUpdateDto dto)
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
