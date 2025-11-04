using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Domain.Enums;



namespace Veterinaria.Domain.DTOs
{


    public sealed record AtencionCreateDto : BaseCreateDto
    {
        [Required] public Guid MascotaId { get; init; }
        [Required] public Guid ClienteId { get; init; }
        public Guid? EmpleadoId { get; init; }
        public TipoAtencion Tipo { get; init; } = TipoAtencion.Consulta;
        [Required] public DateOnly Fecha { get; init; }
        [StringLength(300)] public string? Notas { get; init; }
    }


    public sealed record AtencionUpdateDto : BaseUpdateDto
    {
        [Required] public Guid MascotaId { get; init; }
        [Required] public Guid ClienteId { get; init; }
        public Guid? EmpleadoId { get; init; }
        public TipoAtencion Tipo { get; init; } = TipoAtencion.Consulta;
        [Required] public DateOnly Fecha { get; init; }
        [StringLength(300)] public string? Notas { get; init; }
    }


    public sealed record AtencionReadDto
    {
        public Guid Id { get; init; }
        public Guid MascotaId { get; init; }
        public Guid ClienteId { get; init; }
        public Guid? EmpleadoId { get; init; }
        public TipoAtencion Tipo { get; init; }
        public DateOnly Fecha { get; init; }
        public string? Notas { get; init; }
    }


}
