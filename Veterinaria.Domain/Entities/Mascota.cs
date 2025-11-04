using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Domain.Enums;



namespace Veterinaria.Domain.Entities
{

    public sealed class Mascota
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        [Required]
        public Guid ClienteId { get; set; }


        [Required, StringLength(60)]
        public string Nombre { get; set; } = string.Empty;


        public Especie Especie { get; set; } = Especie.Perro;


        [StringLength(60)]
        public string? Raza { get; set; }


        public SexoMascota Sexo { get; set; } = SexoMascota.Indeterminado;


        public DateOnly? FechaNacimiento { get; set; }


        public bool Activo { get; set; } = true;
    }


}
