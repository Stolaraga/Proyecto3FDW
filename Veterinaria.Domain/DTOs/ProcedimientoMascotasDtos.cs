using System;
using System.ComponentModel.DataAnnotations;
using Veterinaria.Domain.Enums;

namespace Veterinaria.Domain.DTOs
{
    // ===== Crear procedimiento aplicado a una mascota (GUID públicos) =====
    public sealed record ProcedimientoMascotaCreateDto : BaseCreateDto
    {
        [Required] public Guid MascotaId { get; init; }                 // GUID de dbo.Mascotas.Id
        public Guid? ClienteId { get; init; }                           // (opcional) GUID de dbo.Clientes.Id para validar pertenencia
        public Guid? EmpleadoId { get; init; }                          // (opcional) GUID de dbo.Empleados.Id
        [Required] public TipoProcedimientoMascota Tipo { get; init; } = TipoProcedimientoMascota.Consulta;
        [Required] public DateTime Fecha { get; init; }                 // Fecha/hora del procedimiento
        [StringLength(1000)] public string? Notas { get; init; }
    }

    // ===== Actualizar procedimiento =====
    public sealed record ProcedimientoMascotaUpdateDto : BaseUpdateDto
    {
        [Required] public Guid MascotaId { get; init; }
        public Guid? ClienteId { get; init; }
        public Guid? EmpleadoId { get; init; }
        [Required] public TipoProcedimientoMascota Tipo { get; init; } = TipoProcedimientoMascota.Consulta;
        [Required] public DateTime Fecha { get; init; }
        [StringLength(1000)] public string? Notas { get; init; }
    }

    // ===== Lectura / respuesta de la API =====
    public sealed record ProcedimientoMascotaReadDto
    {
        public Guid Id { get; init; }                                   // GUID público del procedimiento (dbo.ProcedimientoMascotas.Id)
        public Guid MascotaId { get; init; }                            // GUID de Mascotas.Id
        public Guid? ClienteId { get; init; }                           // GUID de Clientes.Id (dueño de la mascota)
        public Guid? EmpleadoId { get; init; }                          // GUID de Empleados.Id (si aplica)
        public TipoProcedimientoMascota Tipo { get; init; }
        public DateTime Fecha { get; init; }
        public string? Notas { get; init; }
    }
}
