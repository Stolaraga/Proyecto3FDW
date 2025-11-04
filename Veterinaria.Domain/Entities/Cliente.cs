using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Domain.Enums;


namespace Veterinaria.Domain.Entities
{

    public sealed class Cliente
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        [Required, StringLength(20)]
        public string Cedula { get; set; } = string.Empty;


        [Required, StringLength(60)]
        public string Nombre { get; set; } = string.Empty;


        [Required, StringLength(80)]
        public string Apellidos { get; set; } = string.Empty;


        [EmailAddress, StringLength(120)]
        public string? Email { get; set; }


        [Phone, StringLength(25)]
        public string? Telefono { get; set; }


        public CanalContacto CanalPreferido { get; set; } = CanalContacto.Telefono;


        [StringLength(200)]
        public string? Direccion { get; set; }


        public bool Activo { get; set; } = true;


        public DateOnly FechaRegistro { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }

}
