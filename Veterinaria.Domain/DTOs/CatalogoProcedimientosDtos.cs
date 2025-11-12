using System;

namespace Veterinaria.Domain.DTOs
{
    /// <summary>
    /// Ítem del catálogo de procedimientos (para poblar combos/tablas en la Web).
    /// </summary>
    public sealed class CatalogoProcedimientoReadDto
    {
        public string Codigo { get; init; } = string.Empty;   // p.ej. "CONSULTA", "CASTRACION_10_20KG"
        public string Nombre { get; init; } = string.Empty;   // etiqueta amigable
        public string? Incluye { get; init; }                 // descripción "incluye"
        public decimal Precio { get; init; }                  // CRC
        public bool Activo { get; init; }                     // filtrar no-activos
    }
}
