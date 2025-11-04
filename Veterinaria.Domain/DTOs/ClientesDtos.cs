using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Domain.Enums;



namespace Veterinaria.Domain.DTOs
{

    public sealed record ClienteCreateDto : BaseCreateDto
    {
        [Required, StringLength(20)] public string Cedula { get; init; } = string.Empty;
        [Required, StringLength(60)] public string Nombre { get; init; } = string.Empty;
        [Required, StringLength(80)] public string Apellidos { get; init; } = string.Empty;
        [EmailAddress, StringLength(120)] public string? Email { get; init; }
        [Phone, StringLength(25)] public string? Telefono { get; init; }
        public CanalContacto CanalPreferido { get; init; } = CanalContacto.Telefono;
        [StringLength(200)] public string? Direccion { get; init; }
    }


    public sealed record ClienteUpdateDto : BaseUpdateDto
    {
        [Required, StringLength(20)] public string Cedula { get; init; } = string.Empty;
        [Required, StringLength(60)] public string Nombre { get; init; } = string.Empty;
        [Required, StringLength(80)] public string Apellidos { get; init; } = string.Empty;
        [EmailAddress, StringLength(120)] public string? Email { get; init; }
        [Phone, StringLength(25)] public string? Telefono { get; init; }
        public CanalContacto CanalPreferido { get; init; } = CanalContacto.Telefono;
        [StringLength(200)] public string? Direccion { get; init; }
        public bool Activo { get; init; } = true;
    }


    public sealed record ClienteReadDto
    {
        public Guid Id { get; init; }
        public string Cedula { get; init; } = string.Empty;
        public string NombreCompleto { get; init; } = string.Empty;
        public string? Email { get; init; }
        public string? Telefono { get; init; }
        public CanalContacto CanalPreferido { get; init; }
        public string? Direccion { get; init; }
        public bool Activo { get; init; }
        public DateOnly FechaRegistro { get; init; }
    }



}
