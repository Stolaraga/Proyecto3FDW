using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Domain.Enums;

namespace Veterinaria.Domain.DTOs
{

    public sealed record MascotaCreateDto : BaseCreateDto
    {
        [Required] public Guid ClienteId { get; init; }
        [Required, StringLength(60)] public string Nombre { get; init; } = string.Empty;
        public Especie Especie { get; init; } = Especie.Perro;
        [StringLength(60)] public string? Raza { get; init; }
        public SexoMascota Sexo { get; init; } = SexoMascota.Indeterminado;
        public DateOnly? FechaNacimiento { get; init; }
    }


    public sealed record MascotaUpdateDto : BaseUpdateDto
    {
        [Required] public Guid ClienteId { get; init; }
        [Required, StringLength(60)] public string Nombre { get; init; } = string.Empty;
        public Especie Especie { get; init; } = Especie.Perro;
        [StringLength(60)] public string? Raza { get; init; }
        public SexoMascota Sexo { get; init; } = SexoMascota.Indeterminado;
        public DateOnly? FechaNacimiento { get; init; }
        public bool Activo { get; init; } = true;
    }


    public sealed record MascotaReadDto
    {
        public Guid Id { get; init; }
        public Guid ClienteId { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public Especie Especie { get; init; }
        public string? Raza { get; init; }
        public SexoMascota Sexo { get; init; }
        public DateOnly? FechaNacimiento { get; init; }
        public bool Activo { get; init; }
    }


}
