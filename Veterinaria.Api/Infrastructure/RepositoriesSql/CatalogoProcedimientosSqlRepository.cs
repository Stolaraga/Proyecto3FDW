using Dapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinaria.Domain.DTOs;

namespace Veterinaria.Api.Infrastructure.RepositoriesSql
{
    public interface ICatalogoProcedimientosSqlRepository
    {
        /// <summary> Devuelve el “catálogo” de procedimientos. Ahora se arma desde dbo.Servicios. </summary>
        Task<IReadOnlyList<CatalogoProcedimientoReadDto>> GetAllAsync(bool incluirInactivos = false);

        /// <summary> Busca por “código” (CONSULTA, VACUNAS_ANUALES, etc.). Se mapea desde Nombre de Servicios. </summary>
        Task<CatalogoProcedimientoReadDto?> GetByCodigoAsync(string codigo);
    }

    public sealed class CatalogoProcedimientosSqlRepository : ICatalogoProcedimientosSqlRepository
    {
        private readonly IConnectionFactory _factory;

        public CatalogoProcedimientosSqlRepository(IConnectionFactory factory)
            => _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        public async Task<IReadOnlyList<CatalogoProcedimientoReadDto>> GetAllAsync(bool incluirInactivos = false)
        {
            // Leemos desde dbo.Servicios y mapeamos a un DTO “tipo catálogo”.
            const string sql = @"
SELECT ServicioId, Nombre, PrecioBase, Activo
FROM dbo.Servicios
WHERE (@IncluirInactivos = 1 OR Activo = 1)
ORDER BY Nombre;";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<ServicioRow>(sql, new { IncluirInactivos = incluirInactivos ? 1 : 0 });

            var list = rows.Select(s => new CatalogoProcedimientoReadDto
            {
                Codigo = ToCodigoDesdeNombre(s.Nombre),   // p.ej. “Consulta general” -> "CONSULTA"
                Nombre = s.Nombre,
                Incluye = null,                            // no existe en Servicios
                Precio = s.PrecioBase,
                Activo = s.Activo
            }).ToList();

            return list;
        }

        public async Task<CatalogoProcedimientoReadDto?> GetByCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Debe indicar un código.", nameof(codigo));

            // Traemos todos (la tabla es pequeña) y comparamos por código mapeado.
            const string sql = @"SELECT ServicioId, Nombre, PrecioBase, Activo FROM dbo.Servicios;";

            using var cn = _factory.Create();
            var rows = await cn.QueryAsync<ServicioRow>(sql);

            var codeNorm = NormalizeCode(codigo);

            var srv = rows.FirstOrDefault(s => ToCodigoDesdeNombre(s.Nombre) == codeNorm);
            if (srv is null) return null;

            return new CatalogoProcedimientoReadDto
            {
                Codigo = ToCodigoDesdeNombre(srv.Nombre),
                Nombre = srv.Nombre,
                Incluye = null,
                Precio = srv.PrecioBase,
                Activo = srv.Activo
            };
        }

        // ------ Helpers de mapeo ------

        private static string ToCodigoDesdeNombre(string? nombre)
        {
            var n = (nombre ?? "").Trim();

            // Reglas explícitas (mantienen compatibilidad con lo que ya usabas)
            var low = RemoveDiacritics(n).ToLowerInvariant();
            if (low.Contains("consulta")) return "CONSULTA";
            if (low.Contains("vacun")) return "VACUNAS_ANUALES";
            if (low.Contains("desparasit")) return "DESPARASITACION";
            if (low.Contains("cirugia menor")) return "CIRUGIA_MENOR";
            if (low.Contains("cirugía menor")) return "CIRUGIA_MENOR";
            if (low.Contains("cirugia mayor")) return "CIRUGIA_MAYOR";
            if (low.Contains("cirugía mayor")) return "CIRUGIA_MAYOR";

            // Fallback genérico: “Consulta general” -> “CONSULTA_GENERAL”
            var up = RemoveDiacritics(n).ToUpperInvariant();
            var sb = new StringBuilder(up.Length);
            foreach (var ch in up)
            {
                if (char.IsLetterOrDigit(ch)) sb.Append(ch);
                else sb.Append('_');
            }
            var code = sb.ToString().Trim('_');
            while (code.Contains("__")) code = code.Replace("__", "_");
            return string.IsNullOrWhiteSpace(code) ? "SERVICIO" : code;
        }

        private static string NormalizeCode(string code)
        {
            var up = RemoveDiacritics(code ?? "").ToUpperInvariant();
            var sb = new StringBuilder(up.Length);
            foreach (var ch in up)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_') sb.Append(ch);
                else sb.Append('_');
            }
            var norm = sb.ToString().Trim('_');
            while (norm.Contains("__")) norm = norm.Replace("__", "_");
            return norm;
        }

        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(capacity: normalized.Length);
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // POCO para mapear dbo.Servicios
        private sealed class ServicioRow
        {
            public int ServicioId { get; init; }
            public string Nombre { get; init; } = "";
            public decimal PrecioBase { get; init; }
            public bool Activo { get; init; }
        }
    }
}
