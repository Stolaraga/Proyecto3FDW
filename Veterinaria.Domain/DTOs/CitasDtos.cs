using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Domain.DTOs
{
    public sealed class CitaCreateDto
    {
        public Guid MascotaId { get; set; }
        public int ServicioId { get; set; }
        public Guid? VeterinarioId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; } = "Agendado";
        public string? Notas { get; set; }
    }


    public sealed record CitaUpdateDto : BaseUpdateDto
    {
        [Required] public Guid MascotaId { get; init; }
        [Required] public int ServicioId { get; init; }
        [Required] public Guid VeterinarioId { get; init; }
        [Required] public DateTime FechaHora { get; init; }
        [StringLength(500)] public string? Notas { get; init; }
        [Required] public string Estado { get; init; } = "Pendiente";
    }

    public sealed record CitaReadDto
    {
        public Guid Id { get; init; }

        public Guid MascotaId { get; init; }     // GUID app (desde dbo.Mascotas.Id)
        public int ServicioId { get; init; }    // INT
        public Guid VeterinarioId { get; init; } // GUID app (desde dbo.Empleados.Id)

        public DateTime FechaHora { get; init; }
        public string Estado { get; init; } = "Pendiente";
        public string? Notas { get; init; }

        // Extras útiles para la UI (opcionales)
        public string? NombreMascota { get; init; }
        public string? Servicio { get; init; }
        public string? Veterinario { get; init; }
    }

}
