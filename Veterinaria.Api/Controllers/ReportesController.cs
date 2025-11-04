using Microsoft.AspNetCore.Mvc;
using Veterinaria.Api.Infrastructure;
using Veterinaria.Domain.Abstractions;
using Veterinaria.Domain.Entities.Helpers;


namespace Veterinaria.Api.Controllers

{

    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly InMemoryStore _db;
        private readonly IClock _clock;
        public ReportesController(InMemoryStore db, IClock clock) { _db = db; _clock = clock; }

        public record ReporteVacunaDto(
            Guid ClienteId, string ClienteNombre,
            Guid MascotaId, string MascotaNombre,
            DateOnly Proxima
        );

        /// <summary>
        /// Retorna las mascotas cuya próxima vacunación anual ocurre entre hoy y los próximos 7 días.
        /// </summary>
        [HttpGet("vacunacion-proxima-semana")]
        [ProducesResponseType(typeof(IEnumerable<ReporteVacunaDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ReporteVacunaDto>> GetVacunacionProximaSemana()
        {
            var hoy = _clock.Today;
            var limite = hoy.AddDays(7);
            var salida = new List<ReporteVacunaDto>();

            foreach (var m in _db.Mascotas.Values)
            {
                var ultima = VacunacionHelper.UltimaVacunacionAnual(m.Id, _db.Atenciones.Values);
                if (ultima is null) continue;

                var proxima = ultima.Value.AddYears(1);
                if (proxima >= hoy && proxima <= limite)
                {
                    var c = _db.Clientes[m.ClienteId];
                    salida.Add(new ReporteVacunaDto(
                        c.Id, $"{c.Nombre} {c.Apellidos}",
                        m.Id, m.Nombre,
                        proxima
                    ));
                }
            }

            return Ok(salida.OrderBy(x => x.Proxima));
        }
    }

}
