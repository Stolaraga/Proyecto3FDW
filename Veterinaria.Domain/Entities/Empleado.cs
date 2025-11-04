using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Domain.Enums;

namespace Veterinaria.Domain.Entities
{

    public sealed class Empleado
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        [Required, StringLength(60)]
        public string Nombre { get; set; } = string.Empty;


        [Required, StringLength(80)]
        public string Apellidos { get; set; } = string.Empty;


        [EmailAddress, StringLength(120)]
        public string? Email { get; set; }


        [Phone, StringLength(25)]
        public string? Telefono { get; set; }


        public RolEmpleado Rol { get; set; } = RolEmpleado.Veterinario;


        public DateOnly FechaIngreso { get; set; } = DateOnly.FromDateTime(DateTime.Now);


        public bool Activo { get; set; } = true;
    }


}
