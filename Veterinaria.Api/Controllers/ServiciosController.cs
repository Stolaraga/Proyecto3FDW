// Veterinaria.Api/Controllers/ServiciosController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Veterinaria.Api.Infrastructure.RepositoriesSql;

namespace Veterinaria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class ServiciosController : ControllerBase
    {
        private readonly IServiciosSqlRepository _repo;
        public ServiciosController(IServiciosSqlRepository repo) => _repo = repo;

        // GET /api/servicios?soloActivos=true
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ServicioRow>), 200)]
        public async Task<ActionResult<IReadOnlyList<ServicioRow>>> GetAll([FromQuery] bool soloActivos = true)
            => Ok(await _repo.GetAllAsync(soloActivos));

        // GET /api/servicios/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ServicioRow), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ServicioRow>> GetById(int id)
        {
            var s = await _repo.GetByIdAsync(id);
            return s is null ? NotFound() : Ok(s);
        }

        // POST /api/servicios
        [HttpPost]
        [ProducesResponseType(typeof(ServicioRow), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<ServicioRow>> Create([FromBody] ServicioCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Evita duplicados por nombre (también hay UNIQUE en BD)
            var exist = await _repo.GetByNombreAsync(dto.Nombre.Trim());
            if (exist is not null) return Conflict(new { message = "Ya existe un servicio con ese nombre." });

            var created = await _repo.AddAsync(dto.Nombre.Trim(), dto.PrecioBase, dto.Activo);
            return CreatedAtAction(nameof(GetById), new { id = created.ServicioId }, created);
        }

        // PUT /api/servicios/5
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] ServicioUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var ok = await _repo.UpdateAsync(id, dto.Nombre.Trim(), dto.PrecioBase, dto.Activo);
            return ok ? NoContent() : NotFound();
        }

        // DELETE /api/servicios/5  (borrado duro; si prefieres soft-delete, cámbialo en el repo)
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
            => (await _repo.DeleteAsync(id)) ? NoContent() : NotFound();

        
        public sealed class ServicioCreateDto
        {
            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.StringLength(120, MinimumLength = 2)]
            public string Nombre { get; set; } = "";

            [System.ComponentModel.DataAnnotations.Range(0, double.MaxValue)]
            public decimal PrecioBase { get; set; }

            public bool Activo { get; set; } = true;
        }

        public sealed class ServicioUpdateDto
        {
            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.StringLength(120, MinimumLength = 2)]
            public string Nombre { get; set; } = "";

            [System.ComponentModel.DataAnnotations.Range(0, double.MaxValue)]
            public decimal PrecioBase { get; set; }

            public bool Activo { get; set; } = true;
        }



    }
}
