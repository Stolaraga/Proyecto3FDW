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
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class ProcedimientoMascotasController : ControllerBase
    {
        private readonly ICitasSqlRepository _citasRepo;
        private readonly IServiciosSqlRepository _serviciosRepo;

        public ProcedimientoMascotasController(
            ICitasSqlRepository citasRepo,
            IServiciosSqlRepository serviciosRepo)
        {
            _citasRepo = citasRepo ?? throw new ArgumentNullException(nameof(citasRepo));
            _serviciosRepo = serviciosRepo ?? throw new ArgumentNullException(nameof(serviciosRepo));
        }

        // GET neutro (sin capa vieja)
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<object>), 200)]
        public ActionResult<IReadOnlyList<object>> GetAll([FromQuery] Guid? mascotaId = null)
            => Ok(Array.Empty<object>());

        // GET/{id} neutro (sin capa vieja)
        [HttpGet("{id:guid}", Name = nameof(GetById))]
        [ProducesResponseType(404)]
        public ActionResult GetById(Guid id) => NotFound();

        /// <summary>
        /// Crea un procedimiento (solo lógica nueva): garantiza/crea Servicio y registra Cita.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Create(
            [FromBody] ProcedimientoMascotaCreateDto dto,
            [FromQuery] int? servicioId = null,
            [FromQuery] string? codigo = null,
            [FromQuery] decimal? peso = null)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // 1) Resolver/crear Servicio
            ServicioRow? servicio = null;

            if (servicioId is not null)
            {
                servicio = await _serviciosRepo.GetByIdAsync(servicioId.Value);
                if (servicio is null)
                    return BadRequest(new { message = $"ServicioId inexistente: {servicioId}" });
            }
            else
            {
                // Nombre objetivo desde 'codigo' (mapeo) o desde enum del DTO
                var nombreTarget = !string.IsNullOrWhiteSpace(codigo)
                    ? MapCodigoToServicioNombre(codigo!)
                    : MapTipoEnumToServicioNombre(dto.Tipo);

                servicio = await _serviciosRepo.GetByNombreAsync(nombreTarget);

                if (servicio is null)
                {
                    // Ya no existe catálogo viejo: si no hay servicio, lo creamos con precio base
                    // tomando dto.Precio (si no vino, 0).
                    var precioBase = dto.Precio ?? 0m;
                    servicio = await _serviciosRepo.AddAsync(nombreTarget, precioBase, true);
                }
            }

            // 2) Calcular precio final
            decimal? precioFinal = dto.Precio;
            if (precioFinal is null)
            {
                var baseP = servicio!.PrecioBase;
                if (EsCirugiaPorKg(servicio.Nombre) && peso is not null && peso > 0)
                    precioFinal = baseP * peso.Value;
                else
                    precioFinal = baseP;
            }

            // Defaults defensivos para compatibilidad
            if (dto.IvaPorcentaje <= 0) dto.IvaPorcentaje = 13m;
            dto.Estado ??= "Agendado";
            if (dto.Fecha == default) dto.Fecha = DateTime.UtcNow;

            // 3) Registrar la CITA en dbo.Citas
            var cita = await _citasRepo.AddAsync(new CitaCreateDto
            {
                MascotaId = dto.MascotaId,
                ServicioId = servicio!.ServicioId,
                VeterinarioId = dto.EmpleadoId,
                FechaHora = dto.Fecha,
                Estado = dto.Estado,
                Notas = dto.Notas
            });

            // 4) Responder 201 sin tocar la tabla antigua
            return Created($"/api/ProcedimientoMascotas", new
            {
                ok = true,
                citaId = cita.Id,
                servicioId = servicio.ServicioId,
                servicio = servicio.Nombre,
                total = precioFinal,
                estado = dto.Estado,
                fecha = dto.Fecha
            });
        }

        private static bool EsCirugiaPorKg(string nombre)
        {
            var n = nombre.ToLowerInvariant();
            return n.Contains("cirugía menor") || n.Contains("cirugia menor")
                || n.Contains("cirugía mayor") || n.Contains("cirugia mayor");
        }

        // Mapea códigos (de tu UI) → nombres en tabla Servicios
        private static string MapCodigoToServicioNombre(string codigo)
        {
            switch (codigo.Trim().ToUpperInvariant())
            {
                case "CONSULTA":
                case "CONSULTA_HORARIO_ESPECIAL": return "Consulta general";
                case "VACUNAS_ANUALES": return "Vacunación";
                case "DESPARASITACION": return "Desparasitación";
                case "CIRUGIA_MENOR": return "Cirugía menor";
                case "CIRUGIA_MAYOR": return "Cirugía mayor";
                default: return codigo;
            }
        }

        // Mapea enum del dominio → nombres en tabla Servicios
        private static string MapTipoEnumToServicioNombre(Veterinaria.Domain.Enums.TipoProcedimientoMascota tipo)
        {
            return tipo switch
            {
                Veterinaria.Domain.Enums.TipoProcedimientoMascota.Consulta => "Consulta general",
                Veterinaria.Domain.Enums.TipoProcedimientoMascota.VacunasAnuales => "Vacunación",
                Veterinaria.Domain.Enums.TipoProcedimientoMascota.Desparasitacion => "Desparasitación",
                Veterinaria.Domain.Enums.TipoProcedimientoMascota.CirugiaMenor => "Cirugía menor",
                Veterinaria.Domain.Enums.TipoProcedimientoMascota.CirugiaMayor => "Cirugía mayor",
                _ => "Consulta general"
            };
        }
    }
}
