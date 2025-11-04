using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinaria.Domain.Entities;
using Veterinaria.Domain.Enums;




namespace Veterinaria.Domain.Entities.Helpers
{

    public static class VacunacionHelper
    {
        /// Devuelve la última fecha de vacunación anual registrada para la mascota.
        public static DateOnly? UltimaVacunacionAnual(Guid mascotaId, IEnumerable<ProcedimientoMascotas> atenciones)
        => atenciones
        .Where(a => a.MascotaId == mascotaId && a.Tipo == TipoProcedimientoMascota.VacunacionAnual)
        .Select(a => a.Fecha)
        .OrderByDescending(f => f)
        .FirstOrDefault();
    }


}
