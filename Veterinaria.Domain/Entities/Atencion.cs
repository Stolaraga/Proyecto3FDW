using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Domain.Enums;



namespace Veterinaria.Domain.Entities
{
    /// <summary>
    /// Acto clínico o servicio realizado a una mascota en una fecha.
    /// </summary>
    public sealed class Atencion
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        [Required]
        public Guid MascotaId { get; set; }


        [Required]
        public Guid ClienteId { get; set; }


        public Guid? EmpleadoId { get; set; }


        public TipoAtencion Tipo { get; set; } = TipoAtencion.Consulta;


        [Required]
        public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Now);


        [StringLength(300)]
        public string? Notas { get; set; }
    }


}
