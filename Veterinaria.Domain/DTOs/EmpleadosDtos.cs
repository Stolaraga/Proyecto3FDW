using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Domain.Enums;



namespace Veterinaria.Domain.DTOs
{


    public sealed record EmpleadoCreateDto : BaseCreateDto
    {
        [Required, StringLength(60)] public string Nombre { get; init; } = string.Empty;
        [Required, StringLength(80)] public string Apellidos { get; init; } = string.Empty;
        [EmailAddress, StringLength(120)] public string? Email { get; init; }
        [Phone, StringLength(25)] public string? Telefono { get; init; }
        public RolEmpleado Rol { get; init; } = RolEmpleado.Veterinario;
        public DateOnly? FechaIngreso { get; init; }
    }


    public sealed record EmpleadoUpdateDto : BaseUpdateDto
    {
        [Required, StringLength(60)] public string Nombre { get; init; } = string.Empty;
        [Required, StringLength(80)] public string Apellidos { get; init; } = string.Empty;
        [EmailAddress, StringLength(120)] public string? Email { get; init; }
        [Phone, StringLength(25)] public string? Telefono { get; init; }
        public RolEmpleado Rol { get; init; } = RolEmpleado.Veterinario;
        public DateOnly? FechaIngreso { get; init; }
        public bool Activo { get; init; } = true;
    }


    public sealed record EmpleadoReadDto
    {
        public Guid Id { get; init; }
        public string NombreCompleto { get; init; } = string.Empty;
        public string? Email { get; init; }
        public string? Telefono { get; init; }
        public RolEmpleado Rol { get; init; }
        public DateOnly FechaIngreso { get; init; }
        public bool Activo { get; init; }
    }


}
