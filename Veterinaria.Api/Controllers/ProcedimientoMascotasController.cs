using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veterinaria.Api.Infrastructure.RepositoriesSql;
using Veterinaria.Domain.DTOs;

namespace Veterinaria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ProcedimientoMascotasController : ControllerBase
    {
        private readonly IMascotasSqlRepository _mascRepo;
        private readonly IProcedimientosMascotasSqlRepository _procRepo;

        public ProcedimientoMascotasController(
            IMascotasSqlRepository mascRepo,
            IProcedimientosMascotasSqlRepository procRepo)
        {
            _mascRepo = mascRepo;
            _procRepo = procRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcedimientoMascotaReadDto>>> GetAll([FromQuery] Guid? mascotaId)
        {
            var list = await _procRepo.GetAllAsync(mascotaId);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProcedimientoMascotaReadDto>> Get(Guid id)
        {
            var dto = await _procRepo.GetAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProcedimientoMascotaReadDto>> Post([FromBody] ProcedimientoMascotaCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var mascota = await _mascRepo.GetAsync(dto.MascotaId);
            if (mascota is null)
                return Problem(detail: $"Mascota inexistente: {dto.MascotaId}", statusCode: 400);

            if (dto.ClienteId.HasValue && dto.ClienteId.Value != mascota.ClienteId)
                return Problem(detail: "La mascota no pertenece al cliente indicado.", statusCode: 400);

            try
            {
                var creado = await _procRepo.AddAsync(dto);
                return CreatedAtAction(nameof(Get), new { id = creado.Id }, creado);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(detail: ex.Message, statusCode: 400);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ProcedimientoMascotaUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var mascota = await _mascRepo.GetAsync(dto.MascotaId);
            if (mascota is null)
                return Problem(detail: $"Mascota inexistente: {dto.MascotaId}", statusCode: 400);

            if (dto.ClienteId.HasValue && dto.ClienteId.Value != mascota.ClienteId)
                return Problem(detail: "La mascota no pertenece al cliente indicado.", statusCode: 400);

            try
            {
                var ok = await _procRepo.UpdateAsync(id, dto);
                return ok ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Problem(detail: ex.Message, statusCode: 400);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _procRepo.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
