using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Veterinaria.Api.Infrastructure.RepositoriesSql;
using Veterinaria.Domain.DTOs;

namespace Veterinaria.Api.Controllers
{
    [ApiController]
    [Route("api/catalogo-procedimientos")]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class CatalogoProcedimientosController : ControllerBase
    {
        private readonly ICatalogoProcedimientosSqlRepository _repo;

        public CatalogoProcedimientosController(ICatalogoProcedimientosSqlRepository repo)
            => _repo = repo ?? throw new ArgumentNullException(nameof(repo));

        /// <summary>
        /// Lista el catálogo de procedimientos (por defecto solo activos).
        /// </summary>
        /// <param name="incluirInactivos">Si true, incluye inactivos.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CatalogoProcedimientoReadDto>), 200)]
        public async Task<ActionResult<IReadOnlyList<CatalogoProcedimientoReadDto>>> GetAll([FromQuery] bool incluirInactivos = false)
        {
            var list = await _repo.GetAllAsync(incluirInactivos);
            return Ok(list);
        }

        /// <summary>
        /// Obtiene un ítem del catálogo por su código (p.ej. "CONSULTA").
        /// </summary>
        [HttpGet("{codigo}")]
        [ProducesResponseType(typeof(CatalogoProcedimientoReadDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CatalogoProcedimientoReadDto>> GetByCodigo(string codigo)
        {
            var item = await _repo.GetByCodigoAsync(codigo);
            if (item is null) return NotFound();
            return Ok(item);
        }
    }
}
