using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Veterinaria.Domain.DTOs;
using Veterinaria.Domain.Services;

namespace Veterinaria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class ProcedimientoMascotasController : ControllerBase
    {
        private readonly IProcedimientoMascotaService _service;

        public ProcedimientoMascotasController(IProcedimientoMascotaService service)
            => _service = service ?? throw new ArgumentNullException(nameof(service));

        /// <summary>
        /// Lista procedimientos; puede filtrar por MascotaId.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ProcedimientoMascotaReadDto>), 200)]
        public async Task<ActionResult<IReadOnlyList<ProcedimientoMascotaReadDto>>> GetAll([FromQuery] Guid? mascotaId = null)
        {
            var result = await _service.ListarAsync(mascotaId);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un procedimiento por Id.
        /// </summary>
        [HttpGet("{id:guid}", Name = nameof(GetById))]
        [ProducesResponseType(typeof(ProcedimientoMascotaReadDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProcedimientoMascotaReadDto>> GetById(Guid id)
        {
            var item = await _service.ObtenerAsync(id);
            if (item is null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Crea un procedimiento.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProcedimientoMascotaReadDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProcedimientoMascotaReadDto>> Create([FromBody] ProcedimientoMascotaCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var created = await _service.CrearAsync(dto);
                return CreatedAtRoute(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                // Errores de negocio/validación (mascota inexistente, ClienteId faltante, etc.)
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un procedimiento.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProcedimientoMascotaUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var ok = await _service.ActualizarAsync(id, dto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un procedimiento.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _service.EliminarAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
